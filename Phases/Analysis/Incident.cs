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

using iRacingReplayOverlay.Support;
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Analysis
{
    public class Incidents : IEnumerable<Incidents.Incident>
    {
        public class Incident
        {
            public int LapNumber;
            public double StartSessionTime;
            public double EndSessionTime;
            public int StartFrameNumber;
            public int EndFrameNumber;
            public int CarIdx;
        }

        List<Incident> incidents = new List<Incident>();
        Incident lastIncident;

        public void Process(DataSample data)
        {
            var i = new Incident 
            {
                LapNumber = data.Telemetry.RaceLaps, 
                CarIdx = data.Telemetry.CamCarIdx, 
                StartFrameNumber = data.Telemetry.ReplayFrameNum,
                StartSessionTime = data.Telemetry.SessionTime - 1,
                EndSessionTime = data.Telemetry.SessionTime + 6
            };

            if( lastIncident == null )
            {
                lastIncident = i;
            }
            else if (lastIncident.CarIdx != i.CarIdx)
            {
                AddLastIncident(i);
            }
            else if(lastIncident.EndSessionTime + 15.0 < i.StartSessionTime)
            {
                AddLastIncident(i);
            }
            else
            {
                lastIncident.EndSessionTime = i.EndSessionTime;
            }

            Trace.WriteLine(" Noted incident on lap {0}, with driver {1}, at time of {2}".F(i.LapNumber, i.CarIdx, TimeSpan.FromSeconds(data.Telemetry.SessionTime)));
        }

        public void Stop()
        {
            if (lastIncident != null)
            {
                Trace.WriteLine("Noting incident for driver {0} starting on lap {1} from {2} to {3} ".F(
                    lastIncident.CarIdx, lastIncident.LapNumber,
                    TimeSpan.FromSeconds(lastIncident.StartSessionTime),
                    TimeSpan.FromSeconds(lastIncident.EndSessionTime)));
                incidents.Add(lastIncident);
            }
        }
        
        void AddLastIncident(Incident i)
        {
            Trace.WriteLine("Noting incident for driver {0} starting on lap {1} from {2} to {3} ".F(
                lastIncident.CarIdx, lastIncident.LapNumber,
                TimeSpan.FromSeconds(lastIncident.StartSessionTime),
                TimeSpan.FromSeconds(lastIncident.EndSessionTime)));
            incidents.Add(lastIncident);
            lastIncident = i;
        }

        public IEnumerator<Incidents.Incident> GetEnumerator()
        {
            return incidents.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return incidents.GetEnumerator();
        }
    }
}

