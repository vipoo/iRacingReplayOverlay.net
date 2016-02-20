using iRacingSDK;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;

//TODO: Add plugin management form to main app
//TODO: Discover overlay plugins, and show list in form
//TODO: Allow user to 'select' a plugin, and use that plugin during transcoding
//TODO: Show the plugin's settings & allow user to change.
//TODO: Move all leaderboard graphiccs into plugin
//TODO: Create plugin test app - to draw on a single bitmap
//TODO: Creating publishing model - allow app to install different versions of plugin.
//TODO: Ensure good error feedback if plugin missing stuff (eg: Graphics field)
//TODO: Move support files from overlayplugin to plugin base
//TODO: Add lots more style helpers
//TODO: Additional meta data for plugin - name, description, author, etc. - use assembly attributes
//TODO: Extract timing logic into seperate methods

namespace iRacingReplayOverlay
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

        public void DrawIntroFlashCard(long timestamp, int page)
        {
            plugin.DrawIntroFlashCard(timestamp, page);
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

            ((dynamic)plugin).Graphics = graphics;
        }
    }
}
