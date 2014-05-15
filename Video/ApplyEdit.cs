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

using MediaFoundation.Net;

namespace iRacingReplayOverlay.Video
{
    public partial class Process
    {
        public static ProcessSample ApplyEdit(long starting, long finishing, ProcessSample beforeEdit, ProcessSample afterEdit)
        {
            bool isBeforeEdit = true;
            long offset = 0;
            long skippingFrom = 0;

            return sample =>
            {
                if (sample.Sample == null)
                    return isBeforeEdit ? beforeEdit(sample) : afterEdit(sample);

                if (sample.Timestamp < starting)
                {
                    if (sample.Timestamp > skippingFrom)
                        skippingFrom = sample.Timestamp;

                    return beforeEdit(sample);
                }

                if (sample.Timestamp > finishing)
                {
                    if (isBeforeEdit)
                    {
                        isBeforeEdit = false;
                        offset = sample.Timestamp - skippingFrom;
                    }

                    if (offset != 0 && !sample.Flags.EndOfStream)
                    {
                        var timestamp = sample.GetSampleTime();
                        sample.SetSampleTime(timestamp - offset);
                    }
                    return afterEdit(sample);
                }

                return true;
            };
        }
    }
}
