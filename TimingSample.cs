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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace iRacingReplayOverlay.net
{
	public class TimingSample
	{
        public Dictionary<string, string> DriverNickNames = new Dictionary<string,string> ();

		public long StartTime;
		public string[] Drivers;

        public string[] ShortNames
        {
            get
            {
                return Drivers
                    .Select(d => GetDriversShortName(d))
                    .ToArray();
            }
        }

        string GetDriversShortName(string driversName)
        {
            if (DriverNickNames.ContainsKey(driversName))
                return DriverNickNames[driversName];

            var names = driversName.Split(' ');
            var name = names.First().Substring(0, 1).ToUpper()
                + names.Last().Substring(0, 1).ToUpper()
                + names.Last().Substring(1, 3);
            
            return name;
        }
	}
	
}
