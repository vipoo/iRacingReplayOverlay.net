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
using iRacingReplayOverlay.Drawing;
using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.net;

namespace iRacingReplayOverlay.Phases.Transcoding
{
    class LeaderBoard
    {
        public TimingSample[] TimingSamples;

        internal void Overlay(Graphics graphics, long timestamp)
        {
            timestamp = timestamp.FromNanoToSeconds();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var sample = TimingSamples.LastOrDefault(s => s.StartTime <= timestamp);

            if (sample == null)
                return;

			DrawLeaderboard(graphics, sample);

            DrawCurrentDriverRow(graphics, sample.CurrentDriver);
        }

		private void DrawLeaderboard(Graphics g, TimingSample sample)
        {
            Func<GraphicRect, GraphicRect> simpleWhiteBox = rr =>
                rr.WithLinearGradientBrush(Styles.WhiteSmoke, Styles.White, LinearGradientMode.Horizontal)
                .WithPen(Styles.Pens.Black)
                .DrawRectangleWithBorder()
                .WithBrush(Styles.Brushes.Black)
                .WithFont("Calibri", 24, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Center);

            var r = g.InRectangle(80, 80, 160, 40)
                .With(simpleWhiteBox)
                .DrawText(sample.RacePosition.ToString());

            for (int i = 0; i < Math.Min(sample.ShortNames.Length, 20); i++)
            {
                r = r.ToBelow(width: 40);

                r.With(simpleWhiteBox)
                    .DrawText((i+1).ToString())
                    .ToRight(width: 120)
                    .With(simpleWhiteBox)
                    .WithStringFormat(StringAlignment.Near)
                    .DrawText(sample.ShortNames[i], 10);
            }
        }

		private void DrawCurrentDriverRow(Graphics g, TimingSample._CurrentDriver p)
        {
            g.InRectangle(1920/2-420/2, 980, 70, 40)
                .WithBrush(Styles.Brushes.Yellow)
                .WithPen(Styles.Pens.Black)
                .DrawRectangleWithBorder()
                .WithFont("Calibri", 24, FontStyle.Bold)
                .WithBrush(Styles.Brushes.Black)
                .WithStringFormat(StringAlignment.Near)
                .Center(cg => cg
                            .DrawText(p.Position)
                            .AfterText(p.Position)
                            .MoveRight(3)
                            .WithFont("Calibri", 18, FontStyle.Bold)
                            .DrawText(p.Indicator)
                )

                .ToRight(50)
                .WithLinearGradientBrush(Styles.White, Styles.WhiteSmoke, LinearGradientMode.Horizontal)
                .DrawRectangleWithBorder()
                .WithStringFormat(StringAlignment.Center)
                .WithBrush(Styles.Brushes.Black)
                .DrawText(p.CarNumber)

                .ToRight(300)
                .WithLinearGradientBrush(Styles.White, Styles.WhiteSmoke, LinearGradientMode.Horizontal)
                .DrawRectangleWithBorder()
                .WithStringFormat(StringAlignment.Center)
                .WithBrush(Styles.Brushes.Black)
                .DrawText(p.Name);
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
				public readonly static Brush Yellow = new SolidBrush(Color.Yellow);

            }
        }
    }
}
