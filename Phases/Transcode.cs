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
using iRacingSDK;
using MediaFoundation.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            this.gameDataFile = Path.ChangeExtension(sourceFile, "xml");
        }

        public void _OverlayRaceDataOntoVideo(_Progress progress, Action completed, Action readFramesCompleted)
        {
            var overlayData = OverlayData.FromFile(gameDataFile);

            var leaderBoard = new LeaderBoard
            {
                OverlayData = overlayData
            };

            var transcoder = new Transcoder
            {
                IntroVideoFile = overlayData.IntroVideoFileName,
                SourceFile = sourceFile,
                DestinationFile = destinationFile,
                VideoBitRate = videoBitRate,
                AudioBitRate = audioBitRate
            };

            transcoder.ProcessVideo((introSourceReader, sourceReader, saveToSink) =>
            {
                Action<ProcessSample> introFeed = (next) => introSourceReader.Samples(
                    AVOperation.FadeIn(AVOperation.FadeOut(introSourceReader.MediaSource, next)));

                Action<ProcessSample> mainFeed = (next) => sourceReader.Samples(
                    MonitorProgress(progress, readFramesCompleted, 
                        RaceHightlights(leaderBoard, next)));

                AVOperation.Concat(introFeed, mainFeed, saveToSink);
            });

            completed();
        }

        private ProcessSample MonitorProgress(_Progress progress, Action readFramesCompleted, ProcessSample next)
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

        private ProcessSample RaceHightlights(LeaderBoard leaderBoard, ProcessSample next)
        {
            ProcessSample cut = next;

            foreach (var editCut in leaderBoard.OverlayData.EditCuts)
                cut = AVOperation.ApplyEditWithFade(editCut.StartTime.FromSecondsToNano(), editCut.EndTime.FromSecondsToNano(), cut);

            var overlays = AVOperation.DataSamplesOnly(OverlayRaceData(leaderBoard, AVOperation.FadeIn(cut)), cut);

            return AVOperation.SeperateAudioVideo(cut, overlays);
        }

        public ProcessSample OverlayRaceData(LeaderBoard leaderBoard, ProcessSample next)
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
