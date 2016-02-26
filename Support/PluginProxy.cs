using iRacingReplayOverlay.Phases.Capturing;
using iRacingSDK;
using MediaFoundation.Net;
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
        private dynamic plugin;
        private Type pluginType;
        private OverlayData data;
        private long timestamp;
        private object leaderboardInstance;

        public long Duration
        {
            get
            {
                return data.LeaderBoards.Last().StartTime.FromSecondsToNano();
            }
        }

        public PluginProxy(string pluginPath)
        {
            var an = AssemblyName.GetAssemblyName(pluginPath);
            var assembly = Assembly.Load(an);

            var type = assembly.GetTypes()
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsAbstract)
                .FirstOrDefault(t => t.FullName.EndsWith(".MyPlugin"));

            plugin = Activator.CreateInstance(type);
            pluginType = (Type)plugin.GetType();
        }

        public void DrawIntroFlashCard(long duration)
        {
            plugin.IntroFlashCard(duration, this.timestamp);
        }

        public void SetGraphics(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            plugin.Graphics = graphics;
        }

        public void SetReplayConfig(string filePath)
        {
            this.data = OverlayData.FromFile(filePath);
        }

        public void SetReplayConfig(OverlayData data)
        {
            this.data = data;
        }

        public void SetEventData()
        {
            var eventDataField = pluginType.GetField("EventData");
            var eventDataType = eventDataField.FieldType;
            var instance = Activator.CreateInstance(eventDataType, data.SessionData);

            eventDataField.SetValue(plugin, instance);
        }

        public void SetLeaderboard()
        {
            var leaderboardDataField = pluginType.GetField("LeaderBoard");
            var leaderboardDataType = leaderboardDataField.FieldType;

            var lb = data.LeaderBoards.FirstOrDefault(t => t.StartTime >= timestamp.FromNanoToSeconds());

            if (lb == null)
            {
                leaderboardDataField.SetValue(plugin, null);
                return;
            }

            leaderboardInstance = Activator.CreateInstance(leaderboardDataType);
            leaderboardDataField.SetValue(plugin, leaderboardInstance);

            SetLeaderboardField("StartTime", lb.StartTime);
            SetLeaderboardField("RacePosition", lb.RacePosition);
            SetLeaderboardField("LapCounter", lb.LapCounter);

            var driversDataField = leaderboardDataType.GetField("Drivers");
            var driversDataArrayType = driversDataField.FieldType;
            var driversDataType = driversDataArrayType.GetElementType();

            var driversInstance = Activator.CreateInstance(driversDataArrayType, lb.Drivers.Length);
            var x = driversInstance.GetType().GetMethod("Set");
            SetLeaderboardField("Drivers", driversInstance);

            for (var i = 0; i < lb.Drivers.Length; i++) {
                var driverInstance = Activator.CreateInstance(driversDataType);

                x.Invoke(driversInstance, new object[] { i, driverInstance });

                SetField(driverInstance, "CarNumber", lb.Drivers[i].CarNumber);
                SetField(driverInstance, "UserName", lb.Drivers[i].UserName);
                SetField(driverInstance, "PitStopCount", lb.Drivers[i].PitStopCount);
                SetField(driverInstance, "ShortName", lb.Drivers[i].ShortName);
                SetField(driverInstance, "Position", lb.Drivers[i].Position);
            }
        }

        void SetField(object instance, string name, object value)
        {
            instance
                .GetType()
                .GetField(name)
                .SetValue(instance, value);
        }

        void SetLeaderboardField(string name, object value)
        {
            SetField(leaderboardInstance, name, value);
        }

        public void SetTimestamp(long timestamp)
        {
            this.timestamp = timestamp;
        }

        public void RaceOverlay(long timestamp)
        {
            plugin.RaceOverlay(timestamp);
        }
    }
}
