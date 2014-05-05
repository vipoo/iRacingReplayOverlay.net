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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.Phases
{
    public static class SynchronizationContextExtension
    {
        public static void Post(this SynchronizationContext self, Action action)
        {
            self.Post((ignored) => action(), null);
        }
    }

    public partial class IRacingReplay
    {
        List<Action> actions = new List<Action>();

        private void Add(Action<Action> action, Action onComplete)
        {
            var context = SynchronizationContext.Current;

            actions.Add(() => action(() => context.Post(onComplete)));
        }

        private void Add(Action<Action<string, string>> action, Action<string, string> onComplete)
        {
            var context = SynchronizationContext.Current;

            actions.Add(() => action((a, b) => context.Post(() => onComplete(a, b))));
        }

        public IRacingReplay WhenIRacingStarts(Action onComplete)
        {
            Add(_WhenIRacingStarts, onComplete);

            return this;
        }

        public IRacingReplay AnalyseRace(Action onComplete)
        {
            Add(_AnalyseRace,  onComplete);
            
            return this;
        }

        public IRacingReplay CaptureRace(string workingFolder, Action<string, string> onComplete)
        {
            Add((a) => _CaptureRace(workingFolder, a), onComplete);

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
                action();
        }

        bool requestAbort = false;
        Task backgrounTask = null;

        public void RequestAbort()
        {

        }

        public IRacingReplay InTheBackground(Action onComplete)
        {
            var context = SynchronizationContext.Current;
            
            backgrounTask = new Task(() => {

                try
                {
                    foreach (var action in actions)
                    {
                        action();
                        if (requestAbort)
                            return;
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.StackTrace);
                    Debug.WriteLine("Process aborted");
                }
                finally
                {
                    context.Post(onComplete);
                    backgrounTask = null;
                    actions = new List<Action>();
                }

            });

            backgrounTask.Start();

            return this;
        }
    }
}
