using System;

namespace iRacingReplayOverlay
{
    [Serializable]
    public class PluginProxySettings
    {
        public string Name;  //Prefixed with plugin Name
        public object Value;
        public Type Type;
        public string Description;
    }
}
