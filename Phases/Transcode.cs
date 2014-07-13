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
using iRacingReplayOverlay.Video;
using MediaFoundation.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
        
        public void _WithEncodingOf(int videoBitRate, int audioBitRate)
        {
            this.videoBitRate = videoBitRate;
            this.audioBitRate = audioBitRate;
        }

        public void _WithFiles(string sourceFile)
        {
            this.sourceFile = sourceFile;
            this.destinationFile = Path.ChangeExtension(sourceFile, "wmv");
            this.destinationHighlightsFile = Path.ChangeExtension(sourceFile, ".highlights.wmv");

            this.gameDataFile = Path.ChangeExtension(sourceFile, "xml");
        }

        public void _OverlayRaceDataOntoVideo(_Progress progress, Action completed, Action readFramesCompleted)
        {
            var overlayData = OverlayData.FromFile(gameDataFile);

            const bool TranscodeHightlights = true;
            const bool TranscodeFull = true;
            
            Task transcodeHigh;
            Task transcodeFull;

            Func<ProcessSample, ProcessSample> monitor = null;
            if( !TranscodeFull)
                monitor = next => MonitorProgress(progress, readFramesCompleted, next);

            transcodeHigh = new Task(() => ApplyTransformationsToVideo(overlayData, destinationHighlightsFile, true, monitor));
            transcodeFull = new Task(() => ApplyTransformationsToVideo(overlayData, destinationFile, false, next => MonitorProgress(progress, readFramesCompleted, next)));

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

        void ApplyTransformationsToVideo(OverlayData overlayData, string destFile, bool highlightsOnly, Func<ProcessSample, ProcessSample> monitorProgress = null)
        {
            try
            {
                var leaderBoard = new LeaderBoard { OverlayData = overlayData };

                var transcoder = new Transcoder
                {
                    IntroVideoFile = overlayData.IntroVideoFileName,
                    SourceFile = sourceFile,
                    DestinationFile = destFile,
                    VideoBitRate = videoBitRate,
                    AudioBitRate = audioBitRate
                };

                transcoder.ProcessVideo((introSourceReader, sourceReader, saveToSink) =>
                {
                    Action<ProcessSample> mainFeed;

                    if (monitorProgress == null)
                        mainFeed = next => sourceReader.Samples(OverlayRaceDataToVideo(leaderBoard, next));
                    else
                        mainFeed = next => sourceReader.Samples(monitorProgress(OverlayRaceDataToVideo(leaderBoard, next)));

                    var writeToSinks = highlightsOnly ? ApplyEdits(leaderBoard, saveToSink) : saveToSink;

                    if (introSourceReader == null)
                        mainFeed(writeToSinks);
                    else
                    {
                        Action<ProcessSample> introFeed = next => introSourceReader.Samples(
                            ApplyIntroTitles(leaderBoard, AVOperation.FadeIn(AVOperation.FadeOut(introSourceReader.MediaSource, next))));

                        AVOperation.Concat(introFeed, mainFeed, writeToSinks);
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

        ProcessSample ApplyIntroTitles(LeaderBoard leaderBoard, ProcessSample next)
        {
            return AVOperations.SeperateAudioVideo(next, AVOperation.DataSamplesOnly(IntroTitles(leaderBoard, next), next));
        }

        ProcessSample IntroTitles(LeaderBoard leaderBoard, ProcessSample next)
        {
            return sample =>
                {
                    using (var sampleWithBitmap = new SourceReaderSampleWithBitmap(sample))
                        leaderBoard.Intro(sampleWithBitmap.Graphic, sampleWithBitmap.Timestamp);

                    return next(sample);
                };
        }

        ProcessSample MonitorProgress(_Progress progress, Action readFramesCompleted, ProcessSample next)
        {
            return sample =>
            {
                if (sample.Flags.EndOfStream)
                    readFramesCompleted();
                else
                {
                    if (sample.Timestamp != 0)
                        progress(sample.Timestamp, sample.Duration);
                }

                return !requestAbort && next(sample);
            };
        }

        ProcessSample ApplyEdits(LeaderBoard leaderBoard, ProcessSample next)
        {
            var cut = next;

            foreach (var editCut in leaderBoard.OverlayData.RaceEvents.GetRaceEdits())
                cut = AVOperation.ApplyEditWithFade(editCut.StartTime.FromSecondsToNano(), editCut.EndTime.FromSecondsToNano(), cut);

            return cut;
        }

        ProcessSample OverlayRaceDataToVideo(LeaderBoard leaderBoard, ProcessSample next)
        {
            var overlays = AVOperation.DataSamplesOnly(OverlayRaceData(leaderBoard, AVOperation.FadeIn(next)), next);

            return AVOperations.SeperateAudioVideo(next, overlays);
        }

        ProcessSample OverlayRaceData(LeaderBoard leaderBoard, ProcessSample next)
        {
            return sample =>
            {
                using (var sampleWithBitmap = new SourceReaderSampleWithBitmap(sample))
                    leaderBoard.Overlay(sampleWithBitmap.Graphic, sampleWithBitmap.Timestamp);

                return next(sample);
            };
        }
    }
}
