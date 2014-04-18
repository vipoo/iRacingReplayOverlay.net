﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using iRacingReplayOverlay.net;
using System.Collections.Generic;

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
                TimingSamples = new [] {
                    new TimingSample
                    {
                        StartTime = 0, 
                        Drivers = new [] { "Dean Netherton", "Matty", "Fred" }, 
                        RacePosition = "Lap 12/40",
                        CurrentDriver =new [] { "3", "th", "29","Somebody" },
                        DriverNickNames = driverNickNames
                    }
                }
            };

			leaderboard.Overlay(g, 05000000);

			g.Flush();

			bitmap.Save(@"c:\users\dean\documents\newimage.bmp");
		}
	}
}
