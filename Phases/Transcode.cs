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
using iRacingSDK;
using System;
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
            var leaderBoard = new LeaderBoard
            {
                OverlayData = OverlayData.FromFile(gameDataFile)
            };

            var transcoder = new Transcoder
            {
                IntroVideoFile = leaderBoard.OverlayData.IntroVideoFileName,
                SourceFile = sourceFile,
                DestinationFile = destinationFile,
                VideoBitRate = videoBitRate,
                AudioBitRate = audioBitRate,
                EditCuts = leaderBoard.OverlayData.EditCuts
            };

            transcoder.Frames(frame => 
            {
                if (frame.Flags.EndOfStream)
                    readFramesCompleted();
                else
                {
                    leaderBoard.Overlay(frame.Graphic, frame.Timestamp);

                    if (frame.Timestamp != 0)
                        progress(frame.Timestamp, frame.Duration);
                }

                return !requestAbort;
            });

            completed();
        }
    }
}
