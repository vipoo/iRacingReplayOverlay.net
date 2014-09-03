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
using iRacingSDK.Support;
using NUnit.Framework;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Analysis.Tests
{
    [TestFixture]
    public class Incident
    {
        [Test]
        public void it_should_identify_an_incident()
        {
            var i = new Incidents();

            var data = CreateIncidentSample("Dino", 4.3d);

            i.Process(data);

            i.Stop();

            Assert.That(i.Count(), Is.EqualTo(1));

            var actual = i.First();

            Assert.That(actual.LapNumber, Is.EqualTo(3));
            Assert.That(actual.StartSessionTime, Is.EqualTo(3.3.Seconds()));
            Assert.That(actual.EndSessionTime, Is.EqualTo(12.3.Seconds()));
            Assert.That(actual.Car.UserName, Is.EqualTo("Dino"));
        }

        [Test]
        public void it_should_merge_two_incidents_for_same_driver()
        {
            var i = new Incidents();

            i.Process(CreateIncidentSample("Dino", 4.3d));
            i.Process(CreateIncidentSample("Dino", 5.2d));
             
            i.Stop();

            Assert.That(i.Count(), Is.EqualTo(1));

            var actual = i.First();

            Assert.That(actual.StartSessionTime, Is.EqualTo(3.3.Seconds()));
            Assert.That(actual.EndSessionTime, Is.EqualTo(13.2.Seconds()));
        }

        static DataSample CreateIncidentSample(string driverName, double time)
        {

            var data = new DataSample
            {
                SessionData = new SessionData
                {
                    DriverInfo = new SessionData._DriverInfo
                    {
                        Drivers = new[] { 
                                new SessionData._DriverInfo._Drivers { UserName = driverName }
                            }
                    }
                },
                IsConnected = true,
                Telemetry = new Telemetry
                        {
                            { "SessionTime", time },
                            { "RaceLaps", 3 },
                            { "CarIdxTrackSurface", new [] { TrackLocation.OnTrack } },
                            { "CamCarIdx", 0 }
                        }
            };

            data.Telemetry.SessionData = data.SessionData;
            return data;
        }
    }
}
