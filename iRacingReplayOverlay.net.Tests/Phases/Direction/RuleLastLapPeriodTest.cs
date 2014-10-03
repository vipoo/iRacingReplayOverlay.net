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
using iRacingReplayOverlay.Phases.Direction;
using iRacingSDK;
using iRacingSDK.Support;
using NUnit.Framework;

namespace iRacingReplayOverlay.net.Tests.Phases.Direction
{
    [TestFixture]
    public class RuleLastLapPeriodTest
    {
        SessionData session;
        RuleLastLapPeriod ruleLastLapPeriod;

        [SetUp]
        public void setup()
        {
            Settings.Default.FollowLeaderBeforeRaceEndPeriod = 10.Seconds();

            session = BuildSession(resultsLapsCompleted: 12, resultsAverageLapTime: 30d);

            var cameras = new[] { new TrackCamera { CameraName = "TV2" } };

            ruleLastLapPeriod = new RuleLastLapPeriod(cameras, new Moq.Mock<RemovalEdits>(null).Object);
        }

        [Test]
        public void it_should_return_inactive_when_before_last_lap()
        {
            var data = BuildDataSample(session: session, raceLaps: 10);

            Assert.That(ruleLastLapPeriod.IsActive(data), Is.False);
        }

        [Test]
        public void it_should_return_inactive_when_just_starting_last_lap()
        {
            var data = BuildDataSample(session: session, raceLaps: 12, sessionTime: 100.0d);

            Assert.That(ruleLastLapPeriod.IsActive(data), Is.False);
        }

        [Test]
        public void it_should_return_active_before_leader_crosses_line()
        {

            var data = BuildDataSample(session: session, raceLaps: 12, sessionTime: 100.0d);
            ruleLastLapPeriod.IsActive(data);

            data = BuildDataSample(session: session, raceLaps: 12, sessionTime: 119.9d);
            Assert.That(ruleLastLapPeriod.IsActive(data), Is.False);

            data = BuildDataSample(session: session, raceLaps: 12, sessionTime: 120.4d);
            Assert.That(ruleLastLapPeriod.IsActive(data), Is.True);
        }

        static DataSample BuildDataSample(int raceLaps, SessionData session, double sessionTime = 0)
        {
            var data = new DataSample
            {
                SessionData = session,
                IsConnected = true,
                Telemetry = new Telemetry
                        {
                            { "SessionTime", sessionTime },
                            { "SessionNum", 0 },
                            { "RaceLaps", raceLaps },
                        }
            };

            data.Telemetry.SessionData = session;
            return data;
        }

        private static SessionData BuildSession(int resultsLapsCompleted, double resultsAverageLapTime = 0)
        {
            var session = new SessionData
            {
                SessionInfo = new SessionData._SessionInfo
                {
                    Sessions = new[] {
                        new SessionData._SessionInfo._Sessions {
                            ResultsAverageLapTime = resultsAverageLapTime,
                            ResultsLapsComplete = resultsLapsCompleted
                        }
                    }
                }
            };
            return session;
        }
    }
}
