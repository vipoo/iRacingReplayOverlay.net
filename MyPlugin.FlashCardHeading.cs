using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace iRacingDirector.Plugin.StandardOverlays
{
    public partial class MyPlugin
    {
        GraphicRect DrawFlashCardHeading(string title)
        {
            var displayName = EventData.WeekendInfo.TrackDisplayName.ToUpper();

            Graphics.InRectangle(FlashCardLeft, 250, FlashCardWidth, 575)
                .WithBrush(Styles.TransparentLightGray)
                .WithPen(Styles.BlackPen)
                .DrawRectangleWithBorder();

            Graphics.InRectangle(FlashCardLeft - 10, 240, FlashCardWidth - 100, 72)
                .WithLinearGradientBrush(Color.DarkGray, Color.White, LinearGradientMode.Vertical)
                .WithPen(Styles.BlackPen)
                .DrawRectangleWithBorder()
                .MoveDown(7)
                .MoveRight(20)
                .WithBrush(Styles.BlackBrush)
                .WithFont(Settings.FontName, 23, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near)
                .DrawText(displayName)
                .MoveDown(32)
                .WithFont(Settings.FontName, 17, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near)
                .DrawText(EventData.WeekendInfo.TrackCity.ToUpper() + ", " + EventData.WeekendInfo.TrackCountry.ToUpper());

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
    }
}
