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
        private dynamic plugin;

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

        public void SetEventData(SessionData result)
        {
            var pluginType = (Type)plugin.GetType();

            var eventDataField = pluginType.GetField("EventData");
            var eventDataType = eventDataField.FieldType;
            var instance = Activator.CreateInstance(eventDataType, result);

            eventDataField.SetValue(plugin, instance);
        }

        public void DrawIntroFlashCard(long duration, long timestamp)
        {
            plugin.IntroFlashCard(duration, timestamp);
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
