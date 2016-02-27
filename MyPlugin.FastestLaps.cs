using System;
using System.Drawing;

namespace iRacingDirector.Plugin.StandardOverlays
{
    public partial class MyPlugin
    {
        public FastLap FastLap;

        void DrawFastestLap()
        {
            if (FastLap == null)
                return;
            
            Func<GraphicRect, GraphicRect> blueBox = rr =>
               rr.WithBrush(Styles.TransparentLightBlueBrush)
               .WithPen(Styles.BlackPen)
               .DrawRectangleWithBorder()
               .WithBrush(Styles.BlackBrush)
               .WithFont(Settings.FontName, 20, FontStyle.Bold)
               .WithStringFormat(StringAlignment.Center);

            Graphics.InRectangle(1920 - 80 - 450, 900, 450, 34)
                .With(blueBox)
                .DrawText("New Fast Lap")
                .ToBelow(width: 50)
                .With(blueBox)
                .DrawText(FastLap.Driver.CarNumber)
                .ToRight(width: 250)
                .With(blueBox)
                .DrawText(FastLap.Driver.UserName)
                .ToRight(width: 150)
                .With(blueBox)
                .DrawText(TimeSpan.FromSeconds(FastLap.Time).ToString(@"mm\:ss\.fff"));
        }
    }
}
