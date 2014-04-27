using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.Phases.Capturing
{


    public class CommentaryMessages
    {
        List<OverlayData.MessageState> messageStates = new List<OverlayData.MessageState>();
        //MessageState messageState = new MessageState { Time = double.MaxValue };

        public void Add(string message, double time)
        {
            if( messageStates.Count == 0)
            {
                var m = new OverlayData.MessageState { Messages = new[] { message }, Time = time };
                messageStates.Add(m);
            }
            else
            {
                var lastMsgs = messageStates.Last().Messages.ToList();
                lastMsgs.Add(message);
                if( lastMsgs.Count == 5)
                    lastMsgs.RemoveAt(0);

                time = Math.Max(messageStates.Last().Time + 1, time);
                var m = new OverlayData.MessageState { Messages = lastMsgs.ToArray(), Time = time };
                messageStates.Add(m);
            }
        }

        public OverlayData.MessageState Messages(double atTime)
        {
            return messageStates.LastOrDefault(ms => atTime >= ms.Time);
        }
    }
}
