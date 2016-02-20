using iRacingSDK;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;

//TODO: Add timing, leaderboard and closing titles to overlay
//TODO: Provide user configuration for settings
//TODO: Add 'Font' type of configuration
//TODO: Auto reload plugin on assembly update
//TODO: Enable StandardOverlay to be loaded as single project solution
//TODO: Move support code from overlay plugin into iRacingDirector.Plugin.Support

namespace iRacingDirector
{
    public class PluginProxy
    {
        public PluginProxy(string pluginPath)
        {
            var an = AssemblyName.GetAssemblyName(pluginPath);
            var assembly = Assembly.Load(an);
            
            var type = assembly.GetTypes()
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsAbstract)
                .FirstOrDefault(t => t.FullName.EndsWith(".MyPlugin"));

            plugin = Activator.CreateInstance(type);
        }

        private dynamic plugin;

        public void SetWeekendInfo(SessionData._WeekendInfo weekendInfo)
        {
            plugin.WeekendInfo = weekendInfo;
        }

        public void DrawIntroFlashCard( long timestamp, int page)
        {
            plugin.DrawIntroFlashCard( timestamp, page);
        }

        public void SetQualifyingResults(SessionData._SessionInfo._Sessions._ResultsPositions[] qualifyingResults)
        {
            plugin.QualifyingResults = qualifyingResults;
        }

        public void SetCompetingDrivers(SessionData._DriverInfo._Drivers[] competingDrivers)
        {
            plugin.CompetingDrivers = competingDrivers;
        }

        public void SetGraphics(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            plugin.Graphics = graphics;
        }
    }
}
