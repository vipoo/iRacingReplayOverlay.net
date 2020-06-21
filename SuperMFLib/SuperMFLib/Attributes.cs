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

using MediaFoundation.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaFoundation.Net
{
    public enum MFTranscodeContainer
    {
        Ac3, 
        Adts, 
        Mpeg2, 
        FMpeg4,
        Asf,
        Mpeg4,
        Mp3,
        ThreeGp,
        Wave,
        Avi 
    }

	public class Attributes : COMDisposable<IMFAttributes>
    {
		public Attributes(int initialSize = 1) : base(NewInstance(initialSize))  {  }

		private static IMFAttributes NewInstance(int initialSize)
		{
			IMFAttributes instance;
			MFExtern.MFCreateAttributes(out instance, initialSize).Hr();
			return instance;
		}

        static readonly Dictionary<MFTranscodeContainer, Guid> mapContainerTypeToGuid = new Dictionary<MFTranscodeContainer, Guid>()
        {
            { MFTranscodeContainer.Ac3, MFTranscodeContainerType.AC3 },
            { MFTranscodeContainer.Adts, MFTranscodeContainerType.ADTS },
            { MFTranscodeContainer.Mpeg2, MFTranscodeContainerType.MPEG2 },
            { MFTranscodeContainer.FMpeg4, MFTranscodeContainerType.FMPEG4 },
            { MFTranscodeContainer.Asf, MFTranscodeContainerType.ASF },
            { MFTranscodeContainer.Mpeg4, MFTranscodeContainerType.MPEG4 },
            { MFTranscodeContainer.Mp3, MFTranscodeContainerType.MP3 },
            { MFTranscodeContainer.ThreeGp, MFTranscodeContainerType.x3GP },
            { MFTranscodeContainer.Wave, MFTranscodeContainerType.WAVE },
            { MFTranscodeContainer.Avi, MFTranscodeContainerType.AVI }
        };

        static readonly Dictionary<Guid, MFTranscodeContainer> mapGuidToContainerType = mapContainerTypeToGuid.ToDictionary(kv => kv.Value, kv => kv.Key);

        public MFTranscodeContainer TranscodeContainerType
        {
            get
            {
                Guid value;
                instance.GetGUID(MFAttributesClsid.MF_TRANSCODE_CONTAINERTYPE, out value).Hr();
                return mapGuidToContainerType[value];
            }
            set
            {
                instance.SetGUID(MFAttributesClsid.MF_TRANSCODE_CONTAINERTYPE, mapContainerTypeToGuid[value]).Hr();
            }
        }

        public bool ReadWriterEnableHardwareTransforms
        {
            get
            {
                int result;
                MFError.ThrowExceptionForHR(instance.GetUINT32(MFAttributesClsid.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, out result));
                return result != 0;
            }
            set
            {
                MFError.ThrowExceptionForHR(instance.SetUINT32(MFAttributesClsid.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, value ? 1 : 0));
            }
        }

        public eAVEncH264VProfile H264Profile
        {
            get
            {
                int result;
                instance.GetUINT32(MFAttributesClsid.MF_MT_MPEG2_PROFILE, out result).Hr();
                return (eAVEncH264VProfile)result;
            }
            set
            {
                instance.SetUINT32(MFAttributesClsid.MF_MT_MPEG2_PROFILE, (int)value).Hr();
            }
        }

        public eAVEncH264VLevel Mpeg2Level
        {
            get
            {
                int result;
                instance.GetUINT32(MFAttributesClsid.MF_MT_MPEG2_LEVEL, out result).Hr();
                return (eAVEncH264VLevel)result;
            }
            set
            {
                instance.SetUINT32(MFAttributesClsid.MF_MT_MPEG2_LEVEL, (int)value);
            }
        }

        public int MaxKeyFrameSpacing
        {
            get
            {
                int result;
                instance.GetUINT32(MFAttributesClsid.MF_MT_MAX_KEYFRAME_SPACING, out result).Hr();
                return result;
            }
            set
            {
                instance.SetUINT32(MFAttributesClsid.MF_MT_MAX_KEYFRAME_SPACING, value).Hr();
            }
        }

        public bool SourceReaderEnableVideoProcessing
        {
            get
            {
                int result;
                instance.GetUINT32(MFAttributesClsid.MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, out result).Hr();
                return result != 0;
            }
            set
            {
                instance.SetUINT32(MFAttributesClsid.MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, value ? 1 : 0).Hr();
            }
        }
    }
}
