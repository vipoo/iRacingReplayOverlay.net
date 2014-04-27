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

using System;
using System.Collections.Generic;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class CommentaryMessages
    {
        List<OverlayData.MessageState> messageStates = new List<OverlayData.MessageState>();

        public void Add(string message, double time)
        {
            OverlayData.MessageState m;

            if( messageStates.Count == 0)
                m = new OverlayData.MessageState { Messages = new[] { message }, Time = time };
         
            else
            {
                var lastMsgs = messageStates.Last().Messages.ToList();
                lastMsgs.Add(message);
                if( lastMsgs.Count == 5)
                    lastMsgs.RemoveAt(0);

                time = Math.Max(messageStates.Last().Time + 1, time);
                m = new OverlayData.MessageState { Messages = lastMsgs.ToArray(), Time = time };
            }

            messageStates.Add(m);
        }

        public OverlayData.MessageState Messages(double atTime)
        {
            return messageStates.LastOrDefault(ms => atTime >= ms.Time);
        }
    }
}
