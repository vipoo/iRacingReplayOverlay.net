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

using iRacingReplayOverlay.Phases.Transcoding;
using MediaFoundation.Net;

namespace iRacingReplayOverlay.Video
{
    public partial class Process
    {
        public static ProcessSample OverlayRaceData(LeaderBoard leaderBoard, ProcessSample next)
        {
            return sample =>
                {
                    if (!sample.Stream.NativeMediaType.IsVideo)
                        return true;

                    var videoSample = new SourceReaderSampleWithBitmap(sample);

                    leaderBoard.Overlay(videoSample.Graphic, sample.Timestamp);

                    //if (frame.Timestamp != 0)
                    //    progress(frame.Timestamp, frame.Duration);

                    return true;
                };
        }
    }
}
