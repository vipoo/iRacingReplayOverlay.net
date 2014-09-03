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

            var session = CreateDrivers("dino");

            var data = CreateIncidentSample(0, 4.3d, session);

            i.Process(data);

            Assert.That(i.Count(), Is.EqualTo(1));

            var actual = i.First();

            Assert.That(actual.LapNumber, Is.EqualTo(3));
            Assert.That(actual.StartSessionTime, Is.EqualTo(3.3.Seconds()));
            Assert.That(actual.EndSessionTime, Is.EqualTo(12.3.Seconds()));
            Assert.That(actual.Car.UserName, Is.EqualTo("dino"));
        }

        [Test]
        public void it_should_merge_two_incidents_for_same_driver()
        {
            var i = new Incidents();

            var session = CreateDrivers("dino");

            i.Process(CreateIncidentSample(0, 4.3d, session));
            i.Process(CreateIncidentSample(0, 5.2d, session));
             
            Assert.That(i.Count(), Is.EqualTo(1));

            var actual = i.First();

            Assert.That(actual.StartSessionTime, Is.EqualTo(3.3.Seconds()));
            Assert.That(actual.EndSessionTime, Is.EqualTo(13.2.Seconds()));
        }

        public class SpikeTest
        {
            private Incidents i;

            [SetUp]
            public void setup()
            {
                i = new Incidents();

                var session = CreateDrivers("dino");

                i.Process(CreateIncidentSample(0, 4.0d, session));
                i.Process(CreateIncidentSample(0, (4d + 8d + 17d), session));
            }

            [Test]
            public void it_should_pass()
            {
                Assert.That(i.Count(), Is.EqualTo(2));
            }
        }
        [Test]
        public void it_should_not_merge_two_incidents_when_more_than_15_seconds_apart()
        {
            var i = new Incidents();

            var session = CreateDrivers("dino");

            i.Process(CreateIncidentSample(0, 4.0d, session));
            i.Process(CreateIncidentSample(0, (4d + 8d + 17d), session));

            Assert.That(i.Count(), Is.EqualTo(2));

            var actual = i.First();

            Assert.That(actual.StartSessionTime, Is.EqualTo(3.Seconds()));
            Assert.That(actual.EndSessionTime, Is.EqualTo(12.Seconds()));

            actual = i.Skip(1).First();

            Assert.That(actual.StartSessionTime, Is.EqualTo(28.Seconds()));
            Assert.That(actual.EndSessionTime, Is.EqualTo(37.Seconds()));
        }

        [Test]
        public void it_should_merge_two_incidents_across_another_drivers_incident()
        {
            var i = new Incidents();

            var session = CreateDrivers("dino", "georg");

            i.Process(CreateIncidentSample(0, 4.3d, session));
            i.Process(CreateIncidentSample(1, 5.4d, session));
            i.Process(CreateIncidentSample(0, 6.2d, session));

            Assert.That(i.Count(), Is.EqualTo(2));

            var actual = i.First();

            Assert.That(actual.StartSessionTime, Is.EqualTo(3.3.Seconds()));
            Assert.That(actual.EndSessionTime, Is.EqualTo(14.2.Seconds()));
            Assert.That(actual.Car.UserName, Is.EqualTo("dino"));

            actual = i.Skip(1).First();

            Assert.That(actual.StartSessionTime, Is.EqualTo(4.4.Seconds()));
            Assert.That(actual.EndSessionTime, Is.EqualTo(13.4.Seconds()));
            Assert.That(actual.Car.UserName, Is.EqualTo("georg"));
        }

        static SessionData CreateDrivers(params string[] names)
        {
            return new SessionData
                {
                    DriverInfo = new SessionData._DriverInfo
                    {
                        Drivers = names.Select(n => new SessionData._DriverInfo._Drivers { UserName = n }).ToArray()
                    }
                };
        }

        static DataSample CreateIncidentSample(int carIdx, double time, SessionData sessionData)
        {

            var data = new DataSample
            {
                SessionData = sessionData,
                IsConnected = true,
                Telemetry = new Telemetry
                        {
                            { "SessionTime", time },
                            { "RaceLaps", 3 },
                            { "CarIdxTrackSurface", new [] { TrackLocation.OnTrack, TrackLocation.OnTrack, TrackLocation.OnTrack, TrackLocation.OnTrack } },
                            { "CamCarIdx", carIdx }
                        }
            };

            data.Telemetry.SessionData = sessionData;
            return data;
        }
    }
}
