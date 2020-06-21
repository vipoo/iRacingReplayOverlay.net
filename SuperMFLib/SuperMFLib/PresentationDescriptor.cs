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

using System.Collections.Generic;

namespace MediaFoundation.Net
{
    public class PresentationDescriptor: COMDisposable<IMFPresentationDescriptor>
    {
        public PresentationDescriptor(IMFPresentationDescriptor instance) : base(instance) { }

        public IEnumerable<PresentationDescriptorStream> Streams
        {
            get
            {
                int sourceStreamCount;
                instance.GetStreamDescriptorCount(out sourceStreamCount).Hr();

                for(var i = 0; i < sourceStreamCount; i++)
                {
                    bool isSelected;
                    IMFStreamDescriptor streamDescriptionInstance;
                    instance.GetStreamDescriptorByIndex(i, out isSelected, out streamDescriptionInstance);
                    yield return new PresentationDescriptorStream(this, i, isSelected, streamDescriptionInstance );
                }
            }
        }
    }
}
