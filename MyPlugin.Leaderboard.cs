using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace iRacingDirector.Plugin.StandardOverlays
{
    public partial class MyPlugin
    {
        public LeaderBoard LeaderBoard;

        Func<GraphicRect, GraphicRect> SimpleWhiteBox(int fontSize = 20)
        {
            return rr => rr.WithLinearGradientBrush(Styles.WhiteSmoke, Styles.White, LinearGradientMode.BackwardDiagonal)
            .WithPen(Styles.BlackPen)
            .DrawRectangleWithBorder()
            .WithBrush(Styles.BlackBrush)
            .WithFont(Settings.FontName, fontSize, FontStyle.Bold)
            .WithStringFormat(StringAlignment.Center);
        }

        public Func<GraphicRect, GraphicRect> ColourBox(Color color, int fontSize = 20)
        {
            return rr =>
                rr.WithBrush(new SolidBrush(color))
                    .WithPen(Styles.BlackPen)
                    .DrawRectangleWithBorder()
                    .WithFont(Settings.FontName, fontSize, FontStyle.Bold)
                    .WithBrush(Styles.BlackBrush)
                    .WithStringFormat(StringAlignment.Center);
        }

        void DrawLeaderboard(TimeSpan timeInSeconds)
        {
            var maxRows = LeaderBoard.LapCounter == null ? 22 : 21;
            var showPitStopCount = timeInSeconds.Minutes % 3 == 0 && timeInSeconds.Seconds < 30 && LeaderBoard.Drivers.Take(maxRows).Any(d => d.PitStopCount > 0);

            var numberOfDrivers = LeaderBoard.Drivers.Take(maxRows).Count();

            var height = 35 + numberOfDrivers * 32 + 23;
            if (LeaderBoard.LapCounter != null)
                height += 35;

            var width = showPitStopCount ? 219 : 189;

            Graphics.InRectangle(80, 80, width, height)
                .WithBrush(Styles.TransparentLightGray)
                .WithPen(Styles.BlackPen)
                .DrawRectangleWithBorder();

            var r = Graphics.InRectangle(80, 80, width, 35)
                .WithBrush(Styles.BlackBrush)
                .WithFont(Settings.FontName, 20, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Center);
            
           var headR = Graphics.InRectangle(80, 60, 189, 35)
               .WithLinearGradientBrush(Color.DarkGray, Color.White, LinearGradientMode.Vertical)
               .WithPen(Styles.BlackPen)
               .MoveLeft(20)
               .DrawRectangleWithBorder()
               .WithBrush(Styles.BlackBrush)
               .WithFont(Settings.FontName, 23, FontStyle.Bold)
               .WithStringFormat(StringAlignment.Center)
               .DrawText(LeaderBoard.RacePosition);

            if (LeaderBoard.LapCounter != null)
            {
                r = r.ToBelow();

                var darkRed = Color.DarkRed;
                Func<byte, int> adjust = x => Math.Min((int)(x * 1.4), 255);
                var lightRed = Color.FromArgb(adjust(darkRed.R), adjust(darkRed.G), adjust(darkRed.B));
                headR.ToBelow()
                    .WithLinearGradientBrush(darkRed, lightRed, LinearGradientMode.Vertical)
                    .WithPen(Styles.BlackPen)
                    .DrawRoundRectangle(5)
                    .WithBrush(Styles.WhiteBrush)
                    .WithFont(Settings.FontName, 18, FontStyle.Bold)
                    .WithStringFormat(StringAlignment.Near)
                    .DrawText(LeaderBoard.LapCounter, topOffset: 5, leftOffset: 20);
            }

            r = r.ToBelow(width: 36, height: 23).MoveRight(8);

            var headerSize = 12;
            var size = 16;
            var offset = 4;

            var n1 = r
                .WithFontSize(headerSize)
                .DrawText("Pos", topOffset: offset)
                .ToRight(width: 58)
                .WithFontSize(headerSize)
                .DrawText("Num", topOffset: offset);

            if (showPitStopCount)
                n1 = n1.ToRight(width: 30)
                    .WithFontSize(headerSize)
                    .DrawText("Pit", topOffset: offset);

            n1.ToRight(width: 95)
                .WithFontSize(headerSize)
                .WithStringFormat(StringAlignment.Near)
                .DrawText("Name", leftOffset: 10, topOffset: offset);

            foreach (var d in LeaderBoard.Drivers.Take(maxRows))
            {
                r = r.ToBelow(width: 36, height: 32);

                var position = d.Position != null ? d.Position.Value.ToString() : "";

                var n = r
                    .WithFontSize(size)
                    .DrawText(position, topOffset: offset)
                    .ToRight(width: 58)
                    .DrawText(d.CarNumber, topOffset: offset);

                var pitStopCount = d.PitStopCount != 0 ? d.PitStopCount.ToString() : " ";
                if (showPitStopCount)
                    n = n.ToRight(width: 30)
                    .WithFontSize(size)
                    .DrawText(pitStopCount, topOffset: offset);

                n.ToRight(width: 95)
                    .WithFontSize(size)
                    .WithStringFormat(StringAlignment.Near)
                    .DrawText(d.ShortName.Substring(0, 4).ToUpper(), leftOffset: 10, topOffset: offset);

                Graphics.InRectangle(r.Rectangle.Left, r.Rectangle.Top, n.Rectangle.Left + width, 10)
                    .WithPen(new Pen(Styles.Black, 2))
                    .DrawLine(r.Rectangle.Left, r.Rectangle.Top, n.Rectangle.Left + width, n.Rectangle.Top);
            }
        }
    }
}
