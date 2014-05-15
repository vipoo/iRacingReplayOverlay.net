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
using System;

namespace iRacingReplayOverlay.Video
{
    public delegate bool ProcessSample<T>(SourceReaderSample sample, T t);

    public partial class Process
    {
        public static ProcessSample Split(long duration, ProcessSample<long> beforeSplit, ProcessSample afterSplit)
        {
            long firstSampleTime = -1;

            return sample =>
            {
                if (firstSampleTime == -1)
                    firstSampleTime = sample.Timestamp;

                if (sample.Timestamp <= firstSampleTime + duration)
                    return beforeSplit(sample, firstSampleTime);

                return afterSplit(sample);
            };
        }

        public static ProcessSample DataSamplesOnly(ProcessSample dataSamples, ProcessSample next)
        {
            return If(s => s.Sample != null, dataSamples, next);
        }

        public static ProcessSample If(Func<SourceReaderSample, bool> selector, ProcessSample trueSamples, ProcessSample falseSamples)
        {
            return sample =>
                {
                    if (selector(sample))
                        return trueSamples(sample);

                    return falseSamples(sample);
                };
        }
    }
}
