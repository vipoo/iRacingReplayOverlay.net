using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace iRacingReplayOverlay.net
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

            for (int i = 0; i < sample.ShortNames.Length; i++)
            {
                r = r.ToBelow(width: 40);

                r.With(simpleWhiteBox)
                    .DrawText(i.ToString())
                    .ToRight(width: 120)
                    .With(simpleWhiteBox)
                    .WithStringFormat(StringAlignment.Near)
                    .DrawText(sample.ShortNames[i], 10);
            }
        }

		private void DrawCurrentDriverRow(Graphics g, string[] p)
        {
			g.InRectangle(1920 / 2 - 150, 980, 70, 40)
				.WithBrush(Styles.Brushes.Yellow)
				.WithPen(Styles.Pens.Black)
				.DrawRectangleWithBorder();

            


			/*
            var left = 1920 / 2 - 150;
            var rect = new Rectangle(left, 980, 70, 40);
            DrawBoxWithText(graphics, rect, p[0], StringAlignment.Center, 0, Color.Yellow, Color.Yellow);
 70, 40
            DrawWhiteBox(graphics, rect, Color.Yellow, Color.Yellow);

            var rect2 = new Rectangle(rect.Left + 1, rect.Top + 2, rect.Width, rect.Height);
            graphics.DrawString(p[0], Styles.Fonts.LeaderBoard, Styles.Brushes.Black, rect2, stringLeftFormat);
            var size = graphics.MeasureString(p[0], Styles.Fonts.LeaderBoard);

            var font = new Font("Calibri", 16, FontStyle.Bold);

            var width = (int)size.Width - 7;
            var rect3 = new Rectangle(rect2.Left + width, rect2.Top, rect.Width - width, rect.Height);
            graphics.DrawString(p[1], font, Styles.Brushes.Black, rect3);


            var rect4 = new Rectangle(rect3.Left + rect3.Width, rect.Top, 60, rect.Height);
            DrawWhiteBox(graphics, rect4);
            
            graphics.DrawString(p[2], Styles.Fonts.LeaderBoard, Styles.Brushes.Black, rect4, stringCenterFormat);
*/

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
