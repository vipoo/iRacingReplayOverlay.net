// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
//
// iRacingReplayOverlay is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayOverlay is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace iRacingReplayOverlay.net
{
	public class GraphicRect
    {
        readonly Graphics g;
        readonly Rectangle r;
        readonly Brush b;
        readonly Pen p;
        readonly Font f;
        readonly StringFormat sf;

        public GraphicRect(Graphics g, Rectangle r)
        {
            this.g = g;
            this.r = r;
        }

        public GraphicRect(Graphics g, Rectangle r, Brush b, Pen p, Font f, StringFormat sf)
        {
            this.g = g;
            this.r = r;
            this.b = b;
            this.p = p;
            this.f = f;
            this.sf = sf;
        }

        internal GraphicRect WithLinearGradientBrush(Color color1, Color color2, LinearGradientMode linearGradientMode)
        {
            return new GraphicRect(g, r, new LinearGradientBrush(r, color1, color2, linearGradientMode), p, f, sf);
        }

        internal GraphicRect With(Func<GraphicRect, GraphicRect> modifiers)
        {
            return modifiers(this);
        }

        internal GraphicRect WithPen(Pen pen)
        {
            return new GraphicRect(g, r, b, pen, f, sf);
        }

        internal GraphicRect DrawRectangleWithBorder()
        {
            g.FillRectangle(b, r);
            g.DrawRectangle(p, r);
            return this;
        }

        internal GraphicRect DrawText(string text, int leftOffset = 0)
        {
			var rect2 = new Rectangle(r.Left + leftOffset + 1, r.Top, r.Width, r.Height);
            g.DrawString(text, f, b, rect2, sf);
            return this;
        }

        internal GraphicRect WithBrush(Brush brush)
        {
            return new GraphicRect(g, r, brush, p, f, sf);
        }

        internal GraphicRect WithFont(string prototype, float emSize, FontStyle style)
        {
            return new GraphicRect(g, r, b, p, new Font(prototype, emSize, style), sf);
        }

        internal GraphicRect ToBelow(int? width = null, int? height = null)
        {
            var w = width == null ? r.Width : width.Value;
            var h = height == null ? r.Height : height.Value;

            return new GraphicRect(g, new Rectangle(r.Left, r.Top + r.Height, w, h), b, p, f, sf);
        }

        internal GraphicRect ToRight(int? width = null, int? height = null)
        {
            var w = width == null ? r.Width : width.Value;
            var h = height == null ? r.Height : height.Value;

            return new GraphicRect(g, new Rectangle(r.Left + r.Width, r.Top, w, h), b, p, f, sf);
        }

		public GraphicRect AfterText(string str, int i)
		{
			var size = g.MeasureString(str, f);
			var newRect = new Rectangle(r.Left + (int)size.Width + i, r.Top, r.Width - (int)size.Width - i, r.Height);
			return new GraphicRect(g, newRect, b, p, f, sf);
		}

        internal GraphicRect WithStringFormat(StringAlignment alignment, StringAlignment lineAlignment = StringAlignment.Near)
        {
            var sf = new StringFormat { Alignment = alignment, LineAlignment = lineAlignment };
            return new GraphicRect(g, r, b, p, f, sf);
        }
    }
    
}
