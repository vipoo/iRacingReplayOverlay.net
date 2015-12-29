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

using System;

namespace iRacingReplayOverlay.Phases.Transcoding
{
    public static class FlashCardPaging
    {
        public static int GetNumberOfPages(iRacingSDK.SessionData._DriverInfo driverInfo)
        {
            var numberOfDrivers = driverInfo.CompetingDrivers.Length - 1;
            var numberOfPages = Math.Min(numberOfDrivers / LeaderBoard.DriversPerPage, 3);
            if (((float)numberOfDrivers % LeaderBoard.DriversPerPage) != 0)
                numberOfPages++;

            return numberOfPages;
        }

        public static int GetPageNumber(iRacingSDK.SessionData._DriverInfo driverInfo, float pagePeriod)
        {
            var numberOfPages = GetNumberOfPages(driverInfo);
            var page = (int)Math.Floor(pagePeriod * numberOfPages);
            return Math.Min(page, numberOfPages - 1);
        }
    }
}
