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

using iRacingSDK;
using System.Collections.Generic;

namespace iRacingReplayOverlay.Phases.Analysis
{
    public struct LapToFrameNum
    {
        public LapToFrameNum(int lapNumber, int frameNumber, double sessionTime)
        {
            this.sessionTime = sessionTime;
            this.LapNumber = lapNumber;
            this.FrameNumber = frameNumber;
        }

        public readonly int LapNumber;
        public readonly int FrameNumber;
        public readonly double sessionTime;
    }

    public class LapsToFrameNumbers : IEnumerable<LapToFrameNum>
    {
        Dictionary<int, LapToFrameNum> frameNumberByLap = new Dictionary<int, LapToFrameNum>();
        int lastRaceLaps = -1;

        public void Process(DataSample data)
        {
            if (lastRaceLaps != data.Telemetry.RaceLaps)
            {
                lastRaceLaps = data.Telemetry.RaceLaps;

                frameNumberByLap.Add(lastRaceLaps, new LapToFrameNum(lastRaceLaps, data.Telemetry.ReplayFrameNum, data.Telemetry.SessionTime));
            }
        }

        public LapToFrameNum this[int lapNumber]
        {
            get
            {
                return frameNumberByLap[lapNumber];
            }
        }

        public IEnumerator<LapToFrameNum> GetEnumerator()
        {
            return frameNumberByLap.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return frameNumberByLap.Values.GetEnumerator();
        }
    }
}
