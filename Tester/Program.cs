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

using iRacingReplayOverlay.Phases.Analysis;
using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Phases.Direction;
using IRacingReplayOverlay.Phases;
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            bool? requestAbort = false;

            var workingFolder = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            new IRacingReplay()
                .WhenIRacingStarts(() => { })
                .AnalyseRace(() => { })
                .CaptureRace(workingFolder, (f, e) => { } )
                .CloseIRacing()
                .OverlayRaceDataOntoVideo(progess => { /*update progress bar */})
                .InTheForeground();

                //.InTheBackground(ref requestAbort)
        }
    }
}
