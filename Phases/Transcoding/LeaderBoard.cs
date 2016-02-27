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

using iRacingReplayOverlay.Phases.Capturing;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace iRacingReplayOverlay.Phases.Transcoding
{
    public class LeaderBoard
    {
        public OverlayData OverlayData;
        public const int DriversPerPage = 10;

        PluginProxy plugin;
        
        public LeaderBoard()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.plugin = new PluginProxy(Path.Combine(path, @"plugins\overlay\iRacingDirector.Plugin.StandardOverlays.dll"));
        }

        public void Intro(Graphics graphics, long duration, long timestamp)
        {
            plugin.SetReplayConfig(OverlayData);
            plugin.SetGraphics(graphics);
            plugin.InjectFields(timestamp, Settings.Default.PreferredDriverNames.Split(new char[] { ',', ';' }));
            plugin.DrawIntroFlashCard( duration );
        }

        public void Overlay(Graphics graphics, long timestamp)
        {
            plugin.SetReplayConfig(OverlayData);
            plugin.SetGraphics(graphics);
            plugin.InjectFields(timestamp, Settings.Default.PreferredDriverNames.Split(new char[] { ',', ';' }));
            plugin.RaceOverlay();
        }

        public void Outro(Graphics graphics, long duration, long timestamp, long period)
        {
            plugin.SetReplayConfig(OverlayData);
            plugin.SetGraphics(graphics);
            plugin.InjectFields(timestamp, Settings.Default.PreferredDriverNames.Split(new char[] { ',', ';' }));
            plugin.DrawOutroFlashCard(duration, period);
        }
    }
}