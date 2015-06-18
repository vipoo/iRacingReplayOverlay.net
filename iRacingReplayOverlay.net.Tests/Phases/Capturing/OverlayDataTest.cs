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
    public class OverlayDataTest
    {
        [Test]
        public void first_letter_of_first_name_followed_by_first_four_letters_of_last_name()
        {
            var od = new iRacingReplayOverlay.Phases.Capturing.OverlayData.Driver {
                UserName = "First Last"
            };

            Assert.That(od.ShortName, Is.EqualTo("FLast"));
        }

        [Test]
        public void upper_case_first_initials()
        {
            var od = new iRacingReplayOverlay.Phases.Capturing.OverlayData.Driver
            {
                UserName = "first last"
            };

            Assert.That(od.ShortName, Is.EqualTo("FLast"));
        }

        [Test]
        public void full_user_name_when_less_than_four_chars_long()
        {
            var od = new iRacingReplayOverlay.Phases.Capturing.OverlayData.Driver
            {
                UserName = "Abc"
            };

            Assert.That(od.ShortName, Is.EqualTo("Abc"));
        }


        [Test]
        public void ignore_middle_names()
        {
            var od = new iRacingReplayOverlay.Phases.Capturing.OverlayData.Driver
            {
                UserName = "first middle last"
            };

            Assert.That(od.ShortName, Is.EqualTo("FLast"));
        }

        [Test]
        public void deal_with_short_last_names()
        {
            var od = new iRacingReplayOverlay.Phases.Capturing.OverlayData.Driver
            {
                UserName = "first middle lt"
            };

            Assert.That(od.ShortName, Is.EqualTo("FLt"));
        }
    }
}
