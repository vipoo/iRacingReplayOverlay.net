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
using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class ReplayControl
    {
        readonly IDirectionRule[] directionRules;
        readonly IDirectionRule ruleRandom;
        IDirectionRule currentRule;

        public ReplayControl(SessionData sessionData, Incidents incidents, RemovalEdits removalEdits, TrackCameras trackCameras)
        {
            var cameras = trackCameras.Where(tc => tc.TrackName == sessionData.WeekendInfo.TrackDisplayName).ToArray();

            TraceInfo.WriteLineIf(cameras.Count() <= 0, "Track Cameras not defined for {0}", sessionData.WeekendInfo.TrackDisplayName);
            Debug.Assert(cameras.Count() > 0, "Track Cameras not defined for {0}".F(sessionData.WeekendInfo.TrackDisplayName));

            foreach (var tc in cameras)
                tc.CameraNumber = (short)sessionData.CameraInfo.Groups.First(g => g.GroupName.ToLower() == tc.CameraName.ToLower()).GroupNum;

            var TV3 = cameras.First(tc => tc.CameraName == "TV3");

            iRacing.Replay.CameraOnPositon(1, TV3.CameraNumber);

            var cameraControl = new CameraControl(cameras);

            var battleMarker = removalEdits.For(InterestState.Battle);
            var restartMarker = removalEdits.For(InterestState.Restart);

            var ruleLastSectors = new RuleLastLapPeriod(cameras, removalEdits);
            var ruleFirstSectors = new RuleFirstLapPeriod(cameras, removalEdits);
            var ruleIncident = new RuleIncident(cameras, removalEdits, incidents).WithVeto(ruleLastSectors);
            var rulePaceLaps = new RulePaceLaps(cameras, restartMarker, battleMarker).WithVeto(ruleIncident);
            var ruleBattle = new RuleBattle(cameraControl, battleMarker, Settings.Default.CameraStickyPeriod, Settings.Default.BattleStickyPeriod, Settings.Default.BattleGap, Settings.Default.BattleFactor).WithVeto(rulePaceLaps);
            ruleRandom = new RuleRandomDriver(cameraControl, sessionData, Settings.Default.CameraStickyPeriod).WithVeto(ruleIncident);

            directionRules = new IDirectionRule[] { 
                ruleLastSectors,
                ruleFirstSectors,
                ruleBattle,
                ruleRandom
            };

            currentRule = directionRules[0];
        }

        public void Process(DataSample data)
        {
            if (ActiveRule(currentRule, data))
                return;

            foreach (var rule in directionRules)
                if (ActiveRule(rule, data))
                    return;

            currentRule = ruleRandom;
            currentRule.Direct(data);
        }

        bool ActiveRule(IDirectionRule rule, DataSample data)
        {
            if (rule.IsActive(data))
            {
                currentRule = rule;
                rule.Direct(data);
                return true;
            }

            return false;
        }
    }
}
