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
using System;
using System.Collections.Generic;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Capturing.Tests
{
    [TestFixture]
    public class RemovalEditsTest
    {
        [Test]
        public void it_should_add_a_single_battle_event()
        {
            var ds = CreateSample(30, null);

            var raceEvents = new List<OverlayData.RaceEvent>();

            var re = new RemovalEdits(raceEvents);
            var em = new EditMarker(re, InterestState.Battle);

            em.Start(1);
            re.Process(ds, 10.Seconds());

            em.Stop(1);
            re.Process(ds, 15.Seconds());

            Assert.That(raceEvents.Count, Is.EqualTo(1));

            var e = raceEvents.First();
            Assert.That(e, Is.EqualTo(new OverlayData.RaceEvent {
                CarIdx = 1,
                StartTime = 10d,
                EndTime = 15d,
                Interest = InterestState.Battle
            }));
        }

        [Test]
        public void it_should_reset_a_new_battle()
        {
            var ds = CreateSample(30, null);

            var raceEvents = new List<OverlayData.RaceEvent>();

            var re = new RemovalEdits(raceEvents);
            var markerB = re.For(InterestState.Battle);
            var markerI = re.For(InterestState.Incident);

            markerB.Start(1);
            re.Process(ds, 10.Seconds());

            markerB.Start(2);
            re.Process(ds, 15.Seconds());

            markerB.Stop(2);
            re.Process(ds, 18.Seconds());

            markerI.Start(3);
            re.Process(ds, 20.Seconds());
            markerI.Stop(3);
            re.Process(ds, 22.Seconds());
            
            Assert.That(raceEvents.ToArray(), Is.EqualTo(
                new[] 
                {
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 10d, EndTime = 15d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = 2, StartTime = 15d, EndTime = 18d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = 3, StartTime = 20d, EndTime = 22d, Interest = InterestState.Incident},
                }));
        }

        [Test]
        public void it_should_reset_an_existing_battle()
        {
            var ds = CreateSample(30, null);

            var raceEvents = new List<OverlayData.RaceEvent>();

            var re = new RemovalEdits(raceEvents);
            var markerB = re.For(InterestState.Battle);
            var markerI = re.For(InterestState.Incident);

            markerB.Start(1);
            re.Process(ds, 10.Seconds());

            markerB.Start(1);
            re.Process(ds, 15.Seconds());

            markerB.Stop(1);
            re.Process(ds, 18.Seconds());

            markerI.Start(3);
            re.Process(ds, 20.Seconds());
            markerI.Stop(3);
            re.Process(ds, 22.Seconds());

            Assert.That(raceEvents.ToArray(), Is.EqualTo(
                new[] 
                {
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 10d, EndTime = 15d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 15d, EndTime = 18d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = 3, StartTime = 20d, EndTime = 22d, Interest = InterestState.Incident},
                }));
        }


        [Test]
        public void it_should_error_when_caridx_was_not_started()
        {
            var ds = CreateSample(30, null);

            var raceEvents = new List<OverlayData.RaceEvent>();

            var re = new RemovalEdits(raceEvents);
            var marker = re.For(InterestState.Incident);

            marker.Start(1);
            re.Process(ds, 10.Seconds());

            marker.Stop(2);

            Assert.Throws(typeof(Exception), () => re.Process(ds, 15.Seconds()));
        }

        [Test]
        public void it_should_add_inner_events()
        {
            var ds = CreateSample(30, null);

            var raceEvents = new List<OverlayData.RaceEvent>();

            var re = new RemovalEdits(raceEvents);
            var markerB = re.For(InterestState.Battle);
            var markerI = re.For(InterestState .Incident);
            
            markerB.Start(1);
            re.Process(ds, 10.Seconds());

            markerI.Start(2);
            re.Process(ds, 14.Seconds());

            markerI.Stop(2);
            re.Process(ds, 16.Seconds());

            markerB.Stop(1);
            re.Process(ds, 18.Seconds());

            Assert.That(raceEvents.ToArray(), Is.EqualTo(
                new[] 
                {
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 10d, EndTime = 14d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = 2, StartTime = 14d, EndTime = 16d, Interest = InterestState.Incident},
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 16d, EndTime = 18d, Interest = InterestState.Battle},
                }
                ));
        }

        [Test]
        public void it_should_a_sequence_of_inner_events()
        {
            var ds = CreateSample(30, null);

            var raceEvents = new List<OverlayData.RaceEvent>();

            var re = new RemovalEdits(raceEvents);
            var markerB = re.For(InterestState.Battle);
            var markerI = re.For(InterestState.Incident);

            markerB.Start(1);
            re.Process(ds, 10.Seconds());

            markerI.Start(2);
            re.Process(ds, 14.Seconds());
            markerI.Stop(2);
            re.Process(ds, 16.Seconds());

            markerI.Start(2);
            re.Process(ds, 18.Seconds());
            markerI.Stop(2);
            re.Process(ds, 20.Seconds());

            markerB.Stop(1);
            re.Process(ds, 22.Seconds());

            Assert.That(raceEvents.ToArray(), Is.EqualTo(
                new[] 
                {
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 10d, EndTime = 14d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = 2, StartTime = 14d, EndTime = 16d, Interest = InterestState.Incident},
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 16d, EndTime = 18d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = 2, StartTime = 18d, EndTime = 20d, Interest = InterestState.Incident},
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 20d, EndTime = 22d, Interest = InterestState.Battle},
                }));
        }

        [Test]
        public void it_should_add_nested_events()
        {
            var ds = CreateSample(30, null);

            var raceEvents = new List<OverlayData.RaceEvent>();

            var re = new RemovalEdits(raceEvents);
            var markerB = re.For(InterestState.Battle);
            var markerI = re.For(InterestState.Incident);
            var markerF = re.For(InterestState.FirstLap);

            markerF.Start(1);
            re.Process(ds, 10.Seconds());

            markerB.Start(2);
            re.Process(ds, 14.Seconds());

            markerI.Start(3);
            re.Process(ds, 16.Seconds());

            markerI.Stop(3);
            re.Process(ds, 18.Seconds());

            markerB.Stop(2);
            re.Process(ds, 20.Seconds());

            markerF.Stop(1);
            re.Process(ds, 22.Seconds());

            Assert.That(raceEvents.ToArray(), Is.EqualTo(
                new[] 
                {
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 10d, EndTime = 14d, Interest = InterestState.FirstLap},
                    new OverlayData.RaceEvent { CarIdx = 2, StartTime = 14d, EndTime = 16d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = 3, StartTime = 16d, EndTime = 18d, Interest = InterestState.Incident},
                    new OverlayData.RaceEvent { CarIdx = 2, StartTime = 18d, EndTime = 20d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 20d, EndTime = 22d, Interest = InterestState.FirstLap},
                }));
        }

        [Test]
        public void it_should_add_last_lap_event_when_event_active()
        {
            var ds = CreateSample(30, null);

            var raceEvents = new List<OverlayData.RaceEvent>();

            var re = new RemovalEdits(raceEvents);
            var markerB = re.For(InterestState.Battle);
            var markerL = re.For(InterestState.LastLap);
            
            markerB.Start(1);
            re.Process(ds, 10.Seconds());

            markerL.Start();
            re.Process(ds, 19.Seconds());
            
            re.Process(ds, 20.Seconds());
            re.Stop();

            Assert.That(raceEvents.ToArray(), Is.EqualTo(
                new[] 
                {
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 10d, EndTime = 19d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = -1, StartTime = 19d, EndTime = 20d, Interest = InterestState.LastLap},
                }));
        }

        public void it_should_add_last_lap_event_when_no_event_is_active()
        {
            var ds = CreateSample(30, null);

            var raceEvents = new List<OverlayData.RaceEvent>();

            var re = new RemovalEdits(raceEvents);
            var markerB = re.For(InterestState.Battle);
            var markerL = re.For(InterestState.LastLap);
            
            markerB.Start(1);
            re.Process(ds, 10.Seconds());

            markerB.Stop(2);
            re.Process(ds, 15.Seconds());

            markerL.Start();
            re.Process(ds, 19.Seconds());
            
            re.Process(ds, 20.Seconds());
            re.Stop();

            Assert.That(raceEvents.ToArray(), Is.EqualTo(
                new[] 
                {
                    new OverlayData.RaceEvent { CarIdx = 1, StartTime = 10d, EndTime = 15d, Interest = InterestState.Battle},
                    new OverlayData.RaceEvent { CarIdx = -1, StartTime = 19d, EndTime = 20d, Interest = InterestState.LastLap},
                }));
        }

        static DataSample CreateSample(double time, SessionData sessionData)
        {
            var data = new DataSample
            {
                SessionData = sessionData,
                IsConnected = true,
                Telemetry = new Telemetry
                        {
                            { "SessionTime", time },
                        }
            };

            data.Telemetry.SessionData = sessionData;
            return data;
        }
    }
}
