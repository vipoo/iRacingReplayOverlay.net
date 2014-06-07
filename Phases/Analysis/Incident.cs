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
using iRacingSDK.Support;
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
            public TimeSpan StartSessionTime;
            public TimeSpan EndSessionTime;
            public Car Car;

            public bool IsInside(TimeSpan time)
            {
                return time >= StartSessionTime && time <= EndSessionTime;
            }
        }

        List<Incident> incidents = new List<Incident>();
        Incident lastIncident;

        public void Process(DataSample data)
        {
            if (data.Telemetry.CamCar.TrackSurface == TrackLocation.InPitStall ||
                data.Telemetry.CamCar.TrackSurface == TrackLocation.NotInWorld ||
                data.Telemetry.CamCar.TrackSurface == TrackLocation.AproachingPits)
            {
                Trace.WriteLine("{0} Ignoring incident in the pits on lap {1}".F(data.Telemetry.SessionTimeSpan, data.Telemetry.RaceLaps));
                return;
            }

            var i = new Incident 
            {
                LapNumber = data.Telemetry.RaceLaps, 
                Car = data.Telemetry.CamCar, 
                StartSessionTime = data.Telemetry.SessionTimeSpan - 1.Seconds(),
                EndSessionTime = data.Telemetry.SessionTimeSpan + 8.Seconds()
            };

            if( lastIncident == null )
                lastIncident = i;
           
            else if (lastIncident.Car.CarIdx != i.Car.CarIdx)
                AddLastIncident(i);
            
            else if(lastIncident.EndSessionTime + 15.Seconds() < i.StartSessionTime)
                AddLastIncident(i);
            
            else
                lastIncident.EndSessionTime = i.EndSessionTime;
        }

        public void Stop()
        {
            if (lastIncident != null)
            {
                Trace.WriteLine("Noting incident for driver {0} starting on lap {1} from {2} to {3} ".F(
                    lastIncident.Car.UserName, lastIncident.LapNumber,
                    lastIncident.StartSessionTime,
                    lastIncident.EndSessionTime), "INFO");
                incidents.Add(lastIncident);
            }
        }
        
        void AddLastIncident(Incident i)
        {
            Trace.WriteLine("Noting incident for driver {0} starting on lap {1} from {2} to {3} ".F(
                lastIncident.Car.UserName, lastIncident.LapNumber,
                lastIncident.StartSessionTime,
                lastIncident.EndSessionTime), "INFO");
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

