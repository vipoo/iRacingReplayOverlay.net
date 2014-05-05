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

using MediaFoundation;
using MediaFoundation.Net;
using MediaFoundation.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using iRacingReplayOverlay.Support;
using System.Drawing;
using System.Diagnostics;
using iRacingReplayOverlay.Phases.Capturing;

namespace iRacingReplayOverlay.Phases.Transcoding
{
    public static class SourceReaderExtension
    {
        public static IEnumerable<SourceReaderSample> SamplesAfterEditing(this SourceReader sourceReader, List<OverlayData.BoringBit> edits)
        {
            long skippingFrom = 0;
            bool justSkipped = false;
            Capturing.OverlayData.BoringBit currentSkip = null;
            bool isFadingOut = false;
            long fadingOutFrom = 0;
            var isFadingIn = false;
            long fadingInFrom = 0;
            long offset = 0;

            var nextCut = edits.GetEnumerator();
            nextCut.MoveNext();

            foreach (var sample in sourceReader.Samples())
            {
                if (offset != 0 && !sample.Flags.EndOfStream)
                    sample.SetSampleTime(sample.Timestamp - offset);

                if (sample.Flags.EndOfStream)
                {
                    yield return sample;
                    continue;
                }

                if (justSkipped)
                {
                    if (sample.Timestamp + 0.5.FromSecondsToNano() < currentSkip.EndTime.FromSecondsToNano())
                        continue;
                }

                if (sample.Stream.CurrentMediaType.IsAudio)
                {
                    if (isFadingOut)
                    {
                        var fadeOut = 1.0 - (sample.Timestamp - fadingOutFrom).FromNanoToSeconds();
                        FadeAudio(sample, fadeOut);
                    }

                    if(isFadingIn)
                    {
                        var fadeOut = (sample.Timestamp - fadingInFrom).FromNanoToSeconds();
                        FadeAudio(sample, fadeOut);
                    }
                    yield return sample;
                    continue;
                }

                
                if (isFadingIn)
                {
                    var fadeIn = Math.Min(255, 255 - ((sample.Timestamp - fadingInFrom).FromNanoToSeconds() * 255));
                    fadeIn = Math.Max(0, fadeIn);

                    using(var videoSample = new SourceReaderSampleWithBitmap(sample))
                        videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeIn, Color.Black)), 0, 0, 1920, 1080);

                    if (sample.Timestamp > fadingInFrom + 1.FromSecondsToNano())
                    {
                        isFadingIn = false;
                        Trace.WriteLine("Finishing fading in at {0}".F(TimeSpan.FromSeconds(sample.Timestamp)));
                    }
                }

                if (isFadingOut)
                {
                    var fadeOut = Math.Min(255, (sample.Timestamp - fadingOutFrom).FromNanoToSeconds() * 255);
                    fadeOut = Math.Max(0, fadeOut);

                    using (var videoSample = new SourceReaderSampleWithBitmap(sample))
                        videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeOut, Color.Black)), 0, 0, 1920, 1080);

                    if (fadingOutFrom + 1.FromSecondsToNano() < sample.Timestamp)
                    {
                        Trace.WriteLine("Finished fading out at {0}".F(TimeSpan.FromSeconds(sample.Timestamp.FromNanoToSeconds())));
                        isFadingOut = false;
                        currentSkip = nextCut.Current;
                        skippingFrom = sample.Timestamp;
                        justSkipped = true;
                        nextCut.MoveNext();

                        Trace.WriteLine("Skipping to {0}".F(TimeSpan.FromSeconds(currentSkip.EndTime)));

                        sourceReader.SetCurrentPosition(currentSkip.EndTime.FromSecondsToNano());
                        continue;
                    }
                }
                else
                {
                    if (nextCut.Current != null && sample.Timestamp >= nextCut.Current.StartTime.FromSecondsToNano())
                    {
                        Trace.WriteLine("Starting fading out at {0}".F(TimeSpan.FromSeconds(sample.Timestamp.FromNanoToSeconds())));

                        isFadingOut = true;
                        fadingOutFrom = sample.Timestamp;
                    }
                }
                if (justSkipped)
                {
                    justSkipped = false;
                    offset += (sample.Timestamp - skippingFrom);
                    isFadingIn = true;
                    fadingInFrom = sample.Timestamp;

                    sample.SetSampleTime(sample.Timestamp - offset);

                    Trace.WriteLine("Adjusted timestamp is {0}".F(TimeSpan.FromSeconds(sample.Timestamp.FromNanoToSeconds())));
                    Trace.WriteLine("Offset is now {0}".F(TimeSpan.FromSeconds(offset.FromNanoToSeconds())));
                }

                yield return sample;
            }
        }

        static void FadeAudio(SourceReaderSample sample, double fadeout)
        {
            var buffer = sample.Sample.ConvertToContiguousBuffer();
            var data = buffer.Lock();

            fadeout = Math.Min(1, fadeout);
            fadeout = Math.Max(0, fadeout);

            Trace.WriteLine("Fade {0}".F(fadeout));

            unsafe
            {
                var pData = (short*)data.Buffer.ToPointer();

                int length;
                buffer.instance.GetMaxLength(out length);

                for (int i = 0; i < length / 2; i++)
                    pData[i] = (short)((double)pData[i] * fadeout);
            }

            data.Dispose();
            buffer.Dispose();
        }
    }   
}
