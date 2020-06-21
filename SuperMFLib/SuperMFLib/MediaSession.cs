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
using System.Diagnostics;

namespace MediaFoundation.Net
{
    public class MediaSession : COMDisposable<IMFMediaSession>, IMFAsyncCallback
    {
        Func<MediaEvent, bool> processEvent;

        public MFASync Flags { get; set; }
        public MFAsyncCallbackQueue Queue { get; set; }
        public Action<MediaEvent> OnSessionTopologyStatus { get; set; }

        public MediaSession(IMFMediaSession instance) : base(instance) 
        {
            OnSessionTopologyStatus = me => { };
        }
        
        public MediaSession() : this(CreateInstance()) { }
        
        public static MediaSession Create()
        {
            return new MediaSession(CreateInstance());
        }

        static IMFMediaSession CreateInstance()
        {
            IMFMediaSession instance;
            MFExtern.MFCreateMediaSession(null, out instance).Hr();
            return instance;
        }

        public void Begin()
        {
            this.processEvent = mediaEvent => {

                switch (mediaEvent.EventType)
                {
                    case MediaEventType.MESessionTopologyStatus:
                        OnSessionTopologyStatus(mediaEvent);
                        return true;
                }

                return mediaEvent.EventType != MediaEventType.MESessionClosed;
            };

            instance.BeginGetEvent(this, null).Hr();
        }

        public void Begin(Func<MediaEvent, bool> processEvent)
        {
            this.processEvent = processEvent;
            instance.BeginGetEvent(this, null).Hr();
        }

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = Flags;
            pdwQueue = Queue;

            return S_Ok;
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            try
            {
                GC.Collect();

                IMFMediaEvent mediaEventInstance;
                var h = instance.EndGetEvent(pAsyncResult, out mediaEventInstance);
                h.Hr();

                using (var mediaEvent = new MediaEvent(mediaEventInstance))
                {
                    if (processEvent(mediaEvent))
                        instance.BeginGetEvent(this, null);
                }

                return S_Ok;
            }
            catch( Exception e)
            {
                Trace.WriteLine(e.Message, "ERROR");
                Trace.WriteLine(e.StackTrace, "ERROR");

                return S_False;
            }
        }

        public void SetTopology(Topology topology, MFSessionSetTopologyFlags flags = MFSessionSetTopologyFlags.None)
        {
            instance.SetTopology(flags, topology.instance).Hr();
        }

        public void Start()
        {
            instance.Start(Guid.Empty, new PropVariant()).Hr();
        }
    }
}
