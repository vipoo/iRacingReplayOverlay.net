using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

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

            DrawLeaderboardHeading(graphics, sample.RacePosition);

            for (int i = 0; i < sample.ShortNames.Length; i++)
                DrawRow(graphics, i + 1, sample.ShortNames[i]);
        }


        private void DrawLeaderboardHeading(Graphics graphics, string p)
        {
            var rect = new Rectangle(80, 120 - 40, 160, 40);
            DrawBoxWithText(graphics, rect, p);
        }


        void DrawRow(Graphics g, int position, string name)
        {
            var y = (position - 1) * 40 + 120;
            var rect = new Rectangle(80, y, 40, 40);
            DrawBoxWithText(g, rect, position.ToString());

            rect = new Rectangle(120, y, 120, 40);
            DrawBoxWithText(g, rect, name, StringAlignment.Near, 10);
        }

        void DrawBoxWithText(Graphics g, Rectangle rect, string text, StringAlignment alignment = StringAlignment.Center, int leftOffset = 0)
        {
            DrawWhiteBox(g, rect);
            DrawTextInBox(g, rect, text, alignment, leftOffset);
        }

        void DrawWhiteBox(Graphics g, Rectangle rect)
        {
            var brush = new LinearGradientBrush(rect, Styles.WhiteSmoke, Styles.White, LinearGradientMode.Horizontal);

            g.FillRectangle(brush, rect);
            g.DrawRectangle(Styles.Pens.Black, rect);
        }

        void DrawTextInBox(Graphics g, Rectangle rect, string text, StringAlignment alignment = StringAlignment.Center, int leftOffset = 0)
        {
            var stringFormat = new StringFormat()
            {
                Alignment = alignment,
                LineAlignment = StringAlignment.Center
            };
            var rect2 = new Rectangle(rect.Left + leftOffset + 1, rect.Top + 2, rect.Width, rect.Height);
            g.DrawString(text, Styles.Fonts.LeaderBoard, Styles.Brushes.Black, rect2, stringFormat);
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
    }
}
