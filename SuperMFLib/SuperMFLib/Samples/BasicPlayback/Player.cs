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

using MediaFoundation;
using MediaFoundation.Net;
using System;
using System.Linq;

namespace BasicPlayback
{
    class Player : IDisposable
    {
        readonly IntPtr hwndVideo;

        MediaSession mediaSession;
        VideoDisplayControl videoDisplayControl;

        public Player(IntPtr hwndVideo)
        {
            this.hwndVideo = hwndVideo;
        }
        
        public void Dispose()
        {
            if( mediaSession != null)
                mediaSession.Dispose();

            if( videoDisplayControl != null)
                videoDisplayControl.Dispose();
        }

        public void Play(string filename)
        {
            mediaSession = new MediaSession()
            {
                Flags = MFASync.FastIOProcessingCallback,
                Queue = MFAsyncCallbackQueue.Standard,
                OnSessionTopologyStatus = mediaSession_OnSessionTopologyStatus
            };

            mediaSession.Begin();

            using (var sourceResolver = SourceResolver.Create())
            using (var source = sourceResolver.CreateObjectFromURL(filename, MFResolution.MediaSource))
            using (var topology = Topology.Create())
            {
                BuildTopology(source, topology);

                mediaSession.SetTopology(topology);
            }
        }

        void BuildTopology(MediaSource source, Topology topology)
        {
            using (var presentationDescriptor = source.CreatePresentationDescriptor())
                foreach (var stream in presentationDescriptor.Streams.Where(s => s.IsSelected))
                    using (var sourceNode = TopologyNode.Create(MFTopologyType.SourcestreamNode))
                    {
                        if (!stream.MediaType.IsAudio && !stream.MediaType.IsVideo)
                            throw new Exception("Unknown stream format!");

                        sourceNode.TopNodeSource = source;
                        sourceNode.TopNodePresentationDescriptor = presentationDescriptor;
                        sourceNode.TopNodeStreamDescriptor = stream;

                        using (var outputNode = TopologyNode.Create(MFTopologyType.OutputNode))
                        {
                            outputNode.Object = stream.MediaType.IsAudio ? Activate.CreateAudioRenderer() : Activate.CreateVideoRenderer(hwndVideo);

                            topology.Add(sourceNode);
                            topology.Add(outputNode);
                            sourceNode.ConnectOutput(0, outputNode, 0);
                        }
                    }
        }

        void mediaSession_OnSessionTopologyStatus(MediaEvent mediaEvent)
        {
            if (mediaEvent.EventTopologyStatus == MFTopoStatus.Ready)
            {
                videoDisplayControl = VideoDisplayControl.Create(mediaSession);
                mediaSession.Start();
            }
        }

/*        bool ProcessEvent(MediaEvent mediaEvent)
        {
            switch (mediaEvent.EventType)
            {
                case MediaEventType.MESessionTopologyStatus:
                    break;

                case MediaEventType.MESessionStarted:
                    break;

                case MediaEventType.MESessionPaused:
                    break;

                case MediaEventType.MESessionClosed:
                    break;

                case MediaEventType.MEEndOfPresentation:
                    break;

            }

            return mediaEvent.EventType != MediaEventType.MESessionClosed;
        }
        */
        internal void Repaint()
        {
         //   if (videoDisplayControl != null)
           //     videoDisplayControl.RepaintVideo();
        }
    }
}
