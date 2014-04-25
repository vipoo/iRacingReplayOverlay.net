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
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class OverlayData
    {
        public class TimingSample
        {
            public long StartTime;
            public Driver[] Drivers;
            public string RacePosition;
            public Driver CurrentDriver;
        }

        public class Driver
        {
            public int Position;
            public string Indicator;
            public int CarNumber;
            public string Name;

            [XmlIgnore]
            public Dictionary<string, string> DriverNickNames = new Dictionary<string, string>();

            [XmlIgnore]
            public string ShortName
            {
                get
                {
                    if (DriverNickNames.ContainsKey(Name))
                        return DriverNickNames[Name];

                    var names = Name.Split(' ');
                    var name = names.First().Substring(0, 1).ToUpper()
                        + names.Last().Substring(0, 1).ToUpper()
                        + names.Last().Substring(1, 3);

                    DriverNickNames[Name] = name;
                    return name;
                }
            }
        }

        public class FastLap
        {
            public long StartTime;
            public Driver Driver;
            public double Time;
        }

        public List<TimingSample> TimingSamples = new List<TimingSample>();
        public List<FastLap> FastestLaps = new List<FastLap>();

        public void SaveTo(string fileName)
        {
            var writer = new XmlSerializer(typeof(OverlayData));

            using (var file = new StreamWriter(fileName))
                writer.Serialize(file, this);
        }

        public static OverlayData FromFile(string fileName, Dictionary<string, string> driverNickNames)
        {
            var reader = new XmlSerializer(typeof(OverlayData));
            using (var file = new StreamReader(fileName))
            {
                var result = (OverlayData)reader.Deserialize(file);
                foreach (var timingSample in result.TimingSamples)
                    foreach (var d in timingSample.Drivers)
                        d.DriverNickNames = driverNickNames;

                return result;
            }
        }
    }

}
