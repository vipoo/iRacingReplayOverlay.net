using iRacingSDK.Support;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace iRacingDirector
{
    public class LogListener : TraceListener
    {
        static LogListener logFile;
        ConcurrentQueue<string> items = new ConcurrentQueue<string>();
        bool isCancelled = false;
        Task task;
        string lastMessage = null;
        DateTime lastTime = DateTime.Now;

        public string FileName { get; internal set; }

        LogListener(string filename)
        {
            this.FileName = filename;

            task = Task.Factory.StartNew(Writer);

            Write("\r\n");
            WriteLine("----------------------------");
        }

        void Writer()
        {
            while (!isCancelled)
            {
                string message;
                if (items.TryDequeue(out message))
                {
                    try { File.AppendAllText(FileName, message); }
                    catch (Exception)
                    {
                        items.Enqueue(message);
                    }
                }
                Thread.Sleep(1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            isCancelled = true;

            base.Dispose(disposing);
        }

        public override void Write(string message)
        {
            items.Enqueue(message);
        }

        public override void WriteLine(string message)
        {
            var now = DateTime.Now;
            if (message == lastMessage && now - lastTime < TimeSpan.FromSeconds(20))
                return;

            lastMessage = message;
            lastTime = now;

            this.Write(now.ToString("s"));
            this.Write("\t");
            this.Write(message + "\r\n");
        }

        internal static void ToFile(string filename)
        {
            var oldLogFile = logFile;
            logFile = new LogListener(filename);
            Trace.Listeners.Add(logFile);

            if (oldLogFile != null)
            {
                TraceInfo.WriteLine("Moving logging to file {0}", filename);

                WaitToQueueEmpty(oldLogFile);

                Trace.Listeners.Remove(oldLogFile);
                oldLogFile.Dispose();
            }
        }

        private static void WaitToQueueEmpty(LogListener oldLogFile)
        {
            var count = 10;
            while (count-- > 0 && oldLogFile.items.Count > 0)
                Thread.Sleep(10);
        }

        internal static void MoveToFile(string filename)
        {
            if (logFile == null)
                throw new Exception("Attempt to rename non-existing log file to {0}".F(filename));

            TraceInfo.WriteLine("Renaming logging to file {0}", filename);

            WaitToQueueEmpty(logFile);

            Trace.Listeners.Remove(logFile);
            logFile.Dispose();

            var retryCount = 2;
            while (retryCount > 0)
            {
                try
                {
                    File.Move(logFile.FileName, filename);
                    retryCount = 0;
                }
                catch (IOException)
                {
                    retryCount--;
                    Thread.Sleep(1000);
                }
            }

            logFile = new LogListener(filename);
            Trace.Listeners.Add(logFile);
        }
    }
}