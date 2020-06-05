using iRacingDirector.Plugin;
using System.Drawing;

namespace NoOverlay
{
    public static class Styles
    {
        public const int AlphaLevel = 0;
        public static readonly Color White = Color.FromArgb(AlphaLevel, Color.White);
        public static readonly Color WhiteSmoke = Color.FromArgb(AlphaLevel, Color.WhiteSmoke);
        public static readonly Color Black = Color.FromArgb(AlphaLevel, Color.Black);

        public static readonly Color RedBannerDark = Color.DarkRed;
        public static readonly Color RedBannerLight = Color.DarkRed.BrightenBy(1.4);

        public static readonly Pen BlackPen = new Pen(Black);
        public static readonly Pen ThickBlackPen = new Pen(Black, 2);
        public static readonly Pen WhitePen = new Pen(White);

        public static readonly Brush BlackBrush = new SolidBrush(Color.Black);
        public static readonly Brush RedBrush = new SolidBrush(Color.Red);
        public static readonly Brush WhiteBrush = new SolidBrush(Color.White);
        public static readonly Brush YellowBrush = new SolidBrush(Color.Yellow);

        public static readonly Brush TransparentLightBlack = new SolidBrush(Color.FromArgb(200, 30, 30, 30));

        public static readonly Brush TransparentLightGray = new SolidBrush(Color.FromArgb(200, 60, 60, 60));
        public static readonly Brush TransparentLighterGray = new SolidBrush(Color.FromArgb(40, Color.LightGray));

        public static readonly Pen DividerLinePen = new Pen(Color.FromArgb(255, 40, 40, 40), 2);
    }
}
