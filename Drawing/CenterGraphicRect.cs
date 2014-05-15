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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace iRacingReplayOverlay.Drawing
{
	public class CenterGraphicRect : GraphicRect
	{
		readonly int left;
		readonly int right;

		public CenterGraphicRect(Graphics g, Rectangle r, Brush b, Pen p, Font f, StringFormat sf)
			: base( g, r, b, p, f, sf)
		{
			this.left = int.MaxValue;
			this.right = int.MinValue;
		}

		public CenterGraphicRect(Graphics g, Rectangle r, Brush b, Pen p, Font f, StringFormat sf, int left, int right)
			: base( g, r, b, p, f, sf)
		{
			this.left = left;
			this.right = right;
		}

		protected override GraphicRect New(Graphics g, Rectangle r, Brush b, Pen p, Font f, StringFormat sf)
		{
			return new CenterGraphicRect(g, r, b, p, f, sf, left, right);
		}

        internal int Width { get { return right-left; } }
		
		public override GraphicRect DrawText(string text, int leftOffset = 0)
		{
            var size = TextRenderer.MeasureText(g, text, f, r.Size, TextFormatFlags.NoPadding);

			var newleft = Math.Min(r.Left + leftOffset, left);
			var newRight = Math.Max(right, r.Left + leftOffset + (int)size.Width);

			return new CenterGraphicRect(g, r, b, p, f, sf, newleft, newRight);
		}

        internal override GraphicRect DrawRectangleWithBorder()
        {
            return this;
        }
	}
}
