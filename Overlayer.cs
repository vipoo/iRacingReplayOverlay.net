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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace iRacingReplayOverlay.net
{
	public class TimingSample
	{
		public int StartTime;
		public int Duration;
		public string[] Drivers;
	}

	public static class Overlayer
	{
		public static TimingSample[] DataSet = new TimingSample[]
		{
			new TimingSample { StartTime = 0, Duration = 1, Drivers = new string[] { "DINO", "MATTB", "BLAH", "dfdf", "dfdf", "dfdf" } },
			new TimingSample { StartTime = 3, Duration = 2, Drivers = new string[] { "XYZ", "ABC", "DEF" } }
		};

		public static void Leaderboard(long timestamp, Graphics g)
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			//g.CompositingMode = CompositingMode.SourceCopy;

			timestamp = timestamp / 10000000;

			Console.WriteLine("Timestamp is " + timestamp);

			var sample = DataSet.FirstOrDefault( s => s.StartTime <= timestamp && (s.StartTime + s.Duration) >= timestamp );

			if(sample == null)
				return;

			for(int i = 0; i < sample.Drivers.Length; i++)
				DrawRow(g, i + 1, sample.Drivers[i]);
		}

		public static class Styles
		{
			public const int AlphaLevel = 120;
			public readonly static Color White = Color.FromArgb(AlphaLevel, Color.White);
			public readonly static Color WhiteSmoke = Color.FromArgb(AlphaLevel, Color.WhiteSmoke);
			public readonly static Color Black = Color.FromArgb(AlphaLevel, Color.Black);

			public static class Pens
			{
				public readonly static Pen Black = new Pen(Styles.Black);
			}

			public static class Fonts
			{
				public readonly static Font LeaderBoard = new Font("Calibri", 24, FontStyle.Bold);
			}

			public static class Brushes
			{
				public readonly static Brush Black = new SolidBrush(Color.Black);
			}
		}

		public static void DrawRow(Graphics g, int position, string name)
		{
			var y = (position - 1) * 40 + 120;
			var rect = new Rectangle(80, y, 40, 40);
			DrawBoxWithText(g, rect, position.ToString());

			rect = new Rectangle(120, y, 120, 40);
			DrawBoxWithText(g, rect, name, StringAlignment.Near, 10);
		}

		static void DrawBoxWithText(Graphics g, Rectangle rect, string text, StringAlignment alignment = StringAlignment.Center, int leftOffset = 0)
		{
			DrawWhiteBox(g, rect);
			DrawTextInBox(g, rect, text, alignment, leftOffset);
		}

		static void DrawWhiteBox(Graphics g, Rectangle rect)
		{
			var brush = new LinearGradientBrush(rect, Styles.WhiteSmoke, Styles.White, LinearGradientMode.Horizontal);

			g.FillRectangle(brush, rect);
			g.DrawRectangle(Styles.Pens.Black, rect);
		}

		static void DrawTextInBox(Graphics g, Rectangle rect, string text, StringAlignment alignment = StringAlignment.Center, int leftOffset = 0)
		{
			var stringFormat = new StringFormat() {
				Alignment = alignment,
				LineAlignment = StringAlignment.Center
			};
			var rect2 = new Rectangle(rect.Left + leftOffset + 1, rect.Top + 2, rect.Width, rect.Height);
			g.DrawString(text, Styles.Fonts.LeaderBoard, Styles.Brushes.Black, rect2, stringFormat);
		}
	}
}

