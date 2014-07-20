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

using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.Support
{
    public class SimpleProfiler
    {
        private string name;
        DateTime start;

        private static Dictionary<string, double> recordings = new Dictionary<string, double>();
        private static Dictionary<string, int> counts = new Dictionary<string, int>();

        public SimpleProfiler(string name)
        {
            this.name = name;
            this.start = DateTime.Now;
        }

        public static void Log()
        {
            foreach (var kv in recordings)
                Trace.WriteLine("{0} - {1}, {2}, {3}".F(kv.Key, kv.Value * 1000, counts[kv.Key], kv.Value * 1000 / counts[kv.Key]));
        }

        public static SimpleProfiler StartFor(string name)
        {
            return new SimpleProfiler(name);
        }

        public void Start(string name)
        {
            End();
            this.name = name;
            this.start = DateTime.Now;
        }

        public void End()
        {
            var duration = (DateTime.Now - start).TotalMilliseconds;

            if (recordings.ContainsKey(name))
            {
                recordings[name] += duration;
                counts[name]++;
            }
            else
            {
                counts.Add(name, 1);
                recordings.Add(name, duration);
            }
        }
    }
}
