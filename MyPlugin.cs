using iRacingSDK;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace iRacingDirector.Plugin.StandardOverlays
{
    public static class Settings
    {
        [Description("The font to use for the flash cards")]
        public static string FontName = "Arial";
    }

    public class MyPlugin : IPlugin
    {
        public Graphics Graphics;

        public SessionData._WeekendInfo WeekendInfo;
        public SessionData._SessionInfo._Sessions._ResultsPositions[] QualifyingResults;
        public SessionData._DriverInfo._Drivers[] CompetingDrivers;

        const int FlashCardWidth = 900;
        const int FlashCardLeft = (1920 / 2) - FlashCardWidth / 2;
        const int DriversPerPage = 10;

        public void DrawIntroFlashCard(long timestamp, int page)
        {
            var r = DrawFlashCardHeading("Qualifying Results");

            DrawFlashCardIntro(r, page);
        }

        GraphicRect DrawFlashCardHeading(string title)
        {
            var displayName = WeekendInfo.TrackDisplayName.ToUpper();

            Graphics.InRectangle(FlashCardLeft, 250, FlashCardWidth, 575)
                .WithBrush(new SolidBrush(Color.FromArgb(180, Color.Gray)))
                .WithPen(Styles.BlackPen)
                .DrawRectangleWithBorder();

            Graphics.InRectangle(FlashCardLeft - 10, 240, FlashCardWidth - 100, 72)
                .WithLinearGradientBrush(Color.DarkGray, Color.White, LinearGradientMode.Vertical)
                .WithPen(Styles.BlackPen)
                .DrawRectangleWithBorder()
                //.DrawRoundRectangle(5)
                .MoveDown(7)
                .MoveRight(20)
                .WithBrush(Styles.BlackBrush)
                .WithFont(Settings.FontName, 23, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near)
                .DrawText(displayName)
                .MoveDown(32)
                .WithFont(Settings.FontName, 17, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near)
                .DrawText(WeekendInfo.TrackCity.ToUpper() + ", " + WeekendInfo.TrackCountry.ToUpper());

            var darkRed = Color.DarkRed;
            Func<byte, int> adjust = x => Math.Min((int)(x * 1.4), 255);
            var lightRed = Color.FromArgb(adjust(darkRed.R), adjust(darkRed.G), adjust(darkRed.B));
            Graphics.InRectangle(FlashCardLeft - 10, 311, FlashCardWidth - 100, 48)
                .WithLinearGradientBrush(darkRed, lightRed, LinearGradientMode.Vertical)
                .WithPen(Styles.BlackPen)
                .DrawRoundRectangle(5)
                .MoveDown(7)
                .MoveRight(20)
                .WithBrush(Styles.WhiteBrush)
                .WithFont(Settings.FontName, 23, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near)
                .DrawText(title);

            return Graphics.InRectangle(FlashCardLeft + 30, 400, 60, 40)
                .WithPen(Styles.BlackPen)
                .WithBrush(Styles.BlackBrush)
                .WithFont(Settings.FontName, 20, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near);
        }

        void DrawFlashCardIntro(GraphicRect r, int page)
        {
            var totalWidth = FlashCardWidth;
            var left = FlashCardLeft;

            var thisPageOfQualifyingResults = QualifyingResults.Skip(page * DriversPerPage).Take(DriversPerPage);

            var offset = 5;
            var pen = new Pen(Styles.Black, 2);
            Graphics.InRectangle(left, r.Rectangle.Top, totalWidth, 10)
                .WithPen(pen)
                .DrawLine(left, r.Rectangle.Top - offset, left + totalWidth, r.Rectangle.Top - offset);

            foreach (var qualifier in thisPageOfQualifyingResults)
            {
                var driver = CompetingDrivers[qualifier.CarIdx];
                r
                    .Center(cg => cg
                            .DrawText(qualifier.Position.ToString())
                            .AfterText(qualifier.Position.ToString())
                            .MoveRight(1)
                            .WithFont(Settings.FontName, 16, FontStyle.Bold)
                            .DrawText(qualifier.Position.Ordinal())
                    )
                    .ToRight(width: 120, left: 30)
                    .DrawText(TimeSpan.FromSeconds(qualifier.FastestTime).ToString("mm\\:ss\\.ff"))
                    .ToRight(width: 60)
                    .DrawText(driver.CarNumber)
                    .ToRight(width: 300)
                    .DrawText(driver.UserName);

                r = r.ToBelow();

                Graphics.InRectangle(left, r.Rectangle.Top, totalWidth, 10)
                    .WithPen(pen)
                    .DrawLine(left, r.Rectangle.Top - offset, left + totalWidth, r.Rectangle.Top - offset);
            }
        }
    }
}
