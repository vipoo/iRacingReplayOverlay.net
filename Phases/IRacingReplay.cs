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
using iRacingSDK.Support;
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
        bool shortTestOnly;

        public IRacingReplay(bool shortTestOnly = false)
        {
            this.shortTestOnly = shortTestOnly;
        }

        private void Add(Action<Action> action, Action onComplete)
        {
            var context = SynchronizationContext.Current;
            if( context != null)
                actions.Add(() => action(() => context.Post(onComplete)));
            else
                actions.Add(() => action(onComplete));
        }

        private void Add(Action<Action<string, string>> action, Action<string, string> onComplete)
        {
            var context = SynchronizationContext.Current;

            actions.Add(() => action((a, b) => context.Post(() => onComplete(a, b))));
        }

        private void Add(Action<Action<string>> action, Action<string> onComplete)
        {
            var context = SynchronizationContext.Current;

            actions.Add(() => action(a => context.Post(() => onComplete(a))));
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

        public IRacingReplay CaptureOpeningScenes()
        {
            Add(_CaptureOpeningScenes, () => { });

            return this;
        }

        public IRacingReplay WithWorkingFolder(string workingFolder)
        {
            _WithWorkingFolder(workingFolder);
            return this;
        }

        public IRacingReplay CaptureRace(Action<string> onComplete)
        {
            Add((a) => _CaptureRace(a), onComplete);

            return this;
        }

        public IRacingReplay CloseIRacing()
        {
            return this;
        }

        public IRacingReplay WithEncodingOf(int videoBitRate, int audioBitRate)
        {
            _WithEncodingOf(videoBitRate, audioBitRate);
            return this;
        }

        public IRacingReplay WithFiles(string sourceFile)
        {
            _WithFiles(sourceFile);
            return this;
        }

        public IRacingReplay OverlayRaceDataOntoVideo(Action<long, long> progress, Action completed, bool highlightsOnly)
        {
            var context = SynchronizationContext.Current;

            actions.Add(
                () => _OverlayRaceDataOntoVideo(
                    (c, d) => context.Post(() => progress(c, d)),
                    () => context.Post(completed),
                    highlightsOnly
                )
            );
            
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
            requestAbort = true;
        }

        public IRacingReplay InTheBackground(Action<string> onComplete)
        {
            requestAbort = false;
            var context = SynchronizationContext.Current;
            
            backgrounTask = new Task(() => {

                try
                {
                    foreach (var action in actions)
                    {
                        action();
                        if (requestAbort)
                            break;
                    }

                    context.Post(() => onComplete(null));
                }
                catch(Exception e)
                {
                    TraceInfo.WriteLine(e.Message);
                    Trace.WriteLine(e.StackTrace, "DEBUG");
                    TraceInfo.WriteLine("Process aborted");
                    context.Post(() => onComplete("There was an error - details in Log Messages\n{0}".F(e.Message)));
                }
                finally
                {
                    backgrounTask = null;
                    actions = new List<Action>();
                }

            });

            backgrounTask.Start();

            return this;
        }
    }
}
