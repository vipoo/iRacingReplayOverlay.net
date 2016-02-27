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

            var r = Graphics.InRectangle(80, 80, showPitStopCount ? 219 : 189, 35)
                .With(SimpleWhiteBox())
                .DrawText(LeaderBoard.RacePosition, topOffset: 3);

            if (LeaderBoard.LapCounter != null)
                r = r.ToBelow()
                    .With(SimpleWhiteBox())
                    .DrawText(LeaderBoard.LapCounter, topOffset: 3);

            r = r.ToBelow(width: 36, height: 23);

            var headerSize = 12;
            var size = 17;
            var offset = 4;

            var n1 = r.With(ColourBox(Styles.LightYellow, headerSize))
                .DrawText("Pos", topOffset: offset)
                .ToRight(width: 58)
                .With(SimpleWhiteBox(headerSize))
                .DrawText("Num", topOffset: offset);

            if (showPitStopCount)
                n1 = n1.ToRight(width: 30)
                .With(SimpleWhiteBox(headerSize))
                .DrawText("Pit", topOffset: offset);

            n1.ToRight(width: 95)
                .With(SimpleWhiteBox(headerSize))
                .WithStringFormat(StringAlignment.Near)
                .DrawText("Name", leftOffset: 10, topOffset: offset);

            foreach (var d in LeaderBoard.Drivers.Take(maxRows))
            {
                r = r.ToBelow(width: 36, height: 32);

                var position = d.Position != null ? d.Position.Value.ToString() : "";

                var n = r.With(ColourBox(Styles.LightYellow, size))
                    .DrawText(position, topOffset: offset)
                    .ToRight(width: 58)
                    .With(SimpleWhiteBox(size))
                    .DrawText(d.CarNumber, topOffset: offset);

                var pitStopCount = d.PitStopCount != 0 ? d.PitStopCount.ToString() : " ";
                if (showPitStopCount)
                    n = n.ToRight(width: 30)
                    .With(SimpleWhiteBox(size))
                    .DrawText(pitStopCount, topOffset: offset);

                n.ToRight(width: 95)
                    .With(SimpleWhiteBox(size))
                    .WithStringFormat(StringAlignment.Near)
                    .DrawText(d.ShortName.ToUpper(), leftOffset: 10, topOffset: offset);
            }
        }
    }
}
