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
using iRacingReplayOverlay.Phases.Transcoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using MediaFoundation.Net;
using iRacingSDK;
using System.Linq;

namespace ImagerOverlayer
{
    class MainClass
    {
        static OverlayData BuildData()
        {
            var result = new OverlayData();

            result.SessionData = new SessionData
            {
                DriverInfo = new SessionData._DriverInfo
                {
                    Drivers = Enumerable.Range(0, 24).Select( i => new SessionData._DriverInfo._Drivers 
                        {
                            CarIdx = i,
                            UserName = "Car " + i.ToString()
                        }).ToArray()
                    
                },
                WeekendInfo = new SessionData._WeekendInfo
                {
                    TrackDisplayName = "Mount Panorama Circuit",
                    TrackCity = "Bathurst",
                    TrackCountry = "Australia"
                },
                SessionInfo = new SessionData._SessionInfo
                {
                    Sessions = new[] 
                    { 
                        new iRacingSDK.SessionData._SessionInfo._Sessions 
                        {
                             SessionType = "Open Qualify",
                             ResultsPositions = Enumerable.Range(1, 22).Select( i => new iRacingSDK.SessionData._SessionInfo._Sessions._ResultsPositions
                                {
                                     FastestTime = 127.02,
                                     Position = i,
                                     CarIdx = i
                                }).ToArray()
                             
                        }
                    }
                }
            };

            for (var i = 24.5; i < 30.0; i += 1.0 / 59.9)
            {
                if (i >= 20.0 && i < 21.0)
                {
                    result.MessageStates.Add(new OverlayData.MessageState{ Time = 20d, Messages = new[] { "Message1" } });
                }
                else if (i >= 21.0 && i < 23.0)
                {
                    result.MessageStates.Add(new OverlayData.MessageState{Time = 21d, Messages = new[] { "Message1", "Message 2" }});
                }
                else if (i >= 24.0 && i < 25.432454699999997)
                {
                    result.MessageStates.Add(new OverlayData.MessageState{Time = 24d, Messages = new[] { "Message1", "Message 2", "Messages 3" }});
                }
                else if (i >= 25.432454699999997 && i < 27.0)
                {
                    result.MessageStates.Add(new OverlayData.MessageState{Time = 25.432454699999997d, Messages = new[] { "Message1", "Message 2", "Messages 3", "Messages 4" }});
                }
                else if (i >= 27.0 && i < 28.0)
                {
                    result.MessageStates.Add(new OverlayData.MessageState{Time = 27d, Messages = new[] { "Message 2", "Messages 3", "Messages 4", "Message 5" }});
                }
            }

            result.FastestLaps = new List<OverlayData.FastLap>()
                        {
                            new OverlayData.FastLap() 
                            {
                                StartTime = 10,
                                Driver = new OverlayData.Driver { CarNumber = 888, UserName = "Dean Netherton" },
                                Time = TimeSpan.FromSeconds(65.345).TotalSeconds
                            }
                        };

            result.LeaderBoards = new List<OverlayData.LeaderBoard>() 
                        {
                            new OverlayData.LeaderBoard
                            {
                                LapCounter = "Lap 2",
                                StartTime = 0, 
                                Drivers = Enumerable.Range(1, 21).Select( i => 
                                    new OverlayData.Driver { CarNumber = 888, UserName = "Dean Netherton", Position = i, PitStopCount = i == 3 ? 1 : 0}).ToArray(),
                                RacePosition = "39:34",
                                
                            }
                        };

            result.CamDrivers = new List<OverlayData.CamDriver>()
            {
                new OverlayData.CamDriver
                {
                    StartTime = 0,
                    CurrentDriver = new OverlayData.Driver { Position = 13, CarNumber = 888, UserName = "Dean" },
                }
            };
            return result;
        }

        public static void Main(string[] args)
        {
            TestRaceDataOverlay();
            //TestIntroOverlay();
        }

        private static void TestIntroOverlay()
        {
            var leaderboard = new LeaderBoard
            {
                OverlayData = BuildData()
            };

            int i = 1;
            using (var bitmap = (Bitmap)Bitmap.FromFile(@"c:\users\dean\documents\image.bmp"))
            using (var g = Graphics.FromImage(bitmap))
            {
                Console.WriteLine("time is {0}", i);
                leaderboard.Intro(g, i.FromSecondsToNano());
                g.Flush();
                bitmap.Save(@"c:\users\dean\documents\newimage.bmp");
            }
        }

        public static void TestRaceDataOverlay()
        {
            var leaderboard = new LeaderBoard
            {
                OverlayData = BuildData()
            };

            for (var i = 24.5; i < 300.0; i += 5.0 / 59.9)
            {
                using( var bitmap = (Bitmap)Bitmap.FromFile(@"c:\users\dean\documents\image.bmp") )
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        Console.WriteLine("time is {0}", i);
                        leaderboard.Overlay(g, i.FromSecondsToNano());
                        g.Flush();
                        try
                        {
                            bitmap.Save(@"c:\users\dean\documents\newimage.bmp");
                        }
                        catch
                        {

                        }
                    }

                Thread.Sleep(10);
            }
        }
    }
}
