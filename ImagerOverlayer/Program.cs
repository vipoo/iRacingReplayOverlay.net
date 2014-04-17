using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using iRacingReplayOverlay.net;

namespace ImagerOverlayer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var bitmap = (Bitmap)Bitmap.FromFile(@"c:\users\dean\documents\image.bmp");

			var g = Graphics.FromImage(bitmap);

			var leaderboard = new LeaderBoard();

			leaderboard.Overlay(g, 05000000);

			g.Flush();

			bitmap.Save(@"c:\users\dean\documents\newimage.bmp");
		}
	}
}
