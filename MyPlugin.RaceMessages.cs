using System;
using System.Drawing;

namespace iRacingDirector.Plugin.StandardOverlays
{
    public partial class MyPlugin
    {
        public MessageSet MessageSet;

        void DrawRaceMessages(double timeInSeconds)
        {
            if (MessageSet == null)
                return;

            Func<GraphicRect, GraphicRect> blueBox = rr =>
               rr.WithBrush(Styles.TransparentLightBlueBrush)
               .WithPen(Styles.BlackPen)
               .DrawRectangleWithBorder()
               .WithBrush(Styles.BlackBrush)
               .WithFont(Settings.FontName, 20, FontStyle.Bold)
               .WithStringFormat(StringAlignment.Near);

            var shiftFactor = (double)Math.Min(timeInSeconds - MessageSet.Time, 1d);
            var offset = (int)(34 * shiftFactor);

            offset = offset + (MessageSet.Messages.Length - 1) * 34;

            var row4Top = 900 + 34 * 3;
            offset = row4Top - offset;

            var r = Graphics.InRectangle(80, offset, 450, 34);

            Graphics.SetClip(new Rectangle(80, 900, 450, 34 + 34 + 34));

            foreach (var msg in MessageSet.Messages)
                r = r.With(blueBox)
                    .DrawText(" " + msg)
                    .ToBelow();

            Graphics.ResetClip();
        }
    }
}
