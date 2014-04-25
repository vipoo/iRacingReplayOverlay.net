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

using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Phases.Transcoding;
using System.Collections.Generic;
using System.Drawing;

namespace ImagerOverlayer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
            var driverNickNames = new Dictionary<string, string>();
			var bitmap = (Bitmap)Bitmap.FromFile(@"c:\users\dean\documents\image.bmp");

			var g = Graphics.FromImage(bitmap);

            var leaderboard = new LeaderBoard
            {
                TimingSamples = new TimingSamples {
                    new TimingSample
                    {
                        StartTime = 0, 
                        Drivers = new [] {
                            new TimingSample.Driver { CarNumber = 12, Name = "Dean Netherton", Position = 1, DriverNickNames = driverNickNames },
                            new TimingSample.Driver { CarNumber = 13, Name = "Matty", Position = 2, DriverNickNames = driverNickNames },
                            new TimingSample.Driver { CarNumber = 3, Name = "Fred", Position = 3, DriverNickNames = driverNickNames }
                        },
                        RacePosition = "Lap 12/40",
                        CurrentDriver = new TimingSample.Driver { Position=  13, Indicator = "th", CarNumber = 29, Name = "Somebody" },
                    }
                }
            };

			leaderboard.Overlay(g, 05000000);

			g.Flush();

			bitmap.Save(@"c:\users\dean\documents\newimage.bmp");
		}
	}
}
