using NUnit.Framework;
using iRacingSDK.Support;
using System;
using System.Linq;
using iRacingSDK;
using iRacingReplayOverlay.Support;
using NUnit.Framework.Constraints;

namespace iRacingReplayOverlay.Phases.Direction.Support.Tests
{
	[TestFixture]
	public class BattleTest
	{
        const float PaceCar = 0f;

		[Test]
		public void ShouldIdentifyASingleBattle()
		{
            var data = BuildDataSample();
            data.Telemetry.CarIdxDistance = new float[] { PaceCar, 0.14f, 0.125f, 0.12f, 0.10f };
            var expected = new[] { new Battle.GapMetric { CarIdx = 3, Position = 2, Time = (0.125f - 0.12f) * 100d } };
            
            var all = Battle.All(data, 1.Seconds()).ToArray();

            Assert.That(all, Is.EqualTo(expected));
		}

        [Test]
        public void ShouldIdentifyTwoBattles()
        {
            var data = BuildDataSample();
            data.Telemetry.CarIdxDistance = new float[] { PaceCar, 0.147f, 0.14f, 0.125f, 0.12f, 0.10f };
            var expected = new[] { 
                new Battle.GapMetric { CarIdx = 4, Position = 3, Time = (0.125f - 0.12f) * 100d },
                new Battle.GapMetric { CarIdx = 2, Position = 1, Time = (0.147f - 0.14f) * 100d },
            };

            var all = Battle.All(data, 1.Seconds()).ToArray();

            Assert.That(all, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldReturnEmptySetIfNoBattles()
        {
            var data = BuildDataSample();
            data.Telemetry.CarIdxDistance = new float[] { PaceCar, 0.15f, 0.12f, 0.10f, 0.08f, 0.04f };

            var all = Battle.All(data, 1.Seconds()).ToArray();

            Assert.That(all.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ShouldSelectOneOfTwoBattles()
        {
            var data = BuildDataSample();

            var all = new[] { 
                new Battle.GapMetric { CarIdx = 0, Position = 3 },
                new Battle.GapMetric { CarIdx = 1, Position = 1 },
            };

            AssertThatFor(0, 4, i => Battle.SelectABattle(data, all, i, 1.5d), Is.Null);
            AssertThatFor(5, 24, i => Battle.SelectABattle(data, all, i, 1.5d).UserName, Is.EqualTo("Driver1"));
            AssertThatFor(25, 99, i => Battle.SelectABattle(data, all, i, 1.5d).UserName, Is.EqualTo("Driver2"));
        }

        [Test]
        public void ShouldSelectOneOfThreeBattles()
        {
            var data = BuildDataSample();

            var all = new[] { 
                new Battle.GapMetric { CarIdx = 2, Position = 3 },
                new Battle.GapMetric { CarIdx = 1, Position = 1 },
                new Battle.GapMetric { CarIdx = 0, Position = 1 },
            };

            AssertThatFor(0, 4, i => Battle.SelectABattle(data, all, i, 1.5d), Is.Null);
            AssertThatFor(5, 10, i => Battle.SelectABattle(data, all, i, 1.5d).UserName, Is.EqualTo("Driver3"));
            AssertThatFor(11, 34, i => Battle.SelectABattle(data, all, i, 1.5d).UserName, Is.EqualTo("Driver2"));
            AssertThatFor(35, 99, i => Battle.SelectABattle(data, all, i, 1.5d).UserName, Is.EqualTo("Driver1"));
        }

        static void AssertThatFor<T>(int from, int to, Func<int, T> driver, Constraint constraint)
        {
            for (var i = from; i <= to; i++)
                Assert.That(driver(i), constraint, "Expected {0} for dice of {1}".F(constraint.ToString(), i));
        }

        static DataSample BuildDataSample()
        {
            var data = new DataSample
            {
                IsConnected = true,
                Telemetry = new Telemetry
                {
                    { "SessionNum", 0 },
                    { "SessionTime", 1d }
                },
                SessionData = new SessionData
                {
                    SessionInfo = new SessionData._SessionInfo
                    {
                        Sessions = new[]
                        {
                            new iRacingSDK.SessionData._SessionInfo._Sessions 
                            {
                                ResultsAverageLapTime = 100d
                            }
                        }
                    }
                }
            };

            data.SessionData.DriverInfo = new SessionData._DriverInfo
            {
                Drivers = new[]
                { 
                    new SessionData._DriverInfo._Drivers { UserName = "Driver1"},
                    new SessionData._DriverInfo._Drivers { UserName = "Driver2"},
                    new SessionData._DriverInfo._Drivers { UserName = "Driver3"},
                }
            };

            data.Telemetry.SessionData = data.SessionData;
            return data;
        }
	}
}

