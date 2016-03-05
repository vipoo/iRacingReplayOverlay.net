// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
//
// iRacingReplayOverlay is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayOverlay is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.
//

using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Phases.Transcoding;
using MediaFoundation.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using iRacingSDK.Support;
using System.Reflection;

namespace iRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        int videoBitRate;
        int audioBitRate;
        string destinationFile;
        string destinationHighlightsFile;
        string gameDataFile;

        public void _WithEncodingOf(int videoBitRate, int audioBitRate)
        {
            this.videoBitRate = videoBitRate;
            this.audioBitRate = audioBitRate;
        }

        public void _WithOverlayFile(string overlayFileName)
        {
            destinationFile = Path.ChangeExtension(overlayFileName, "wmv");
            destinationHighlightsFile = Path.ChangeExtension(overlayFileName, ".highlights.wmv");

            gameDataFile = overlayFileName;
        }

        public void _OverlayRaceDataOntoVideo(Action<long, long> progress, Action completed, bool highlightOnly)
        {
            var myLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var info = new AppDomainSetup();
            var domain = AppDomain.CreateDomain("TranscodingDomain", null, info);
            try
            {
                var a = new Arguments(gameDataFile, videoBitRate, audioBitRate, destinationHighlightsFile, destinationFile, progress, completed, highlightOnly);

                domain.DoCallBack(a.OverlayRaceDataOntoVideo);
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        public class Arguments : MarshalByRefObject
        {
            string gameDataFile;
            int videoBitRate;
            int audioBitRate;
            string destinationFile;
            string destinationHighlightsFile;
            Action<long, long> progressReporter;
            Action completed;
            bool highlightOnly;

            public Arguments(string gameDataFile, int videoBitRate, int audioBitRate, string destinationHighlightsFile, string destinationFile, Action<long, long> progressReporter, Action completed, bool highlightOnly)
            {
                this.gameDataFile = gameDataFile;
                this.videoBitRate = videoBitRate;
                this.audioBitRate = audioBitRate;
                this.destinationHighlightsFile = destinationHighlightsFile;
                this.destinationFile = destinationFile;
                this.progressReporter = progressReporter;
                this.completed = completed;
                this.highlightOnly = highlightOnly;
            }

            public void OverlayRaceDataOntoVideo()
            {
                __OverlayRaceDataOntoVideo(gameDataFile, videoBitRate, audioBitRate, destinationHighlightsFile, destinationFile, progressReporter, completed, highlightOnly);
            }
        }

        public static void __OverlayRaceDataOntoVideo(string gameDataFile, int videoBitRate, int audioBitRate, string destinationHighlightsFile, string destinationFile, Action<long, long> progress, Action completed, bool highlightOnly)
        {
            bool TranscodeFull = !highlightOnly;

            var transcodeHigh = new Task(() => TranscodeAndOverlay.Apply(gameDataFile, videoBitRate, audioBitRate, destinationHighlightsFile, true, highlightOnly ? progress : null));
            var transcodeFull = new Task(() => TranscodeAndOverlay.Apply(gameDataFile, videoBitRate, audioBitRate, destinationFile, false, progress));

            using (MFSystem.Start())
            {
                var waits = new List<Task>();

                transcodeHigh.Start();
                waits.Add(transcodeHigh);

                //Seem to have some kind of bug in MediaFoundation - where if two threads attempt to open source Readers to the same file, we get exception raised.
                //To work around issue, delay the start of the second transcoder - so we dont have two threads opening at the same time.
                if (TranscodeFull)
                {
                    Thread.Sleep(10000);
                    transcodeFull.Start();
                    waits.Add(transcodeFull);
                }

                Task.WaitAll(waits.ToArray());
            }
            completed();
        }
    }

    public class TranscodeAndOverlay
    {

        readonly LeaderBoard leaderBoard;
        long totalDuration;
        readonly Action<long, long> progressReporter;

        private TranscodeAndOverlay(LeaderBoard leaderBoard, Action<long, long> progressReporter )
        {
            this.leaderBoard = leaderBoard;
            this.progressReporter = progressReporter;
        }

        public static void Apply(string gameDataFile, int videoBitRate, int audioBitRate, string destFile, bool highlights, Action<long, long> progressReporter)
        {
            try
            {
                var leaderBoard = new LeaderBoard { OverlayData = OverlayData.FromFile(gameDataFile) };

                var transcoder = new Transcoder
                {
                    VideoFiles = leaderBoard.OverlayData.VideoFiles.ToSourceReaderExtra(),
                    DestinationFile = destFile,
                    VideoBitRate = videoBitRate,
                    AudioBitRate = audioBitRate
                };

                new TranscodeAndOverlay(leaderBoard, progressReporter).Process(transcoder, highlights, progressReporter);
            }
            catch (Exception e)
            {
                TraceError.WriteLine(e.Message);
                TraceError.WriteLine(e.StackTrace);
                throw e;
            }
        }
        
        void Process(Transcoder transcoder, bool highlights, Action<long, long> monitorProgress)
        {
            try
            {
                TraceInfo.WriteLineIf(highlights, "Transcoding highlights to {0}", transcoder.DestinationFile);
                TraceInfo.WriteLineIf(!highlights, "Transcoding full replay to {0}", transcoder.DestinationFile);

                transcoder.ProcessVideo((readers, saveToSink) =>
                {
                    var writeToSink = monitorProgress == null ? saveToSink : MonitorProgress(saveToSink);

                    var fadeSegments = AVOperations.FadeIn(AVOperations.FadeOut(writeToSink));
                    var edits = highlights ? ApplyEdits(writeToSink) : writeToSink;
                    var mainBodyOverlays = AVOperations.Overlay(applyRaceDataOverlay, edits);
                    var introOverlay = AVOperations.Overlay(applyIntroOverlay, fadeSegments);

                    var sourceReaderExtra = readers.FirstOrDefault(r => ((CapturedVideoFile)r.State).isIntroVideo);
                    if (sourceReaderExtra != null)
                    {
                        var introSourceReader = sourceReaderExtra.SourceReader;
                        var mainReaders = AVOperations.Combine(readers.Skip(1).Select(r => r.SourceReader).ToArray(), Settings.Default.VideoSplitGap);

                        totalDuration += introSourceReader.Duration + mainReaders.Duration;
                        
                        AVOperations.StartConcat(introSourceReader, introOverlay,
                            AVOperations.Concat(mainReaders, mainBodyOverlays));
                    }
                    else
                    {
                        var mainReaders = AVOperations.Combine(readers.Select(r => r.SourceReader).ToArray(), Settings.Default.VideoSplitGap);

                        totalDuration += mainReaders.Duration;

                        AVOperations.Concat(mainReaders, mainBodyOverlays)(0, 0);
                    }
                });


                TraceInfo.WriteLineIf(highlights, "Done Transcoding highlights to {0}", transcoder.DestinationFile);
                TraceInfo.WriteLineIf(!highlights, "Done Transcoding full replay to {0}", transcoder.DestinationFile);
            }
            catch (Exception e)
            {
                TraceError.WriteLine(e.Message);
                TraceError.WriteLine(e.StackTrace);
                throw e;
            }
        }

        void applyRaceDataOverlay(SourceReaderSampleWithBitmap sample)
        {
            if (showClosingFlashCard(sample))
            {
                if (sample.Timestamp.FromNanoToSeconds() - leaderBoard.OverlayData.TimeForOutroOverlay.Value > 30)
                    return;

                var duration = sample.Duration - leaderBoard.OverlayData.TimeForOutroOverlay.Value.FromSecondsToNano();
                duration = Math.Min(duration, 30.FromSecondsToNano());
                var period = sample.Timestamp - leaderBoard.OverlayData.TimeForOutroOverlay.Value.FromSecondsToNano();

                leaderBoard.Outro(sample.Graphic, duration, sample.Timestamp, period);
                return;
            }

            leaderBoard.Overlay(sample.Graphic, sample.Timestamp);
        }

        void applyIntroOverlay(SourceReaderSampleWithBitmap sample)
        {
            leaderBoard.Intro(sample.Graphic, sample.Duration, sample.Timestamp);
        }
        
        ProcessSample ApplyEdits(ProcessSample next)
        {
            var cut = next;

            var firstEdit = leaderBoard.OverlayData.RaceEvents.GetRaceEdits().First();
            var lastEdit = leaderBoard.OverlayData.RaceEvents.GetRaceEdits().Last();

            foreach (var editCut in leaderBoard.OverlayData.RaceEvents.GetRaceEdits())
            {
                cut = AVOperations.Cut(editCut.StartTime.FromSecondsToNano(), editCut.EndTime.FromSecondsToNano(), AVOperations.FadeInOut(cut));
                totalDuration -= editCut.EndTime.FromSecondsToNano() - editCut.StartTime.FromSecondsToNano();
            }

            return cut;
        }
       
        bool showClosingFlashCard( SourceReaderSample sample)
        {
            if (!leaderBoard.OverlayData.TimeForOutroOverlay.HasValue)
                return false;

            return sample.Timestamp >= leaderBoard.OverlayData.TimeForOutroOverlay.Value.FromSecondsToNano();
        }

        ProcessSample MonitorProgress(ProcessSample next)
        {
            return sample =>
            {
                if (!sample.Flags.EndOfStream && progressReporter != null)
                    progressReporter(sample.SampleTime, totalDuration);

                return next(sample);
            };
        }
    }
}
