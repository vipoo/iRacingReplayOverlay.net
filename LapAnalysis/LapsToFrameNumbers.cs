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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.net.LapAnalysis
{
    public struct LapToFrameNum
    {
        public LapToFrameNum(int lapNumber, int frameNumber)
        {
            this.LapNumber = lapNumber;
            this.FrameNumber = frameNumber;
        }

        public readonly int LapNumber;
        public readonly int FrameNumber;
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

                frameNumberByLap.Add(lastRaceLaps, new LapToFrameNum(lastRaceLaps, data.Telemetry.ReplayFrameNum));
            }
        }

        public int this[int lapNumber]
        {
            get
            {
                return frameNumberByLap[lapNumber].FrameNumber ;
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
