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

namespace iRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        public delegate void _Progress(long count, long duration);

        int videoBitRate;
        int audioBitRate;
        string sourceFile;
        string destinationFile;
        string destinationHighlightsFile;
        string gameDataFile;
        long totalDuration;
        LeaderBoard leaderBoard;

        public void _WithEncodingOf(int videoBitRate, int audioBitRate)
        {
            this.videoBitRate = videoBitRate;
            this.audioBitRate = audioBitRate;
        }

        public void _WithFiles(string sourceFile)
        {
            this.sourceFile = sourceFile;
            destinationFile = Path.ChangeExtension(sourceFile, "wmv");
            destinationHighlightsFile = Path.ChangeExtension(sourceFile, ".highlights.wmv");

            gameDataFile = Path.ChangeExtension(sourceFile, "xml");
            leaderBoard = new LeaderBoard { OverlayData = OverlayData.FromFile(gameDataFile) };
        }

        public void _OverlayRaceDataOntoVideo(_Progress progress, Action completed, bool highlightOnly)
        {
            const bool TranscodeHightlights = true;
            bool TranscodeFull = !highlightOnly;
            
            Task transcodeHigh;
            Task transcodeFull;

            Func<ProcessSample, ProcessSample> monitor = null;
            if( !TranscodeFull)
                monitor = next => MonitorProgress(progress, next);

            transcodeHigh = new Task(() => ApplyTransformationsToVideo(destinationHighlightsFile, true, monitor));
            transcodeFull = new Task(() => ApplyTransformationsToVideo(destinationFile, false, next => MonitorProgress(progress, next)));

            using (MFSystem.Start())
            {
                var waits = new List<Task>();

                if (TranscodeHightlights)
                {
                    transcodeHigh.Start();
                    waits.Add(transcodeHigh);
                }
                
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

        void ApplyTransformationsToVideo(string destFile, bool highlightsOnly, Func<ProcessSample, ProcessSample> monitorProgress = null)
        {
            try
            {
                var transcoder = new Transcoder
                {
                    IntroVideoFile = leaderBoard.OverlayData.IntroVideoFileName,
                    SourceFile = sourceFile,
                    DestinationFile = destFile,
                    VideoBitRate = videoBitRate,
                    AudioBitRate = audioBitRate
                };

                transcoder.ProcessVideo((introSourceReader, sourceReader, saveToSink) =>
                {
                    var writeToSink = monitorProgress == null ? saveToSink : monitorProgress(saveToSink);

                    var fadeSegments = AVOperations.FadeIn(AVOperations.FadeOut(writeToSink));
                    var edits = AVOperations.Overlay(applyRaceDataOverlay, ApplyEdits(writeToSink));
                    var introOverlay = AVOperations.Overlay(applyIntroOverlay, fadeSegments);

                    if (introSourceReader != null)
                    {
                        totalDuration += (long)introSourceReader.MediaSource.Duration + (long)sourceReader.MediaSource.Duration;

                        AVOperations.StartConcat(introSourceReader, introOverlay,
                            AVOperations.Concat(sourceReader, edits));
                    }
                    else
                    {
                        totalDuration += (long)sourceReader.MediaSource.Duration;

                        sourceReader.Samples(edits);
                    }
                });
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message, "DEBUG");
                Trace.WriteLine(e.StackTrace, "DEBUG");
                throw;
            }
        }

        void applyRaceDataOverlay(SourceReaderSampleWithBitmap sample)
        {
            if (showClosingFlashCard(sample))
            {
                var duration = sample.Duration - (long)leaderBoard.OverlayData.TimeForOutroOverlay.Value.FromSecondsToNano();
                duration = Math.Min(duration, 30.FromSecondsToNano());
                var period = sample.Timestamp - leaderBoard.OverlayData.TimeForOutroOverlay.Value.FromSecondsToNano();
                var page = GetPageNumber((float)period / duration);

                leaderBoard.Outro(sample.Graphic, sample.Timestamp, page);
            }
            else
                leaderBoard.Overlay(sample.Graphic, sample.Timestamp);
        }

        void applyIntroOverlay(SourceReaderSampleWithBitmap sample)
        {
            var pagePeriod = (float)sample.Timestamp / sample.Duration;

            int page = GetPageNumber( pagePeriod);

            leaderBoard.Intro(sample.Graphic, sample.Timestamp, page);
        }
        
        ProcessSample MonitorProgress(_Progress progress, ProcessSample next)
        {
            return sample =>
            {
                if (!sample.Flags.EndOfStream)
                    progress(sample.SampleTime, totalDuration);

                return !requestAbort && next(sample);
            };
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

        int GetNumberOfPages()
        {
            var numberOfDrivers = leaderBoard.OverlayData.SessionData.DriverInfo.CompetingDrivers.Length;
            var numberOfPages = Math.Min(numberOfDrivers / LeaderBoard.DriversPerPage, 3);
            if (((float)numberOfDrivers % LeaderBoard.DriversPerPage) != 0)
                numberOfPages++;

            return numberOfPages;
        }

        int GetPageNumber(float pagePeriod)
        {
            var numberOfPages = GetNumberOfPages();

            var page = (int)Math.Floor(pagePeriod * numberOfPages);
            return Math.Min(page, numberOfPages-1);
        }

        bool showClosingFlashCard(LeaderBoard leaderBoard, SourceReaderSample sample)
        {
            if (!leaderBoard.OverlayData.TimeForOutroOverlay.HasValue)
                return false;

            return sample.Timestamp >= leaderBoard.OverlayData.TimeForOutroOverlay.Value.FromSecondsToNano();
        }
    }
}
