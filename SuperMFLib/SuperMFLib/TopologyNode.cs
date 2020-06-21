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


namespace MediaFoundation.Net
{
    public class TopologyNode : COMDisposable<IMFTopologyNode>
    {
        public TopologyNode(IMFTopologyNode instance) : base(instance) { }

        public static TopologyNode Create(MFTopologyType topologyType)
        {
            IMFTopologyNode instance;
            MFExtern.MFCreateTopologyNode(topologyType, out instance);
            return new TopologyNode(instance);
        }

        public MediaSource TopNodeSource
        {
            set
            {
                instance.SetUnknown(MFAttributesClsid.MF_TOPONODE_SOURCE, value.instance).Hr();
            }
        }

        public PresentationDescriptor TopNodePresentationDescriptor
        {
            set
            {
                instance.SetUnknown(MFAttributesClsid.MF_TOPONODE_PRESENTATION_DESCRIPTOR, value.instance).Hr();
            }
        }

        public PresentationDescriptorStream TopNodeStreamDescriptor
        {
            set
            {
                instance.SetUnknown(MFAttributesClsid.MF_TOPONODE_STREAM_DESCRIPTOR, value.instance).Hr();
            }
        }

        public Activate Object
        {
            set
            {
                instance.SetObject(value.instance).Hr();
            }
        }

        public void ConnectOutput(int outputIndex, TopologyNode node, int inputIndexOnDownstreamNode)
        {
            instance.ConnectOutput(outputIndex, node.instance, inputIndexOnDownstreamNode).Hr();
        }
    }
}
