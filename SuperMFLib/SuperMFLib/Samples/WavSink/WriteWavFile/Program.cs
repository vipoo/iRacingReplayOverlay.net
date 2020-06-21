/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released
as public domain.  It is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
*****************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.Alt;

namespace WriteWavFile
{
    class Program
    {
        ///////////////////////////////////////////////////////////////////////
        //  Name: wmain
        //  Description:  Entry point to the application.
        //  
        //  Usage: writewavfile.exe inputfile outputfile
        ///////////////////////////////////////////////////////////////////////

        static void Main(string[] args)
        {
            int hr;
            if (args.Length == 2)
            {
                hr = MFExtern.MFStartup(0x10070, MFStartup.Full);
                MFError.ThrowExceptionForHR(hr);

                try
                {
                    CreateWavFile(args[0], args[1]);
                    Console.WriteLine("Complete!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    hr = MFExtern.MFShutdown();
                    MFError.ThrowExceptionForHR(hr);
                }
            }
            else
            {
                Console.WriteLine("Usage: WriteWavFile.exe InputFile OuputFile");
            }
        }

        ///////////////////////////////////////////////////////////////////////
        //  Name: CreateWavFile
        //  Description:  Creates a .wav file from an input file.
        ///////////////////////////////////////////////////////////////////////

        static void CreateWavFile(string sURL, string sOutputFile)
        {
            IMFByteStream pStream = null;
            IMFMediaSinkAlt pSink = null;
            IMFMediaSource pSource = null;
            IMFTopology pTopology = null;
            WavSinkNS.CWavSink pObj = null;

            int hr = MFExtern.MFCreateFile(MFFileAccessMode.Write, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, sOutputFile, out pStream);
            MFError.ThrowExceptionForHR(hr);

            try
            {
                pObj = new WavSinkNS.CWavSink(pStream);
                pSink = pObj as IMFMediaSinkAlt;

                // Create the media source from the URL.
                CreateMediaSource(sURL, out pSource);

                // Create the topology.
                CreateTopology(pSource, pSink, out pTopology);

                // Run the media session.
                RunMediaSession(pTopology);

                hr = pSource.Shutdown();
                MFError.ThrowExceptionForHR(hr);
            }
            finally
            {
                if (pStream != null)
                {
                    Marshal.ReleaseComObject(pStream);
                }
                if (pSource != null)
                {
                    Marshal.ReleaseComObject(pSource);
                }
                if (pTopology != null)
                {
                    Marshal.ReleaseComObject(pTopology);
                }
                //pObj.Dispose();
            }
        }

        ///////////////////////////////////////////////////////////////////////
        //  Name: RunMediaSession
        //  Description:  
        //  Queues the specified topology on the media session and runs the
        //  media session until the MESessionEnded event is received.
        ///////////////////////////////////////////////////////////////////////

        static void RunMediaSession(IMFTopology pTopology)
        {
            int hr;
            IMFMediaSession pSession;

            bool bGetAnotherEvent = true;
            PropVariant varStartPosition = new PropVariant();

            hr = MFExtern.MFCreateMediaSession(null, out pSession);
            MFError.ThrowExceptionForHR(hr);

            try
            {
                hr = pSession.SetTopology(0, pTopology);
                MFError.ThrowExceptionForHR(hr);

                while (bGetAnotherEvent)
                {
                    int hrStatus = 0;
                    IMFMediaEvent pEvent;
                    MediaEventType meType = MediaEventType.MEUnknown;

                    int TopoStatus = (int)MFTopoStatus.Invalid; // Used with MESessionTopologyStatus event.    

                    hr = pSession.GetEvent(MFEventFlag.None, out pEvent);
                    MFError.ThrowExceptionForHR(hr);

                    try
                    {
                        hr = pEvent.GetStatus(out hrStatus);
                        MFError.ThrowExceptionForHR(hr);

                        hr = pEvent.GetType(out meType);
                        MFError.ThrowExceptionForHR(hr);

                        if (hrStatus >= 0)
                        {
                            switch (meType)
                            {
                                case MediaEventType.MESessionTopologySet:
                                    Debug.WriteLine("MESessionTopologySet");
                                    break;

                                case MediaEventType.MESessionTopologyStatus:
                                    // Get the status code.
                                    hr = pEvent.GetUINT32(MFAttributesClsid.MF_EVENT_TOPOLOGY_STATUS, out TopoStatus);
                                    MFError.ThrowExceptionForHR(hr);
                                    switch ((MFTopoStatus)TopoStatus)
                                    {
                                        case MFTopoStatus.Ready:
                                            Debug.WriteLine("MESessionTopologyStatus: MF_TOPOSTATUS_READY");
                                            pSession.Start(Guid.Empty, varStartPosition);
                                            break;

                                        case MFTopoStatus.Ended:
                                            Debug.WriteLine("MESessionTopologyStatus: MF_TOPOSTATUS_ENDED");
                                            break;
                                    }
                                    break;

                                case MediaEventType.MESessionStarted:
                                    Debug.WriteLine("MESessionStarted");
                                    break;

                                case MediaEventType.MESessionEnded:
                                    Debug.WriteLine("MESessionEnded");
                                    hr = pSession.Stop();
                                    break;

                                case MediaEventType.MESessionStopped:
                                    Debug.WriteLine("MESessionStopped");
                                    Console.WriteLine("Attempting to close the media session.");
                                    hr = pSession.Close();
                                    MFError.ThrowExceptionForHR(hr);
                                    break;

                                case MediaEventType.MESessionClosed:
                                    Debug.WriteLine("MESessionClosed");
                                    bGetAnotherEvent = false;
                                    break;

                                default:
                                    Debug.WriteLine(string.Format("Media session event: {0}", meType));
                                    break;
                            }
                        }
                        else
                        {
                            Debug.WriteLine(string.Format("HRStatus: {0}", hrStatus));
                            bGetAnotherEvent = false;
                        }
                    }
                    finally
                    {
                        if (pEvent != null)
                        {
                            Marshal.ReleaseComObject(pEvent);
                        }
                    }
                }

                Debug.WriteLine("Shutting down the media session.");
                hr = pSession.Shutdown();
                MFError.ThrowExceptionForHR(hr);
            }
            finally
            {
                if (pSession != null)
                {
                    Marshal.ReleaseComObject(pSession);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        //  Name: CreateMediaSource
        //  Description:  Create a media source from a URL.
        //
        //  sURL: The URL to open.
        //  ppSource: Receives a pointer to the media source.
        ///////////////////////////////////////////////////////////////////////

        static void CreateMediaSource(string sURL, out IMFMediaSource ppSource)
        {
            int hr;
            IMFSourceResolver pSourceResolver;
            object pSourceUnk;

            // Create the source resolver.
            hr = MFExtern.MFCreateSourceResolver(out pSourceResolver);
            MFError.ThrowExceptionForHR(hr);

            try
            {
                // Use the source resolver to create the media source.
                MFObjectType ObjectType = MFObjectType.Invalid;
                hr = pSourceResolver.CreateObjectFromURL(
                        sURL,                      // URL of the source.
                        MFResolution.MediaSource, // Create a source object.
                        null,                      // Optional property store.
                        out ObjectType,               // Receives the object type. 
                        out pSourceUnk   // Receives a pointer to the source.
                    );
                MFError.ThrowExceptionForHR(hr);

                // Get the IMFMediaSource interface from the media source.
                ppSource = (IMFMediaSource)pSourceUnk;
            }
            finally
            {
                // Clean up.
                if (pSourceResolver != null)
                {
                    Marshal.ReleaseComObject(pSourceResolver);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        //  Name: CreateTopology
        //  Description:  Creates the topology.
        // 
        //  Note: The first audio stream is conntected to the media sink.
        //        Other streams are deselected.
        ///////////////////////////////////////////////////////////////////////

        static void CreateTopology(IMFMediaSource pSource, IMFMediaSinkAlt pSink, out IMFTopology ppTopology)
        {
            int hr;
            IMFPresentationDescriptor pPD = null;
            IMFStreamDescriptor pSD = null;

            int cStreams = 0;
            bool fConnected = false;

            hr = MFExtern.MFCreateTopology(out ppTopology);
            MFError.ThrowExceptionForHR(hr);

            hr = pSource.CreatePresentationDescriptor(out pPD);
            MFError.ThrowExceptionForHR(hr);

            try
            {
                hr = pPD.GetStreamDescriptorCount(out cStreams);
                MFError.ThrowExceptionForHR(hr);

                Guid majorType;
                bool fSelected = false;

                for (int iStream = 0; iStream < cStreams; iStream++)
                {
                    hr = pPD.GetStreamDescriptorByIndex(iStream, out fSelected, out pSD);
                    MFError.ThrowExceptionForHR(hr);

                    try
                    {
                        // If the stream is not selected by default, ignore it.
                        if (!fSelected)
                        {
                            continue;
                        }

                        // Get the major media type.
                        GetStreamMajorType(pSD, out majorType);

                        // If it's not audio, deselect it and continue.
                        if (majorType != MFMediaType.Audio)
                        {
                            // Deselect this stream
                            hr = pPD.DeselectStream(iStream);
                            MFError.ThrowExceptionForHR(hr);

                            continue;
                        }

                        // It's an audio stream, so try to create the topology branch.
                        CreateTopologyBranch(ppTopology, pSource, pPD, pSD, pSink);
                    }
                    finally
                    {
                        if (pSD != null)
                        {
                            Marshal.ReleaseComObject(pSD);
                        }
                    }

                    // Set our status flag. 
                    fConnected = true;

                    // At this point we have reached the first audio stream in the
                    // source, so we can stop looking (whether we succeeded or failed).
                    break;
                }
            }
            finally
            {
                if (pPD != null)
                {
                    Marshal.ReleaseComObject(pPD);
                }
            }

            // Even if we succeeded, if we didn't connect any streams, it's a failure.
            // (For example, it might be a video-only source.
            if (!fConnected)
            {
                throw new Exception("No audio streams");
            }
        }

        //////////////////////////////////////////////////////////////////////
        //  Name: CreateSourceNode
        //  Creates a source node for a media stream. 
        //
        //  pSource:   Pointer to the media source.
        //  pSourcePD: Pointer to the source's presentation descriptor.
        //  pSourceSD: Pointer to the stream descriptor.
        //  ppNode:    Receives the IMFTopologyNode pointer.
        ///////////////////////////////////////////////////////////////////////

        static void CreateSourceNode(
            IMFMediaSource pSource,          // Media source.
            IMFPresentationDescriptor pPD,   // Presentation descriptor.
            IMFStreamDescriptor pSD,         // Stream descriptor.
            out IMFTopologyNode ppNode          // Receives the node pointer.
            )
        {
            int hr;

            // Create the node.
            hr = MFExtern.MFCreateTopologyNode(
                MFTopologyType.SourcestreamNode,
                out ppNode);
            MFError.ThrowExceptionForHR(hr);

            // Set the attributes.
            hr = ppNode.SetUnknown(
                MFAttributesClsid.MF_TOPONODE_SOURCE,
                pSource);
            MFError.ThrowExceptionForHR(hr);

            hr = ppNode.SetUnknown(
                MFAttributesClsid.MF_TOPONODE_PRESENTATION_DESCRIPTOR,
                pPD);
            MFError.ThrowExceptionForHR(hr);

            hr = ppNode.SetUnknown(
                MFAttributesClsid.MF_TOPONODE_STREAM_DESCRIPTOR,
                pSD);
            MFError.ThrowExceptionForHR(hr);
        }

        ///////////////////////////////////////////////////////////////////////
        //  Name: CreateTopologyBranch
        //  Description:  Adds a source and sink to the topology and
        //                connects them.
        //
        //  pTopology: The topology.
        //  pSource:   The media source.
        //  pPD:       The source's presentation descriptor.
        //  pSD:       The stream descriptor for the stream.
        //  pSink:     The media sink.
        //
        ///////////////////////////////////////////////////////////////////////

        static void CreateTopologyBranch(
            IMFTopology pTopology,
            IMFMediaSource pSource,          // Media source.
            IMFPresentationDescriptor pPD,   // Presentation descriptor.
            IMFStreamDescriptor pSD,         // Stream descriptor.
            IMFMediaSinkAlt pSink
            )
        {
            int hr;
            IMFTopologyNode pSourceNode = null;
            IMFTopologyNode pOutputNode = null;

            CreateSourceNode(pSource, pPD, pSD, out pSourceNode);

            try
            {
                CreateOutputNode(pSink, 0, out pOutputNode);

                try
                {
                    hr = pTopology.AddNode(pSourceNode);
                    MFError.ThrowExceptionForHR(hr);

                    hr = pTopology.AddNode(pOutputNode);
                    MFError.ThrowExceptionForHR(hr);

                    hr = pSourceNode.ConnectOutput(0, pOutputNode, 0);
                    MFError.ThrowExceptionForHR(hr);
                }
                finally
                {
                    if (pOutputNode != null)
                    {
                        Marshal.ReleaseComObject(pOutputNode);
                    }
                }
            }
            finally
            {
                if (pSourceNode != null)
                {
                    Marshal.ReleaseComObject(pSourceNode);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        //  Name: CreateOutputNode
        //  Description:  Creates an output node for a stream sink.
        //
        //  pSink:     The media sink.
        //  iStream:   Index of the stream sink on the media sink.
        //  ppNode:    Receives a pointer to the topology node.
        ///////////////////////////////////////////////////////////////////////

        static void CreateOutputNode(IMFMediaSinkAlt pSink, int iStream, out IMFTopologyNode ppNode)
        {
            int hr;
            IMFStreamSinkAlt pStream = null;

            hr = pSink.GetStreamSinkByIndex(iStream, out pStream);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateTopologyNode(MFTopologyType.OutputNode, out ppNode);
            MFError.ThrowExceptionForHR(hr);

            hr = ppNode.SetObject(pStream);
            MFError.ThrowExceptionForHR(hr);

            //Marshal.ReleaseComObject(pStream);
        }

        ///////////////////////////////////////////////////////////////////////
        //  Name: GetStreamMajorType
        //  Description:  Returns the major media type for a stream.
        ///////////////////////////////////////////////////////////////////////

        static void GetStreamMajorType(IMFStreamDescriptor pSD, out Guid pMajorType)
        {
            int hr;
            IMFMediaTypeHandler pHandler;

            hr = pSD.GetMediaTypeHandler(out pHandler);
            MFError.ThrowExceptionForHR(hr);

            try
            {
                hr = pHandler.GetMajorType(out pMajorType);
                MFError.ThrowExceptionForHR(hr);
            }
            finally
            {
                if (pHandler != null)
                {
                    Marshal.ReleaseComObject(pHandler);
                }
            }
        }
    }
}
