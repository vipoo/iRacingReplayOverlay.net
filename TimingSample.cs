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
using System.IO;
using System.Linq;

namespace iRacingReplayOverlay.net
{
	public class TimingSample
	{
        public class _CurrentDriver
        {
            public string Position;
            public string Indicator;
            public string CarNumber;
            public string Name;
        }
        public static TimingSample[] FromFile(string filename, Dictionary<string,string> driverNickNames)
        {
            return File
                .ReadAllLines(filename)
                .Skip(1)
                .Select(line => line.Split(','))
                .Select(line => new TimingSample
                {
                    StartTime = long.Parse(line[0]),
                    Drivers = line[1].Split('|'),
                    RacePosition = line[2],
                    CurrentDriver = new _CurrentDriver() {
                        Position = line[3],
                        Indicator = line[4],
                        CarNumber = line[5],
                        Name = line[6]
                    },
                    DriverNickNames = driverNickNames
                })
                .ToArray();
        }
        public static void WriteCSVHeader(StreamWriter file)
        {
            file.WriteLine("StartTime, Drivers, DriverPosition, DriverIndicator, DriverCarNumber, DriverName");
        }

        public void WriteCSVRow(StreamWriter file)
        {
            file.Write(StartTime);
            file.Write(',');
            file.Write(String.Join("|", Drivers));
            file.Write(',');
            file.Write(RacePosition);
            file.Write(',');
            file.Write(CurrentDriver.Position);
            file.Write(',');
            file.Write(CurrentDriver.Indicator);
            file.Write(',');
            file.Write(CurrentDriver.CarNumber);
            file.Write(',');
            file.Write(CurrentDriver.Name);
            file.WriteLine();
        }

        public Dictionary<string, string> DriverNickNames;

		public long StartTime;
		public string[] Drivers;
        string[] shortNames;
        public string RacePosition;
        
        public _CurrentDriver CurrentDriver = new _CurrentDriver();

        public string[] ShortNames
        {
            get
            {
                if(shortNames == null)
                    shortNames = Drivers
                        .Select(d => GetDriversShortName(d))
                        .ToArray();

                return shortNames;
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

            DriverNickNames[driversName] = name;
            return name;
        }
	}
}
