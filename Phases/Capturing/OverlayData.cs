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
        public class BoringBit
        {
            public double StartTime;
            public double EndTime;
        }

        public class MessageState
        {
            public string[] Messages;
            public double Time;
        }

        public class TimingSample
        {
            public double StartTime;
            public Driver[] Drivers;
            public string RacePosition;
			public string LapCounter; //optional
            public Driver CurrentDriver;

            public TimingSample Clone()
            {
                return (TimingSample)base.MemberwiseClone();
            }
        }

        public class Driver
        {
            public int Position;
            public string Indicator;
            public int CarNumber;
            public string Name;

            [XmlIgnore]
            public int CarIdx;

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
                    var firstName = names.First();
                    var lastName = names.Last();

                    var name = firstName.Substring(0, 1).ToUpper()
                        + lastName.Substring(0, 1).ToUpper()
                        + lastName.Substring(1, Math.Min(3, lastName.Length-1));

                    DriverNickNames[Name] = name;
                    return name;
                }
            }

            public Driver Clone()
            {
                return (Driver)base.MemberwiseClone();
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
        public List<MessageState> MessageStates = new List<MessageState>();
        public List<BoringBit> EditCuts = new List<BoringBit>();

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
