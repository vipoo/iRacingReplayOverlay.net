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

using iRacingReplayOverlay.Drawing;
using iRacingReplayOverlay.Phases.Capturing;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using MediaFoundation.Net;
using iRacingReplayOverlay.Support;
using iRacingSDK.Support;
using iRacingSDK;
using System.Reflection;
using System.IO;

namespace iRacingReplayOverlay.Phases.Transcoding
{
    public class LeaderBoard
    {
        public OverlayData OverlayData;
        public const int DriversPerPage = 10;

        readonly _Styles Styles = new _Styles();
        PluginProxy plugin;

        const int FlashCardWidth = 900;
        const int FlashCardLeft = (1920 / 2) - FlashCardWidth / 2;

        
        public LeaderBoard()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.plugin = new PluginProxy(Path.Combine(path, @"plugins\overlay\iRacingDirector.Plugin.StandardOverlays.dll"));
        }

        public void DrawFlashCard(string title, Graphics graphics, long timestamp, Action<GraphicRect> drawBody)
        {
            var timeInSeconds = timestamp.FromNanoToSeconds();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var totalWidth = FlashCardWidth;
            var left = FlashCardLeft;
            var r = graphics.InRectangle(left, 250, totalWidth, 575);

            r
                .WithBrush(new SolidBrush(Color.FromArgb(180, Color.Gray)))
                .WithPen(Styles.BlackPen)
                .DrawRectangleWithBorder();

            graphics.InRectangle(left - 10, 240, totalWidth - 100, 72)
                .WithLinearGradientBrush(Color.DarkGray, Color.White, LinearGradientMode.Vertical)
                .WithPen(Styles.BlackPen)
                .DrawRoundRectangle(5)
                .MoveDown(7)
                .MoveRight(20)
                .WithBrush(Styles.BlackBrush)
                .WithFont(fontName, 23, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near)
                .DrawText(OverlayData.SessionData.WeekendInfo.TrackDisplayName.ToUpper())
                .MoveDown(32)
                .WithFont(fontName, 17, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near)
                .DrawText(OverlayData.SessionData.WeekendInfo.TrackCity.ToUpper() + ", " + OverlayData.SessionData.WeekendInfo.TrackCountry.ToUpper());

            var darkRed = Color.DarkRed;
            Func<byte, int> adjust = x => Math.Min((int)(x * 1.4), 255);
            var lightRed = Color.FromArgb(adjust(darkRed.R), adjust(darkRed.G), adjust(darkRed.B));
            graphics.InRectangle(left - 10, 311, totalWidth - 100, 48)
                .WithLinearGradientBrush(darkRed, lightRed, LinearGradientMode.Vertical)
                .WithPen(Styles.BlackPen)
                .DrawRoundRectangle(5)
                .MoveDown(7)
                .MoveRight(20)
                .WithBrush(Styles.WhiteBrush)
                .WithFont(fontName, 23, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near)
                .DrawText(title);

            r = graphics.InRectangle(left + 30, 400, 60, 40)
                .WithPen(Styles.BlackPen)
                .WithBrush(Styles.BlackBrush)
                .WithFont(fontName, 20, FontStyle.Bold)
                .WithStringFormat(StringAlignment.Near);

            drawBody(r);
        }
        

        public void Intro(Graphics graphics, long duration, long timestamp)
        {
            plugin.SetReplayConfig(OverlayData);
            plugin.SetGraphics(graphics);
            plugin.InjectFields(timestamp);
            plugin.DrawIntroFlashCard( duration );
        }

        public void Overlay(Graphics graphics, long timestamp)
        {
            plugin.SetReplayConfig(OverlayData);
            plugin.SetGraphics(graphics);
            plugin.InjectFields(timestamp);
            plugin.RaceOverlay();
        }

        public void Outro(Graphics graphics, long timestamp, int page = 0)
        {
            DrawFlashCard("Race Results", graphics, timestamp, r => DrawOutroBody(graphics, r, page));
        }

        public void DrawOutroBody(Graphics graphics, GraphicRect r, int page)
        { 
            var rsession = OverlayData.SessionData.SessionInfo.Sessions.Race();

            var results = rsession.ResultsPositions ?? new SessionData._SessionInfo._Sessions._ResultsPositions[0];

            var offset = 5;
            var pen = new Pen(Styles.Black, 2);
            graphics.InRectangle(FlashCardLeft, r.Rectangle.Top, FlashCardWidth, 10)
                .WithPen(pen)
                .DrawLine(FlashCardLeft, r.Rectangle.Top - offset, FlashCardLeft + FlashCardWidth, r.Rectangle.Top - offset);

            var LeaderTime = TimeSpan.FromSeconds(results[0].Time);

            foreach (var racer in results.Skip(DriversPerPage * page).Take(DriversPerPage))
            {
                var driver = OverlayData.SessionData.DriverInfo.CompetingDrivers[racer.CarIdx];

                var Gap = TimeSpan.FromSeconds(racer.Time) - LeaderTime; // Gap calculation
                if (Gap == TimeSpan.Zero) //For the leader we want to display the race duration
                    Gap = LeaderTime;

                r.WithBrush(Settings.Default.PreferredDriverNames.Contains(driver.UserName) ? Styles.RedBrush : Styles.BlackBrush);

                r
                    .Center(cg => cg
                            .DrawText(racer.Position.ToString())
                            .AfterText(racer.Position.ToString())
                            .MoveRight(1)
                            .WithFont(fontName, 16, FontStyle.Bold)
                            .DrawText(racer.Position.Ordinal())
                    )
                    .ToRight(width: 190, left: 30)
                    .DrawText(Gap.ToString("hh\\:mm\\:ss\\.fff"))
                    .ToRight(width: 80, left: 20)
                    .DrawText(driver.CarNumber)
                    .ToRight(width: 350)
                    .DrawText(driver.UserName);

                r = r.ToBelow();

                graphics.InRectangle(FlashCardLeft, r.Rectangle.Top, FlashCardWidth, 10)
                    .WithPen(pen)
                    .DrawLine(FlashCardLeft, r.Rectangle.Top - offset, FlashCardLeft + FlashCardWidth, r.Rectangle.Top - offset);
            }
        }

        



        public Func<GraphicRect, GraphicRect> SimpleWhiteBox(int fontSize = 20)
        {
            return rr => rr.WithLinearGradientBrush(Styles.WhiteSmoke, Styles.White, LinearGradientMode.BackwardDiagonal)
            .WithPen(Styles.BlackPen)
            .DrawRectangleWithBorder()
            .WithBrush(Styles.BlackBrush)
            .WithFont(fontName, fontSize, FontStyle.Bold)
            .WithStringFormat(StringAlignment.Center);
        }

        const string fontName = "Arial";

        public Func<GraphicRect, GraphicRect> ColourBox(Color color, int fontSize = 20)
        {
            return rr =>
                rr.WithBrush(new SolidBrush(color))
                    .WithPen(Styles.BlackPen)
                    .DrawRectangleWithBorder()
                    .WithFont(fontName, fontSize, FontStyle.Bold)
                    .WithBrush(Styles.BlackBrush)
                    .WithStringFormat(StringAlignment.Center);
        }

        void DrawLeaderboard(Graphics g, OverlayData.LeaderBoard sample, TimeSpan timeInSeconds)
        {
            var maxRows = sample.LapCounter == null ? 22 : 21;
            var showPitStopCount = timeInSeconds.Minutes % 3 == 0 && timeInSeconds.Seconds < 30 && sample.Drivers.Take(maxRows).Any(d => d.PitStopCount > 0);

            var r = g.InRectangle(80, 80, showPitStopCount ? 219 : 189, 35)
                .With(SimpleWhiteBox())
                .DrawText(sample.RacePosition, topOffset: 3);

            if (sample.LapCounter != null)
                r = r.ToBelow()
                    .With(SimpleWhiteBox())
                    .DrawText(sample.LapCounter, topOffset: 3);

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

            foreach (var d in sample.Drivers.Take(maxRows))
            {
                r = r.ToBelow(width: 36, height: 32);

                var position = d.Position != null ? d.Position.Value.ToString() : "";

                var n = r.With(ColourBox(Styles.LightYellow, size))
                    .DrawText(position, topOffset: offset)
                    .ToRight(width: 58)
                    .With(SimpleWhiteBox(size))
                    .DrawText(d.CarNumber, topOffset: offset);

                var pitStopCount = d.PitStopCount != 0 ? d.PitStopCount.ToString() : " ";
                if( showPitStopCount )
                    n = n.ToRight(width: 30)
                    .With(SimpleWhiteBox(size))
                    .DrawText(pitStopCount, topOffset: offset);

                n.ToRight(width: 95)
                    .With(SimpleWhiteBox(size))
                    .WithStringFormat(StringAlignment.Near)
                    .DrawText(d.ShortName.ToUpper(), leftOffset: 10,  topOffset: offset);
            }
        }

       


        public class _Styles
        {
            public const int AlphaLevel = 120;
            public readonly Color White = Color.FromArgb(AlphaLevel, Color.White);
            public readonly Color WhiteSmoke = Color.FromArgb(AlphaLevel, Color.WhiteSmoke);
            public readonly Color Black = Color.FromArgb(AlphaLevel, Color.Black);
            public readonly Color LightYellow = Color.FromArgb(AlphaLevel, Color.LightYellow);
            public readonly Color Yellow = Color.FromArgb(AlphaLevel, 150, 150, 0);

            Pen blackPen;
            public Pen BlackPen
            {
                get
                {
                    blackPen = blackPen ?? new Pen(Black);
                    return blackPen;
                }
            }

            public readonly Brush BlackBrush = new SolidBrush(Color.Black);
            public readonly Brush RedBrush = new SolidBrush(Color.Red);
            public readonly Brush WhiteBrush = new SolidBrush(Color.White);
            public readonly Brush YellowBrush = new SolidBrush(Color.Yellow);
            public readonly Brush TransparentLightBlueBrush = new SolidBrush(Color.FromArgb(AlphaLevel, Color.LightBlue));
        }
    }
}