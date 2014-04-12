using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImagerOverlayer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var bitmap = (Bitmap)Bitmap.FromFile(@"c:\users\dean\documents\image.bmp");

			var g = Graphics.FromImage(bitmap);

			iRacingReplayOverlay.net.Overlayer.Leaderboard(05000000, g);

			g.Flush();

			bitmap.Save(@"c:\users\dean\documents\newimage.bmp");
		}
	}
}
