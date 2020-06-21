// This file is part of SuperMFLib.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/SuperMFLib
//
// SuperMFLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SuperMFLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SuperMFLib.  If not, see <http://www.gnu.org/licenses/>.

using System;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using System.Collections.Generic;
using System.Linq;

namespace MediaFoundation.Net
{
    public enum MFSourceReader
    {
        MediaSource = -1
    }

    public delegate bool ProcessSample(SourceReaderSample sample);

    public interface ISourceReader
    {
        long Duration { get; }

        IEnumerable<SourceReaderSample> Samples(int streamIndex = (int)MF_SOURCE_READER.AnyStream, int controlFlags = 0);
        void SetCurrentPosition(long position);
    }

    public class SourceReader : COMDisposable<IMFSourceReader>, ISourceReader
    {
        public SourceReader(IMFSourceReader instance) : base(instance) { }

        public void Samples(ProcessSample samplesFn)
        {
            Samples(samplesFn, (int)MF_SOURCE_READER.AnyStream, 0);
        }

        public void Samples(ProcessSample samplesFn, int streamIndex, int controlFlags)
        {
            foreach (var sample in Samples(streamIndex, controlFlags))
                if (!samplesFn(sample))
                    break;
        }

        static long gcMonitor = 0;

        public IEnumerable<SourceReaderSample> Samples(int streamIndex = (int)MF_SOURCE_READER.AnyStream, int controlFlags = 0)
        {
            var countOfSelectedStreams = Streams.Where(s => s.IsSelected).Count();
            var countOfClosedStreams = 0;

            var sampleCounts = new int[countOfSelectedStreams];
            for (var i = 0; i < countOfSelectedStreams; i++)
                sampleCounts[i] = 0;

            var duration = MediaSource.Duration;

            while (countOfClosedStreams < countOfSelectedStreams)
            {
                int actualStreamIndex;
                int flags;
                long timestamp;
                IMFSample sample;

                this.instance.ReadSample(streamIndex, controlFlags, out actualStreamIndex, out flags, out timestamp, out sample).Hr();

                yield return new SourceReaderSample(
                    this,
                    new SourceStream(this, actualStreamIndex),
                    new SourceReaderSampleFlags(flags),
                    timestamp,
                    (long)duration,
                    sample == null ? null : new Sample(sample),
                    sampleCounts[actualStreamIndex]++
                );

                if (gcMonitor++ % 2000 == 0)
                    System.GC.Collect(10, GCCollectionMode.Forced);

                if ((flags & (int)MF_SOURCE_READER_FLAG.EndOfStream) != 0)
                    countOfClosedStreams++;
            }
        }

        public IEnumerable<SourceStream> Streams
        {
            get
            {
                int i = 0;
                bool ignored;
                while (true)
                {
                    var hr = instance.GetStreamSelection(i, out ignored);
                    if (hr == MFError.MF_E_INVALIDSTREAMNUMBER)
                        yield break;

                    yield return new SourceStream(this, i++);
                }
            }
        }

        public long Duration
        {
            get { return (long)MediaSource.Duration; }
        }

        public SourceStream MediaSource
        {
            get
            {
                return new SourceStream(this, (int)MFSourceReader.MediaSource);
            }
        }


        public object GetPresentationAttribute(MFSourceReader dwStreamIndex, Guid guidAttribute)
        {
            PropVariant result = new PropVariant();
            instance.GetPresentationAttribute((int)dwStreamIndex, guidAttribute, result);
            switch (result.GetVariantType())
            {

                case ConstPropVariant.VariantType.Double:
                    return result.GetDouble();

                case ConstPropVariant.VariantType.UInt32:
                    return result.GetUInt();

                case ConstPropVariant.VariantType.UInt64:
                    return result.GetULong();
            }

            return null;
        }

        public void SetCurrentPosition(long position)
        {
            instance.SetCurrentPosition(Guid.Empty, new PropVariant(position));
        }
    }
}
