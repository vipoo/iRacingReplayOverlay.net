using iRacingDirector.Plugin;
using System;
using System.Drawing;
using System.Linq;

namespace JockeOverlays
{
    public partial class MyPlugin
    {
        public LeaderBoard LeaderBoard;

        const int fontSize = 16;
        const int offset = 4;
        const int positionColumnWidth = 50;
        const int carNumberColumnWidth = 70;
        const int nameColumnWidth = 80;
        const int leaderboardWidth = positionColumnWidth + carNumberColumnWidth + nameColumnWidth;
        const int maxRows = 25;
        const int leaderboardLeft = 80;
        const int leaderboardTop = 60;
        const int rowHeight = 30;
        const int counterGap = 5;

        void DrawLeaderboard(TimeSpan timeInSeconds)
        {
            var graphics = Graphics.With()
                                   .WithFontSizeOf(fontSize)
                                   .WithTextOffset(topOffset: offset);

            var showPitStopCount = timeInSeconds.Minutes % 3 == 0 && timeInSeconds.Seconds < 30 && LeaderBoard.Drivers.Take(maxRows).Any(d => d.PitStopCount > 0);

            var top = DrawLapCounterRow(graphics);
            var first = true;

            foreach (var driver in LeaderBoard.Drivers.Take(maxRows))
            {
                if (!first)
                    top = DrawDividerLine(graphics, leaderboardLeft, top);

                DrawLeaderboardRow(graphics, top, driver, leaderboardLeft);

                top += rowHeight;
                first = false;
            }
        }

        private int DrawLapCounterRow(GraphicRect graphics)
        {
            var counter = GetLapCounterDescription();

            graphics.InRectangle(leaderboardLeft, leaderboardTop, leaderboardWidth, rowHeight)
                    .DrawBlackBackground()
                    .DrawWhiteText(counter, StringAlignment.Center);

            return leaderboardTop + rowHeight + counterGap;
        }

        private static void DrawLeaderboardRow(GraphicRect graphics, int top, Driver d, int left)
        {
            DrawPositionNumber(graphics, d, left, top);
            left = DrawBackgroundForCarNumberAndDriverName(graphics, left, top);
            left = DrawCarNumber(graphics, d, left, top);
            DrawDriverName(graphics, d, left, top);
        }

        static void DrawDriverName(GraphicRect graphics, Driver d, int left, int top)
        {
            graphics.InRectangle(left, top, nameColumnWidth, rowHeight)
                    .DrawWhiteText(d.ShortName.FormattedForLeaderboard(), StringAlignment.Near);
        }

        static int DrawCarNumber(GraphicRect graphics, Driver d, int left, int top)
        {
            graphics.InRectangle(left, top, carNumberColumnWidth, rowHeight)
                    .DrawWhiteText(d.CarNumber, StringAlignment.Center);

            return left + carNumberColumnWidth;
        }

        static int DrawBackgroundForCarNumberAndDriverName(GraphicRect graphics, int left, int top)
        {
            left += positionColumnWidth;
            graphics.InRectangle(left, top, leaderboardWidth - positionColumnWidth, rowHeight)
                    .DrawBlackBackground();
            return left;
        }

        static void DrawPositionNumber(GraphicRect graphics, Driver d, int left, int top)
        {
            graphics.InRectangle(left, top, positionColumnWidth, rowHeight)
                    .DrawGrayBackground()
                    .DrawWhiteText(d.Position.ToString(), StringAlignment.Center);
        }

        static int DrawDividerLine(GraphicRect graphics, int left, int top)
        {
            graphics.InRectangle(left, top, leaderboardWidth, 4)
                    .WithPen(Styles.DividerLinePen)
                    .DrawLine(leaderboardLeft, top, leaderboardLeft + leaderboardWidth, top);

            return top + 1;
        }

        string GetLapCounterDescription()
        {
            if (LeaderBoard.LapCounter != null)
                return string.Format("{0} / {1}", LeaderBoard.LapCounter, LeaderBoard.RacePosition).ToUpper();

            return LeaderBoard.RacePosition;
        }
    }
}
