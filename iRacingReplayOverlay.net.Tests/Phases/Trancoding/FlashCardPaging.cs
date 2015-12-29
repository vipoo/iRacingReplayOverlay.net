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
using NUnit.Framework;
using System.Linq;

namespace iRacingReplayOverlay.net.Tests.Phases.Transcoding
{
    [TestFixture]
    public class FlashCardPagingTest
    {
        [Test]
        public void has_1_page_for_1_driver()
        {
            iRacingSDK.SessionData._DriverInfo driverInfo = BuildDrivers(1);
            var actual = FlashCardPaging.GetNumberOfPages(driverInfo);

            Assert.That(actual, Is.EqualTo(1));
        }

        [Test]
        public void has_1_page_for_10_drivers()
        {
            iRacingSDK.SessionData._DriverInfo driverInfo = BuildDrivers(10);
            var actual = FlashCardPaging.GetNumberOfPages(driverInfo);

            Assert.That(actual, Is.EqualTo(1));
        }

        [Test]
        public void has_2_page_for_15_drivers()
        {
            iRacingSDK.SessionData._DriverInfo driverInfo = BuildDrivers(15);
            var actual = FlashCardPaging.GetNumberOfPages(driverInfo);

            Assert.That(actual, Is.EqualTo(2));
        }

        [Test]
        public void show_page_1_at_start()
        {
            iRacingSDK.SessionData._DriverInfo driverInfo = BuildDrivers(10);
            var actual = FlashCardPaging.GetPageNumber(driverInfo, 0.0f);

            Assert.That(actual, Is.EqualTo(0));
        }

        [Test]
        public void show_page_2_at_end_for_20_drivers()
        {
            iRacingSDK.SessionData._DriverInfo driverInfo = BuildDrivers(20);
            var actual = FlashCardPaging.GetPageNumber(driverInfo, 1);

            Assert.That(actual, Is.EqualTo(1));
        }

        private static iRacingSDK.SessionData._DriverInfo BuildDrivers(int count)
        {
            var paceCar = new iRacingSDK.SessionData._DriverInfo._Drivers() { CarNumberRaw = 0 };

            var d = Enumerable.Range(0, count + 1).Select(i =>
                               new iRacingSDK.SessionData._DriverInfo._Drivers()
                               {
                                   CarIdx = i,
                                   CarNumberRaw = i 
                               }).ToArray();

            return new iRacingSDK.SessionData._DriverInfo
            {
                Drivers = d
            };
        }
    }
}
