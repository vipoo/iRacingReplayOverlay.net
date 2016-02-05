using System.Drawing;

namespace iRacingDirector.Plugin.StandardOverlays
{
    public static class Styles
    {
        public const int AlphaLevel = 120;
        public static readonly Color White = Color.FromArgb(AlphaLevel, Color.White);
        public static readonly Color WhiteSmoke = Color.FromArgb(AlphaLevel, Color.WhiteSmoke);
        public static readonly Color Black = Color.FromArgb(AlphaLevel, Color.Black);
        public static readonly Color LightYellow = Color.FromArgb(AlphaLevel, Color.LightYellow);
        public static readonly Color Yellow = Color.FromArgb(AlphaLevel, 150, 150, 0);

        public static readonly Pen BlackPen = new Pen(Black);

        public static readonly Brush BlackBrush = new SolidBrush(Color.Black);
        public static readonly Brush RedBrush = new SolidBrush(Color.Red);
        public static readonly Brush WhiteBrush = new SolidBrush(Color.White);
        public static readonly Brush YellowBrush = new SolidBrush(Color.Yellow);
        public static readonly Brush TransparentLightBlueBrush = new SolidBrush(Color.FromArgb(AlphaLevel, Color.LightBlue));
    }
}
