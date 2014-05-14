using iRacingReplayOverlay.Phases.Transcoding;
using MediaFoundation.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.Video
{
    public partial class Process
    {
        public static ProcessSample OverlayRaceData(LeaderBoard leaderBoard, ProcessSample next)
        {
            return sample =>
                {
                    if (!sample.Stream.NativeMediaType.IsVideo)
                        return true;

                    var videoSample = new SourceReaderSampleWithBitmap(sample);

                    leaderBoard.Overlay(videoSample.Graphic, sample.Timestamp);

                    //if (frame.Timestamp != 0)
                    //    progress(frame.Timestamp, frame.Duration);

                    return true;
                };
        }
    }
}
