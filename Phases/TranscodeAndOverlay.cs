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
using System.Linq;
using iRacingSDK.Support;
using System.Reflection;
using System.Threading;

namespace iRacingReplayOverlay.Phases
{
    public class TranscodeAndOverlayMarshaled
    {
        public static void Apply(string name, string gameDataFile, int videoBitRate, int audioBitRate, string destFile, bool highlights, Action<long, long> progressReporter, CancellationToken token)
        {
            var domain = AppDomain.CreateDomain(name, null, new AppDomainSetup());
            try
            {
                var hostArgs = new TranscodeAndOverlayArguments(progressReporter, () => token.IsCancellationRequested);

                var arg = (TranscodeAndOverlayArguments)domain.CreateInstanceFromAndUnwrap(
                    typeof(TranscodeAndOverlayArguments).Assembly.Location,
                    typeof(TranscodeAndOverlayArguments).FullName,
                    false,
                    BindingFlags.CreateInstance,
                    null,
                    new object[] { gameDataFile, videoBitRate, audioBitRate, destFile, highlights, (Action<long, long>)hostArgs.ProgressReporter, (Func<bool>)hostArgs.IsAborted },
                    null,
                    null);
                arg.Apply();
            }
            catch (Exception e)
            {
                TraceError.WriteLine(e.Message);
                TraceError.WriteLine(e.StackTrace);
                throw e;
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }
    }

    public class TranscodeAndOverlayArguments : MarshalByRefObject
    {
        private int audioBitRate;
        private string destFile;
        private string gameDataFile;
        private bool highlights;
        private Func<bool> _isAborted;
        private Action<long, long> _progressReporter;
        private int videoBitRate;

        public TranscodeAndOverlayArguments(Action<long, long> progressReporter, Func<bool> isAborted)
        {
            this._progressReporter = progressReporter;
            this._isAborted = isAborted;
        }

        public TranscodeAndOverlayArguments(string gameDataFile, int videoBitRate, int audioBitRate, string destFile, bool highlights, Action<long, long> progressReporter, Func<bool> isAborted)
        {
            this.gameDataFile = gameDataFile;
            this.videoBitRate = videoBitRate;
            this.audioBitRate = audioBitRate;
            this.destFile = destFile;
            this.highlights = highlights;
            this._progressReporter = progressReporter;
            this._isAborted = isAborted;
        }

        public bool IsAborted()
        {
            return _isAborted();
        }

        public void ProgressReporter(long a, long b)
        {
            if(_progressReporter != null)
                _progressReporter(a, b);
        }

        public void Apply()
        {
            TranscodeAndOverlay.Apply(gameDataFile, videoBitRate, audioBitRate, destFile, highlights, ProgressReporter, IsAborted);
        }
    }

    public class TranscodeAndOverlay 
    {
        public static void Apply(string gameDataFile, int videoBitRate, int audioBitRate, string destFile, bool highlights, Action<long, long> progressReporter, Func<bool> isAborted)
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

                new TranscodeAndOverlay(leaderBoard, progressReporter).Process(transcoder, highlights, progressReporter, isAborted);
            }
            catch (Exception e)
            {
                TraceError.WriteLine(e.Message);
                TraceError.WriteLine(e.StackTrace);
                throw e;
            }
        }

        readonly LeaderBoard leaderBoard;
        long totalDuration;
        readonly Action<long, long> progressReporter;

        private TranscodeAndOverlay(LeaderBoard leaderBoard, Action<long, long> progressReporter)
        {
            this.leaderBoard = leaderBoard;
            this.progressReporter = progressReporter;
        }

        void Process(Transcoder transcoder, bool highlights, Action<long, long> monitorProgress, Func<bool> isAborted)
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
                            AVOperations.Concat(mainReaders, mainBodyOverlays, isAborted), isAborted);
                    }
                    else
                    {
                        var mainReaders = AVOperations.Combine(readers.Select(r => r.SourceReader).ToArray(), Settings.Default.VideoSplitGap);

                        totalDuration += mainReaders.Duration;

                        AVOperations.Concat(mainReaders, mainBodyOverlays, isAborted)(0, 0);
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
