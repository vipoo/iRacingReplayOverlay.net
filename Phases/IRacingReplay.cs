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

using iRacingSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        List<Action<bool?>> actions = new List<Action<bool?>>();

        public IRacingReplay WhenIRacingStarts()
        {
            actions.Add(_WhenIRacingStarts);

            return this;
        }

        public IRacingReplay AnalyseRace()
        {
            actions.Add(_AnalyseRace);
            
            return this;
        }

        public IRacingReplay CaptureRace(string workingFolder)
        {
            actions.Add(ra => _CaptureRace(workingFolder));

            return this;
        }

        public IRacingReplay CloseIRacing()
        {
            return this;
        }

        public IRacingReplay OverlayRaceDataOntoVideo(Action<int> progressFunction)
        {
            return this;
        }

        public void InTheForeground()
        {
            foreach (var action in actions)
                action(false);
        }

        public void InTheBackground(bool? requestAbort)
        {
            var t = new Task(() => {
            
                foreach( var action in actions)
                {
                    action(requestAbort);
                    if( requestAbort.Value)
                        return;
                }

            });
        }
    }
}
