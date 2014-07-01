using System;
using iRacingSDK;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace iRacingReplayOverlay.net.Tests
{
    public class Support
    {
        public static void Main()
        {
            new iRacingReplayOverlay.Phases.Direction.Support.Tests.BattleTest().ShouldIdentifyTwoBattles();
        }
    }
}

