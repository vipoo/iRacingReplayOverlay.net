/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released
as public domain.  It is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
*****************************************************************************/

// C:\Windows\Microsoft.NET\Framework64\v2.0.50727\regasm /codebase WavSink.dll

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;

using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.Alt;

namespace WavSinkNS
{
    [ComVisible(true), System.Security.SuppressUnmanagedCodeSecurity,
    Guid("6239901C-3FFD-4be1-92CE-303FE0D253EE"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCustom
    {
        [PreserveSig]
        int SetStream(object pStream);
    }

    // PCM_Audio_Format_Params
    // Defines parameters for uncompressed PCM audio formats.
    // The remaining fields can be derived from these.
    internal class PCM_Audio_Format_Params
    {
        public int nSamplesPerSec; // Samples per second.
        public short wBitsPerSample; // Bits per sample.
        public short nChannels;      // Number of channels.

        public PCM_Audio_Format_Params(int sps, short bps, short c)
        {
            nSamplesPerSec = sps;
            wBitsPerSample = bps;
            nChannels = c;
        }
    }

    /// <summary>
    /// This class can be invoked in 1 of 2 ways.  If you are calling it from managed code, simply use
    /// the assembly, and do a "new CWavSink(pStream)".  If you are calling it from c++, you can use
    /// CoCreateInstance to create an instance of the class, then call IMFCustom::SetStream to provide
    /// the stream.
    /// </summary>
    [ComVisible(true),
    Guid("57FF2C99-9AA8-41da-93DC-FE9B6B97029A"),
    ClassInterface(ClassInterfaceType.None)]
    public class CWavSink : COMBase, IMFFinalizableMediaSinkAlt, IMFClockStateSink, IDisposable, IMFCustom
    {
        // The stream ID of the one stream on the sink.
        public const int WAV_SINK_STREAM_ID = 1;

        #region Members

        // This is an ordered list that we use to hand out formats in the 
        // stream's IMFMediaTypeHandler::GetMediaTypeByIndex method. The 
        // stream will accept other bit rates not listed here.

        internal static PCM_Audio_Format_Params[] g_AudioFormats;

        bool m_IsShutdown;               // Flag to indicate if Shutdown() method was called.

        CWavStream m_pStream;                 // Byte stream
        IMFPresentationClock m_pClock;                  // Presentation clock.

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////
        //
        // CWavSink class. - Implements the media sink.
        //
        // Notes:
        // - Most public methods calls CheckShutdown. This method fails if the sink was shut down.
        //
        /////////////////////////////////////////////////////////////////////////////////////////////


        //-------------------------------------------------------------------
        // CWavSink constructor.
        //-------------------------------------------------------------------

        public CWavSink()
        {
            TRACE("CWavSink::CWavSink");
            Init();
        }

        private void Init()
        {
            g_AudioFormats = new PCM_Audio_Format_Params[12];

            g_AudioFormats[0] = new PCM_Audio_Format_Params(48000, 16, 2);
            g_AudioFormats[1] = new PCM_Audio_Format_Params(48000, 8, 2);
            g_AudioFormats[2] = new PCM_Audio_Format_Params(44100, 16, 2);
            g_AudioFormats[3] = new PCM_Audio_Format_Params(44100, 8, 2);
            g_AudioFormats[4] = new PCM_Audio_Format_Params(22050, 16, 2);
            g_AudioFormats[5] = new PCM_Audio_Format_Params(22050, 8, 2);

            g_AudioFormats[6] = new PCM_Audio_Format_Params(48000, 16, 1);
            g_AudioFormats[7] = new PCM_Audio_Format_Params(48000, 8, 1);
            g_AudioFormats[8] = new PCM_Audio_Format_Params(44100, 16, 1);
            g_AudioFormats[9] = new PCM_Audio_Format_Params(44100, 8, 1);
            g_AudioFormats[10] = new PCM_Audio_Format_Params(22050, 16, 1);
            g_AudioFormats[11] = new PCM_Audio_Format_Params(22050, 8, 1);

            m_IsShutdown = false;
        }

        public CWavSink(IMFByteStream pStream)
        {
            TRACE("CWavSink::CWavSink(IMFByteStream)");

            if (pStream != null)
            {
                Init();
                m_pStream = new CWavStream(this, (IMFByteStream)pStream);
            }
            else
            {
                throw new COMException("Null pstream", E_Pointer);
            }
        }

        //-------------------------------------------------------------------
        // CWavSink destructor.
        //-------------------------------------------------------------------

#if DEBUG

        ~CWavSink()
        {
            // Most cleanup happens in Shutdown

            TRACE("CWavSink::~CWavSink");
            Debug.Assert(m_IsShutdown);
        }

#endif

        #region IMFMediaSink methods.

        //-------------------------------------------------------------------
        // Name: GetCharacteristics 
        // Description: Returns the characteristics flags. 
        //
        // Note: This sink has a fixed number of streams and is rateless.
        //-------------------------------------------------------------------

        public int GetCharacteristics(out MFMediaSinkCharacteristics pdwCharacteristics)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::GetCharacteristics");
                lock (this)
                {
                    CheckShutdown();

                    pdwCharacteristics = MFMediaSinkCharacteristics.FixedStreams | MFMediaSinkCharacteristics.Rateless;
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                pdwCharacteristics = MFMediaSinkCharacteristics.None;
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: AddStreamSink 
        // Description: Adds a new stream to the sink. 
        //
        // Note: This sink has a fixed number of streams, so this method
        //       always returns MF_E_STREAMSINKS_FIXED.
        //-------------------------------------------------------------------

        public int AddStreamSink(
            int dwStreamSinkIdentifier,
            IMFMediaType pMediaType,
            out IMFStreamSinkAlt ppStreamSink)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::AddStreamSink");
                throw new COMException("Fixed streams", MFError.MF_E_STREAMSINKS_FIXED);
            }
            catch (Exception e)
            {
                ppStreamSink = null;
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: RemoveStreamSink 
        // Description: Removes a stream from the sink. 
        //
        // Note: This sink has a fixed number of streams, so this method
        //       always returns MF_E_STREAMSINKS_FIXED.
        //-------------------------------------------------------------------

        public int RemoveStreamSink(int dwStreamSinkIdentifier)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::RemoveStreamSink");
                throw new COMException("Fixed streams", MFError.MF_E_STREAMSINKS_FIXED);
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: GetStreamSinkCount 
        // Description: Returns the number of streams. 
        //-------------------------------------------------------------------

        public int GetStreamSinkCount(out int pcStreamSinkCount)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::GetStreamSinkCount");
                lock (this)
                {
                    CheckShutdown();

                    pcStreamSinkCount = 1;  // Fixed number of streams.
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                pcStreamSinkCount = 0;
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: GetStreamSinkByIndex 
        // Description: Retrieves a stream by index. 
        //-------------------------------------------------------------------

        public int GetStreamSinkByIndex(
            int dwIndex,
            out IMFStreamSinkAlt ppStreamSink)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::GetStreamSinkByIndex");
                lock (this)
                {
                    // Fixed stream: Index 0. 
                    if (dwIndex > 0)
                    {
                        throw new COMException("Invalid index", MFError.MF_E_INVALIDINDEX);
                    }

                    CheckShutdown();

                    ppStreamSink = m_pStream;
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                ppStreamSink = null;
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: GetStreamSinkById 
        // Description: Retrieves a stream by ID. 
        //-------------------------------------------------------------------

        public int GetStreamSinkById(
            int dwStreamSinkIdentifier,
            out IMFStreamSinkAlt ppStreamSink)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::GetStreamSinkById");
                lock (this)
                {
                    // Fixed stream ID.
                    if (dwStreamSinkIdentifier != WAV_SINK_STREAM_ID)
                    {
                        throw new COMException("Stream id not valid", MFError.MF_E_INVALIDSTREAMNUMBER);
                    }

                    CheckShutdown();

                    ppStreamSink = m_pStream;
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                ppStreamSink = null;
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: SetPresentationClock 
        // Description: Sets the presentation clock. 
        //
        // pPresentationClock: Pointer to the clock. Can be null.
        //-------------------------------------------------------------------

        public int SetPresentationClock(IMFPresentationClock pPresentationClock)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                int hr;
                TRACE("CWavSink::SetPresentationClock");
                lock (this)
                {
                    CheckShutdown();

                    // If we already have a clock, remove ourselves from that clock's
                    // state notifications.

                    if (m_pClock != pPresentationClock)
                    {
                        if (m_pClock != null)
                        {
                            hr = m_pClock.RemoveClockStateSink(this);
                            MFError.ThrowExceptionForHR(hr);
                        }

                        // Register ourselves to get state notifications from the new clock.
                        if (pPresentationClock != null)
                        {
                            hr = pPresentationClock.AddClockStateSink(this);
                            MFError.ThrowExceptionForHR(hr);
                        }

                        // Release the pointer to the old clock.
                        // Store the pointer to the new clock.

                        //SAFE_RELEASE(m_pClock);
                        m_pClock = pPresentationClock;
                    }
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: GetPresentationClock 
        // Description: Returns a pointer to the presentation clock. 
        //-------------------------------------------------------------------

        public int GetPresentationClock(out IMFPresentationClock ppPresentationClock)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::GetPresentationClock");
                lock (this)
                {
                    CheckShutdown();

                    if (m_pClock == null)
                    {
                        throw new COMException("There is no presentation clock.", MFError.MF_E_NO_CLOCK);
                    }
                    else
                    {
                        // Return the pointer to the caller.
                        ppPresentationClock = m_pClock;
                    }
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                ppPresentationClock = null;
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: Shutdown 
        // Description: Releases resources held by the media sink. 
        //-------------------------------------------------------------------

        public int Shutdown()
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::Shutdown");
                GC.SuppressFinalize(this);

                lock (this)
                {
                    try
                    {
                        CheckShutdown();

                        m_pStream.Shutdown();
                    }
                    finally
                    {
                        m_IsShutdown = true;

                        SafeRelease(m_pClock);
                        m_pClock = null;

                        if (m_pStream != null)
                        {
                            m_pStream.Dispose();
                            m_pStream = null;
                        }
                    }
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        #endregion

        #region IMFFinalizableMediaSink methods

        //-------------------------------------------------------------------
        // Name: BeginFinalize 
        // Description: Starts the asynchronous finalize operation.
        //
        // Note: We use the Finalize operation to write the RIFF headers.
        //-------------------------------------------------------------------

        public int BeginFinalize(
            IMFAsyncCallback pCallback,
            object punkState)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::BeginFinalize");

                lock (this)
                {
                    CheckShutdown();

                    // Tell the stream to finalize.
                    m_pStream.Finalize(pCallback, punkState);
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: EndFinalize 
        // Description: Completes the asynchronous finalize operation.
        //-------------------------------------------------------------------

        public int EndFinalize(IMFAsyncResult pResult)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::EndFinalize");

                // Return the status code from the async result.
                int hr = pResult.GetStatus();
                if (hr < 0)
                {
                    throw new COMException("Failed status code in EndFinalize", hr);
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        #endregion

        #region IMFClockStateSink methods

        //-------------------------------------------------------------------
        // Name: OnClockStart 
        // Description: Called when the presentation clock starts.
        //
        // hnsSystemTime: System time when the clock started.
        // llClockStartOffset: Starting presentatation time.
        //
        // Note: For an archive sink, we don't care about the system time.
        //       But we need to cache the value of llClockStartOffset. This 
        //       gives us the earliest time stamp that we archive. If any 
        //       input samples have an earlier time stamp, we discard them.
        //-------------------------------------------------------------------

        public int OnClockStart(
            /* [in] */ long hnsSystemTime,
            /* [in] */ long llClockStartOffset)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::OnClockStart");
                lock (this)
                {
                    CheckShutdown();

                    m_pStream.Start(llClockStartOffset);
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: OnClockStop 
        // Description: Called when the presentation clock stops.
        //
        // Note: After this method is called, we stop accepting new data.
        //-------------------------------------------------------------------

        public int OnClockStop(
            /* [in] */ long hnsSystemTime)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::OnClockStop");
                lock (this)
                {
                    CheckShutdown();

                    m_pStream.Stop();
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: OnClockPause 
        // Description: Called when the presentation clock paused.
        //
        // Note: For an archive sink, the paused state is equivalent to the
        //       running (started) state. We still accept data and archive it.
        //-------------------------------------------------------------------

        public int OnClockPause(
            /* [in] */ long hnsSystemTime)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::OnClockPause");
                lock (this)
                {
                    CheckShutdown();

                    m_pStream.Pause();
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: OnClockRestart 
        // Description: Called when the presentation clock restarts.
        //-------------------------------------------------------------------

        public int OnClockRestart(
            /* [in] */ long hnsSystemTime)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::OnClockRestart");
                lock (this)
                {
                    CheckShutdown();

                    m_pStream.Restart();
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        //-------------------------------------------------------------------
        // Name: OnClockSetRate 
        // Description: Called when the presentation clock's rate changes.
        //
        // Note: For a rateless sink, the clock rate is not important.
        //-------------------------------------------------------------------

        public int OnClockSetRate(
            /* [in] */ long hnsSystemTime,
            /* [in] */ float flRate)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::OnClockSetRate");
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            TRACE("CWavSink::Dispose");

            Shutdown();
        }

        #endregion

        #region Private methods

        void CheckShutdown()
        {
            if (m_IsShutdown)
            {
                throw new COMException("Sink is shutdown", MFError.MF_E_SHUTDOWN);
            }
        }

        #endregion

        #region IMFCustom Members

        public int SetStream(object pStream)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("CWavSink::SetStream");
                if (pStream != null)
                {
                    m_pStream = new CWavStream(this, (IMFByteStream)pStream);
                }
                else
                {
                    throw new COMException("Null pstream", E_Pointer);
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        #endregion
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    //
    // CWavStream class. - Implements the stream sink.
    //
    // Notes: 
    // - Most of the real work gets done in this class. 
    // - The sink has one stream. If it had multiple streams, it would need to coordinate them.
    // - Most operations are done asynchronously on a work queue.
    // - Async methods are handled like this:
    //      1. Call ValidateOperation to check if the operation is permitted at this time
    //      2. Create an CAsyncOperation object for the operation.
    //      3. Call QueueAsyncOperation. This puts the operation on the work queue.
    //      4. The workqueue calls OnDispatchWorkItem.
    // - Locking:
    //      To avoid deadlocks, do not hold the CWavStream lock followed by the CWavSink lock.
    //      The other order is OK (CWavSink, then CWavStream).
    // 
    /////////////////////////////////////////////////////////////////////////////////////////////

    internal class CWavStream : COMBase, IMFStreamSinkAlt, IMFMediaTypeHandler, IMFAsyncCallback, IDisposable
    {
        #region Declarations

        enum State
        {
            TypeNotSet = 0,    // No media type is set
            Ready,             // Media type is set, Start has never been called.
            Started,
            Stopped,
            Paused,
            Finalized,

            Count = Finalized + 1    // Number of states
        }

        // StreamOperation: Defines various operations that can be performed on the stream.
        enum StreamOperation
        {
            OpSetMediaType = 0,
            OpStart,
            OpRestart,
            OpPause,
            OpStop,
            OpProcessSample,
            OpPlaceMarker,
            OpFinalize,

            Op_Count = OpFinalize + 1  // Number of operations
        }

        enum FlushState
        {
            DropSamples = 0,
            WriteSamples
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal class RIFFCHUNK
        {
            public FourCC fcc;
            public int cb;
        }

        // WAV_FILE_HEADER
        // This structure contains the first part of the .wav file, up to the
        // data portion. (Wave files are so simple there is no reason to write
        // a general-purpose RIFF authoring object.)
        internal struct WAV_FILE_HEADER
        {
            public RIFFCHUNK FileHeader;
            public FourCC fccWaveType;    // must be 'WAVE'
            public RIFFCHUNK WaveHeader;
            public WaveFormatEx WaveFormat;
            public RIFFCHUNK DataHeader;
        }

        #endregion

        #region Members

        static bool[,] m_ValidStateMatrix;

        State m_state;
        bool m_IsShutdown;               // Flag to indicate if Shutdown() method was called.

        int m_WorkQueueId;              // ID of the work queue for asynchronous operations.

        long m_StartTime;                // Presentation time when the clock started.
        int m_cbDataWritten;            // How many bytes we have written so far.

        CWavSink m_pSink;                   // Parent media sink

        IMFMediaEventQueueAlt m_pEventQueue;             // Event queue
        IMFByteStream m_pByteStream;             // Bytestream where we write the data.
        IMFMediaType m_pCurrentType;

        Queue m_SampleQueue;              // Queue to hold samples and markers.
        // Applies to: ProcessSample, PlaceMarker, BeginFinalize

        IMFAsyncResult m_pFinalizeResult;         // Result object for Finalize operation.

        #endregion

        public CWavStream(CWavSink pParent, IMFByteStream pByteStream)
        {
            int hr;
            TRACE("CWavStream::CWavStream");

            m_SampleQueue = new Queue();
            m_state = State.TypeNotSet;
            m_IsShutdown = false;
            m_pSink = null;
            m_pEventQueue = null;
            m_pByteStream = null;

            m_pCurrentType = null;
            m_pFinalizeResult = null;
            m_StartTime = 0;
            m_cbDataWritten = 0;
            m_WorkQueueId = 0;

            Debug.Assert(pParent != null);
            Debug.Assert(pByteStream != null);

            MFByteStreamCapabilities dwCaps = MFByteStreamCapabilities.None;
            const MFByteStreamCapabilities dwRequiredCaps = (MFByteStreamCapabilities.IsWritable | MFByteStreamCapabilities.IsSeekable);

            // Make sure the byte stream has the necessary caps bits.
            hr = pByteStream.GetCapabilities(out dwCaps);
            MFError.ThrowExceptionForHR(hr);

            if ((dwCaps & dwRequiredCaps) != dwRequiredCaps)
            {
                throw new COMException("stream doesn't have required caps", E_Fail);
            }

            // Move the file pointer to leave room for the RIFF headers.
            hr = pByteStream.SetCurrentPosition(Marshal.SizeOf(typeof(WAV_FILE_HEADER)));
            MFError.ThrowExceptionForHR(hr);

            // Create the event queue helper.
            hr = MFExternAlt.MFCreateEventQueue(out m_pEventQueue);
            MFError.ThrowExceptionForHR(hr);

            // Allocate a new work queue for async operations.
            hr = MFExtern.MFAllocateWorkQueue(out m_WorkQueueId);
            MFError.ThrowExceptionForHR(hr);

            m_pByteStream = pByteStream;
            m_pSink = pParent;
        }

        ~CWavStream()
        {
            TRACE("CWavStream::~CWavStream");
            Debug.Assert(m_IsShutdown);
        }

        #region IMFMediaEventGenerator methods.
        // Note: These methods call through to the event queue helper object.

        public int BeginGetEvent(
            //[In, MarshalAs(UnmanagedType.Interface)] IMFAsyncCallback pCallback,
            IntPtr pCallback,
            object punkState)
        {
            int hr;

            TRACE("CWavStream::BeginGetEvent");
            lock (this)
            {
                CheckShutdown();
                hr = m_pEventQueue.BeginGetEvent(pCallback, punkState);
                MFError.ThrowExceptionForHR(hr);
            }
            return S_Ok;
        }

        public int EndGetEvent(
            //IMFAsyncResult pResult,
            IntPtr pResult,
            out IMFMediaEvent ppEvent)
        {
            int hr;
            TRACE("CWavStream::EndGetEvent");

            lock (this)
            {
                CheckShutdown();

                hr = m_pEventQueue.EndGetEvent(pResult, out ppEvent);
                MFError.ThrowExceptionForHR(hr);
            }
            return S_Ok;
        }

        public int GetEvent(MFEventFlag dwFlags, out IMFMediaEvent ppEvent)
        {
            int hr;
            TRACE("CWavStream::GetEvent");

            // NOTE: 
            // GetEvent can block indefinitely, so we don't hold the lock.
            // This requires some juggling with the event queue pointer.

            IMFMediaEventQueueAlt pQueue;

            lock (this)
            {
                // Check shutdown
                CheckShutdown();

                // Get the pointer to the event queue.
                pQueue = m_pEventQueue;

            }   // release lock

            // Now get the event.
            hr = pQueue.GetEvent(dwFlags, out ppEvent);
            MFError.ThrowExceptionForHR(hr);

            //SAFE_RELEASE(pQueue);
            return S_Ok;
        }

        public int QueueEvent(MediaEventType met, Guid guidExtendedType, int hrStatus, ConstPropVariant pvValue)
        {
            int hr;
            TRACE(string.Format("CWavStream::QueueEvent ({0})", met.ToString()));

            lock (this)
            {
                CheckShutdown();

                hr = m_pEventQueue.QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
                MFError.ThrowExceptionForHR(hr);
            }
            return S_Ok;
        }

        #endregion

        #region IMFStreamSink methods

        //-------------------------------------------------------------------
        // Name: GetMediaSink 
        // Description: Returns the parent media sink.
        //-------------------------------------------------------------------

        public int GetMediaSink(out IMFMediaSinkAlt ppMediaSink)
        {
            TRACE("CWavStream::GetMediaSink");

            lock (this)
            {
                CheckShutdown();

                ppMediaSink = (IMFMediaSinkAlt)m_pSink;
            }
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: GetIdentifier 
        // Description: Returns the stream identifier.
        //-------------------------------------------------------------------

        public int GetIdentifier(out int pdwIdentifier)
        {
            TRACE("CWavStream::GetIdentifier");

            lock (this)
            {
                CheckShutdown();

                pdwIdentifier = CWavSink.WAV_SINK_STREAM_ID;
            }
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: GetMediaTypeHandler 
        // Description: Returns a media type handler for this stream.
        //-------------------------------------------------------------------

        public int GetMediaTypeHandler(out IMFMediaTypeHandler ppHandler)
        {
            TRACE("CWavStream::GetMediaTypeHandler");

            lock (this)
            {
                CheckShutdown();

                // This stream object acts as its own type handler, so we QI ourselves.
                ppHandler = (IMFMediaTypeHandler)this;
            }
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: ProcessSample 
        // Description: Receives an input sample. [Asynchronous]
        //
        // Note: The client should only give us a sample after we send an
        //       MEStreamSinkRequestSample event.
        //-------------------------------------------------------------------

        public int ProcessSample(IMFSample pSample)
        {
            TRACE("CWavStream::ProcessSample");
            lock (this)
            {
                if (pSample == null)
                {
                    throw new COMException("Null pSample", E_InvalidArgument);
                }

                CheckShutdown();

                // Validate the operation.
                ValidateOperation(CAsyncOperation.StreamOperation.OpProcessSample);

                // Add the sample to the sample queue.
                m_SampleQueue.Enqueue(pSample);

                // Unless we are paused, start an async operation to dispatch the next sample.
                if (m_state != State.Paused)
                {
                    // Queue the operation.
                    QueueAsyncOperation(CAsyncOperation.StreamOperation.OpProcessSample);
                }
            }
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: PlaceMarker 
        // Description: Receives a marker. [Asynchronous]
        //
        // Note: The client can call PlaceMarker at any time. In response,
        //       we need to queue an MEStreamSinkMarer event, but not until
        //       *after* we have processed all samples that we have received
        //       up to this point. 
        //
        //       Also, in general you might need to handle specific marker
        //       types, although this sink does not.
        //-------------------------------------------------------------------

        public int PlaceMarker(
            MFStreamSinkMarkerType eMarkerType,
            ConstPropVariant pvarMarkerValue,
            ConstPropVariant pvarContextValue)
        {
            TRACE("CWavStream::PlaceMarker");

            lock (this)
            {
                IMarker pMarker = null;

                CheckShutdown();

                ValidateOperation(CAsyncOperation.StreamOperation.OpPlaceMarker);

                // Create a marker object and put it on the sample queue.
                pMarker = new CMarker(
                    eMarkerType,
                    pvarMarkerValue,
                    pvarContextValue);

                m_SampleQueue.Enqueue(pMarker);

                // Unless we are paused, start an async operation to dispatch the next sample/marker.
                if (m_state != State.Paused)
                {
                    // Queue the operation.
                    QueueAsyncOperation(CAsyncOperation.StreamOperation.OpPlaceMarker); // Increments ref count on pOp.
                }
            }
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: Flush 
        // Description: Discards all samples that were not processed yet.
        //-------------------------------------------------------------------

        public int Flush()
        {
            TRACE("CWavStream::Flush");

            lock (this)
            {
                CheckShutdown();

                // Note: Even though we are flushing data, we still need to send 
                // any marker events that were queued.
                ProcessSamplesFromQueue(FlushState.DropSamples);
            }
            return S_Ok;
        }

        #endregion

        #region IMFMediaTypeHandler methods

        //-------------------------------------------------------------------
        // Name: IsMediaTypeSupported 
        // Description: Check if a media type is supported.
        //
        // pMediaType: The media type to check.
        // ppMediaType: Optionally, receives a "close match" media type.
        //-------------------------------------------------------------------

        public int IsMediaTypeSupported(
            /* [in] */ IMFMediaType pMediaType,
            // /* [out] */ out IMFMediaType ppMediaType)
            IntPtr ppMediaType) // use IntPtr since this can be NULL
        {
            TRACE("CWavStream::IsMediaTypeSupported");

            int hr;
            Guid majorType;
            WaveFormatEx pWav;
            int cbSize;

            lock (this)
            {
                CheckShutdown();

                hr = pMediaType.GetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, out majorType);
                MFError.ThrowExceptionForHR(hr);

                // First make sure it's audio. 
                if (majorType != MFMediaType.Audio)
                {
                    throw new COMException("type not audio", MFError.MF_E_INVALIDTYPE);
                }

                // Get a WAVEFORMATEX structure to validate against.
                hr = MFExtern.MFCreateWaveFormatExFromMFMediaType(pMediaType, out pWav, out cbSize, MFWaveFormatExConvertFlags.Normal);
                MFError.ThrowExceptionForHR(hr);

                // Validate the WAVEFORMATEX structure.
                ValidateWaveFormat(pWav, cbSize);

                // We don't return any "close match" types.
                if (ppMediaType != IntPtr.Zero)
                {
                    Marshal.WriteIntPtr(ppMediaType, IntPtr.Zero);
                }
            }

            //CoTaskMemFree(pWav);
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: GetMediaTypeCount 
        // Description: Return the number of preferred media types.
        //-------------------------------------------------------------------

        public int GetMediaTypeCount(out int pdwTypeCount)
        {
            TRACE("CWavStream::GetMediaTypeCount");

            lock (this)
            {
                CheckShutdown();
                pdwTypeCount = CWavSink.g_AudioFormats.Length;
            }
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: GetMediaTypeByIndex 
        // Description: Return a preferred media type by index.
        //-------------------------------------------------------------------

        public int GetMediaTypeByIndex(
            /* [in] */ int dwIndex,
            /* [out] */ out IMFMediaType ppType)
        {
            int hr;
            TRACE("CWavStream::GetMediaTypeByIndex");

            lock (this)
            {
                CheckShutdown();

                if (dwIndex >= CWavSink.g_AudioFormats.Length)
                {
                    throw new COMException("No more types", MFError.MF_E_NO_MORE_TYPES);
                }

                WaveFormatEx wav;
                InitializePCMWaveFormat(out wav, CWavSink.g_AudioFormats[dwIndex]);

                hr = MFExtern.MFCreateMediaType(out ppType);
                MFError.ThrowExceptionForHR(hr);

                hr = MFExtern.MFInitMediaTypeFromWaveFormatEx(ppType, wav, Marshal.SizeOf(typeof(WaveFormatEx)));
                MFError.ThrowExceptionForHR(hr);
            }
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: SetCurrentMediaType 
        // Description: Set the current media type.
        //-------------------------------------------------------------------

        public int SetCurrentMediaType(IMFMediaType pMediaType)
        {
            TRACE("CWavStream::SetCurrentMediaType");

            lock (this)
            {
                if (pMediaType == null)
                {
                    throw new COMException("Null media type", E_InvalidArgument);
                }

                CheckShutdown();

                // We don't allow format changes after streaming starts,
                // because this would invalidate the .wav file.
                ValidateOperation(CAsyncOperation.StreamOperation.OpSetMediaType);

                IsMediaTypeSupported(pMediaType, IntPtr.Zero);

                //SAFE_RELEASE(m_pCurrentType);
                m_pCurrentType = pMediaType;

                m_state = State.Ready;
            }
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: GetCurrentMediaType 
        // Description: Return the current media type, if any.
        //-------------------------------------------------------------------

        public int GetCurrentMediaType(out IMFMediaType ppMediaType)
        {
            TRACE("CWavStream::GetCurrentMediaType");
            lock (this)
            {
                CheckShutdown();

                if (m_pCurrentType == null)
                {
                    throw new COMException("no type set", MFError.MF_E_NOT_INITIALIZED);
                }

                ppMediaType = m_pCurrentType;
            }
            return S_Ok;
        }

        //-------------------------------------------------------------------
        // Name: GetMajorType 
        // Description: Return the major type GUID.
        //-------------------------------------------------------------------

        public int GetMajorType(out Guid pguidMajorType)
        {
            TRACE("CWavStream::GetMajorType");

            pguidMajorType = MFMediaType.Audio;
            return S_Ok;
        }

        #endregion

        #region private methods

        //-------------------------------------------------------------------
        // Name: Start 
        // Description: Called when the presentation clock starts.
        //
        // Note: Start time can be PRESENTATION_CURRENT_POSITION meaning
        //       resume from the last current position.
        //-------------------------------------------------------------------

        public void Start(long start)
        {
            lock (this)
            {
                ValidateOperation(CAsyncOperation.StreamOperation.OpStart);

                if (start != long.MaxValue) // 0x7fffffffffffffff
                {
                    m_StartTime = start;        // Cache the start time.
                }

                m_state = State.Started;
                QueueAsyncOperation(CAsyncOperation.StreamOperation.OpStart);
            }
        }

        //-------------------------------------------------------------------
        // Name: Stop
        // Description: Called when the presentation clock stops.
        //-------------------------------------------------------------------

        public void Stop()
        {
            lock (this)
            {
                ValidateOperation(CAsyncOperation.StreamOperation.OpStop);

                m_state = State.Stopped;
                QueueAsyncOperation(CAsyncOperation.StreamOperation.OpStop);
            }
        }

        //-------------------------------------------------------------------
        // Name: Pause
        // Description: Called when the presentation clock pauses.
        //-------------------------------------------------------------------

        public void Pause()
        {
            lock (this)
            {
                ValidateOperation(CAsyncOperation.StreamOperation.OpPause);

                m_state = State.Paused;
                QueueAsyncOperation(CAsyncOperation.StreamOperation.OpPause);
            }
        }

        //-------------------------------------------------------------------
        // Name: Restart
        // Description: Called when the presentation clock restarts.
        //-------------------------------------------------------------------

        public void Restart()
        {
            lock (this)
            {
                ValidateOperation(CAsyncOperation.StreamOperation.OpRestart);

                m_state = State.Started;
                QueueAsyncOperation(CAsyncOperation.StreamOperation.OpRestart);
            }
        }

        //-------------------------------------------------------------------
        // Name: Finalize
        // Description: Starts the async finalize operation.
        //-------------------------------------------------------------------

        public void Finalize(IMFAsyncCallback pCallback, object punkState)
        {
            int hr;
            lock (this)
            {
                ValidateOperation(CAsyncOperation.StreamOperation.OpFinalize);

                if (m_pFinalizeResult != null)
                {
                    throw new COMException("The operation is already pending.", MFError.MF_E_INVALIDREQUEST);
                }

                // Create and store the async result object.
                hr = MFExtern.MFCreateAsyncResult(null, pCallback, punkState, out m_pFinalizeResult);
                MFError.ThrowExceptionForHR(hr);

                m_state = State.Finalized;
                QueueAsyncOperation(CAsyncOperation.StreamOperation.OpFinalize);
            }
        }

        //-------------------------------------------------------------------
        // Name: ValidStateMatrix
        // Description: Class-static matrix of operations vs states.
        //
        // If an entry is TRUE, the operation is valid from that state.
        //-------------------------------------------------------------------

        bool ValidStateMatrix(State stat, CAsyncOperation.StreamOperation so)
        {
            if (m_ValidStateMatrix == null)
            {
                m_ValidStateMatrix = new bool[(int)State.Count, (int)CAsyncOperation.StreamOperation.Op_Count];

                // Note about states:
                // 1. OnClockRestart should only be called from paused state.
                // 2. While paused, the sink accepts samples but does not process them.

                m_ValidStateMatrix[(int)State.TypeNotSet, (int)CAsyncOperation.StreamOperation.OpSetMediaType] = true;
                m_ValidStateMatrix[(int)State.TypeNotSet, (int)CAsyncOperation.StreamOperation.OpStart] = false;
                m_ValidStateMatrix[(int)State.TypeNotSet, (int)CAsyncOperation.StreamOperation.OpRestart] = false;
                m_ValidStateMatrix[(int)State.TypeNotSet, (int)CAsyncOperation.StreamOperation.OpPause] = false;
                m_ValidStateMatrix[(int)State.TypeNotSet, (int)CAsyncOperation.StreamOperation.OpStop] = false;
                m_ValidStateMatrix[(int)State.TypeNotSet, (int)CAsyncOperation.StreamOperation.OpProcessSample] = false;
                m_ValidStateMatrix[(int)State.TypeNotSet, (int)CAsyncOperation.StreamOperation.OpPlaceMarker] = false;
                m_ValidStateMatrix[(int)State.TypeNotSet, (int)CAsyncOperation.StreamOperation.OpFinalize] = false;

                m_ValidStateMatrix[(int)State.Ready, (int)CAsyncOperation.StreamOperation.OpSetMediaType] = true;
                m_ValidStateMatrix[(int)State.Ready, (int)CAsyncOperation.StreamOperation.OpStart] = true;
                m_ValidStateMatrix[(int)State.Ready, (int)CAsyncOperation.StreamOperation.OpRestart] = false;
                m_ValidStateMatrix[(int)State.Ready, (int)CAsyncOperation.StreamOperation.OpPause] = true;
                m_ValidStateMatrix[(int)State.Ready, (int)CAsyncOperation.StreamOperation.OpStop] = true;
                m_ValidStateMatrix[(int)State.Ready, (int)CAsyncOperation.StreamOperation.OpProcessSample] = false;
                m_ValidStateMatrix[(int)State.Ready, (int)CAsyncOperation.StreamOperation.OpPlaceMarker] = true;
                m_ValidStateMatrix[(int)State.Ready, (int)CAsyncOperation.StreamOperation.OpFinalize] = true;

                m_ValidStateMatrix[(int)State.Started, (int)CAsyncOperation.StreamOperation.OpSetMediaType] = false;
                m_ValidStateMatrix[(int)State.Started, (int)CAsyncOperation.StreamOperation.OpStart] = true;
                m_ValidStateMatrix[(int)State.Started, (int)CAsyncOperation.StreamOperation.OpRestart] = false;
                m_ValidStateMatrix[(int)State.Started, (int)CAsyncOperation.StreamOperation.OpPause] = true;
                m_ValidStateMatrix[(int)State.Started, (int)CAsyncOperation.StreamOperation.OpStop] = true;
                m_ValidStateMatrix[(int)State.Started, (int)CAsyncOperation.StreamOperation.OpProcessSample] = true;
                m_ValidStateMatrix[(int)State.Started, (int)CAsyncOperation.StreamOperation.OpPlaceMarker] = true;
                m_ValidStateMatrix[(int)State.Started, (int)CAsyncOperation.StreamOperation.OpFinalize] = true;

                m_ValidStateMatrix[(int)State.Paused, (int)CAsyncOperation.StreamOperation.OpSetMediaType] = false;
                m_ValidStateMatrix[(int)State.Paused, (int)CAsyncOperation.StreamOperation.OpStart] = true;
                m_ValidStateMatrix[(int)State.Paused, (int)CAsyncOperation.StreamOperation.OpRestart] = true;
                m_ValidStateMatrix[(int)State.Paused, (int)CAsyncOperation.StreamOperation.OpPause] = true;
                m_ValidStateMatrix[(int)State.Paused, (int)CAsyncOperation.StreamOperation.OpStop] = true;
                m_ValidStateMatrix[(int)State.Paused, (int)CAsyncOperation.StreamOperation.OpProcessSample] = true;
                m_ValidStateMatrix[(int)State.Paused, (int)CAsyncOperation.StreamOperation.OpPlaceMarker] = true;
                m_ValidStateMatrix[(int)State.Paused, (int)CAsyncOperation.StreamOperation.OpFinalize] = true;

                m_ValidStateMatrix[(int)State.Stopped, (int)CAsyncOperation.StreamOperation.OpSetMediaType] = false;
                m_ValidStateMatrix[(int)State.Stopped, (int)CAsyncOperation.StreamOperation.OpStart] = true;
                m_ValidStateMatrix[(int)State.Stopped, (int)CAsyncOperation.StreamOperation.OpRestart] = false;
                m_ValidStateMatrix[(int)State.Stopped, (int)CAsyncOperation.StreamOperation.OpPause] = false;
                m_ValidStateMatrix[(int)State.Stopped, (int)CAsyncOperation.StreamOperation.OpStop] = true;
                m_ValidStateMatrix[(int)State.Stopped, (int)CAsyncOperation.StreamOperation.OpProcessSample] = false;
                m_ValidStateMatrix[(int)State.Stopped, (int)CAsyncOperation.StreamOperation.OpPlaceMarker] = true;
                m_ValidStateMatrix[(int)State.Stopped, (int)CAsyncOperation.StreamOperation.OpFinalize] = true;

                m_ValidStateMatrix[(int)State.Finalized, (int)CAsyncOperation.StreamOperation.OpSetMediaType] = false;
                m_ValidStateMatrix[(int)State.Finalized, (int)CAsyncOperation.StreamOperation.OpStart] = false;
                m_ValidStateMatrix[(int)State.Finalized, (int)CAsyncOperation.StreamOperation.OpRestart] = false;
                m_ValidStateMatrix[(int)State.Finalized, (int)CAsyncOperation.StreamOperation.OpPause] = false;
                m_ValidStateMatrix[(int)State.Finalized, (int)CAsyncOperation.StreamOperation.OpStop] = false;
                m_ValidStateMatrix[(int)State.Finalized, (int)CAsyncOperation.StreamOperation.OpProcessSample] = false;
                m_ValidStateMatrix[(int)State.Finalized, (int)CAsyncOperation.StreamOperation.OpPlaceMarker] = false;
                m_ValidStateMatrix[(int)State.Finalized, (int)CAsyncOperation.StreamOperation.OpFinalize] = false;
            }

            return m_ValidStateMatrix[(int)stat, (int)so];
        }

        //-------------------------------------------------------------------
        // Name: ValidateOperation
        // Description: Checks if an operation is valid in the current state.
        //-------------------------------------------------------------------

        void ValidateOperation(CAsyncOperation.StreamOperation op)
        {
            Debug.Assert(!m_IsShutdown);

            bool bTransitionAllowed = ValidStateMatrix(m_state, op);
            if (!bTransitionAllowed)
            {
                throw new COMException("Not valid from current state", MFError.MF_E_INVALIDREQUEST);
            }
        }

        //-------------------------------------------------------------------
        // Name: Shutdown
        // Description: Shuts down the stream sink.
        //-------------------------------------------------------------------

        public void Shutdown()
        {
            int hr;
            Debug.Assert(!m_IsShutdown);
            GC.SuppressFinalize(this);

            if (m_pEventQueue != null)
            {
                hr = m_pEventQueue.Shutdown();
                MFError.ThrowExceptionForHR(hr);
            }

            hr = MFExtern.MFUnlockWorkQueue(m_WorkQueueId);
            MFError.ThrowExceptionForHR(hr);

            m_SampleQueue.Clear();

            SafeRelease(m_pSink);
            SafeRelease(m_pEventQueue);
            SafeRelease(m_pByteStream);
            SafeRelease(m_pCurrentType);
            SafeRelease(m_pFinalizeResult);

            m_pSink = null;
            m_pEventQueue = null;
            m_pByteStream = null;
            m_pCurrentType = null;
            m_pFinalizeResult = null;

            m_IsShutdown = true;
        }

        //-------------------------------------------------------------------
        // Name: QueueAsyncOperation
        // Description: Puts an async operation on the work queue.
        //-------------------------------------------------------------------

        void QueueAsyncOperation(CAsyncOperation.StreamOperation op)
        {
            int hr;
            CAsyncOperation pOp = new CAsyncOperation(op); // Created with ref count = 1

            hr = MFExtern.MFPutWorkItem(m_WorkQueueId, this, pOp);
            MFError.ThrowExceptionForHR(hr);

            //SAFE_RELEASE(pOp);
        }

        //-------------------------------------------------------------------
        // Name: OnDispatchWorkItem
        // Description: Callback for MFPutWorkItem.
        //-------------------------------------------------------------------

        void OnDispatchWorkItem(IMFAsyncResult pAsyncResult)
        {
            int hr;

            // Called by work queue thread. Need to hold the critical section.
            lock (this)
            {
                object pState = null;

                hr = pAsyncResult.GetState(out pState);
                MFError.ThrowExceptionForHR(hr);

                try
                {
                    // The state object is a CAsncOperation object.
                    CAsyncOperation pOp = (CAsyncOperation)pState;

                    CAsyncOperation.StreamOperation op = pOp.m_op;

                    switch (op)
                    {
                        case CAsyncOperation.StreamOperation.OpStart:
                        case CAsyncOperation.StreamOperation.OpRestart:
                            // Send MEStreamSinkStarted.
                            QueueEvent(MediaEventType.MEStreamSinkStarted, Guid.Empty, 0, null);

                            // Kick things off by requesting two samples...
                            QueueEvent(MediaEventType.MEStreamSinkRequestSample, Guid.Empty, 0, null);
                            QueueEvent(MediaEventType.MEStreamSinkRequestSample, Guid.Empty, 0, null);

                            // There might be samples queue from earlier (ie, while paused).
                            ProcessSamplesFromQueue(FlushState.WriteSamples);
                            break;

                        case CAsyncOperation.StreamOperation.OpStop:
                            // Drop samples from queue.
                            ProcessSamplesFromQueue(FlushState.DropSamples);

                            // Send the event even if the previous call failed.
                            QueueEvent(MediaEventType.MEStreamSinkStopped, Guid.Empty, 0, null);
                            break;

                        case CAsyncOperation.StreamOperation.OpPause:
                            QueueEvent(MediaEventType.MEStreamSinkPaused, Guid.Empty, 0, null);
                            break;

                        case CAsyncOperation.StreamOperation.OpProcessSample:
                        case CAsyncOperation.StreamOperation.OpPlaceMarker:
                            DispatchProcessSample(pOp);
                            break;

                        case CAsyncOperation.StreamOperation.OpFinalize:
                            DispatchFinalize(pOp);
                            break;
                    }
                }
                finally
                {
                    SafeRelease(pState);
                }
            }
        }

        //-------------------------------------------------------------------
        // Name: DispatchProcessSample
        // Description: Complete a ProcessSample or PlaceMarker request.
        //-------------------------------------------------------------------

        void DispatchProcessSample(CAsyncOperation pOp)
        {
            Debug.Assert(pOp != null);

            try
            {
                ProcessSamplesFromQueue(FlushState.WriteSamples);

                // Ask for another sample
                if (pOp.m_op == CAsyncOperation.StreamOperation.OpProcessSample)
                {
                    QueueEvent(MediaEventType.MEStreamSinkRequestSample, Guid.Empty, 0, null);
                }
            }
            catch (Exception e)
            {
                int hr = Marshal.GetHRForException(e);

                // We are in the middle of an asynchronous operation, so if something failed, send an error.
                QueueEvent(MediaEventType.MEError, Guid.Empty, hr, null);
            }
        }

        //-------------------------------------------------------------------
        // Name: ProcessSamplesFromQueue
        // Description: 
        //
        // Removes all of the samples and markers that are currently in the
        // queue and processes them.
        //
        // If bFlushData = DropSamples:
        //     For each marker, send an MEStreamSinkMarker event, with hr = E_ABORT.
        //     For each sample, drop the sample.
        //
        // If bFlushData = WriteSamples
        //     For each marker, send an MEStreamSinkMarker event, with hr = S_OK.
        //     For each sample, write the sample to the file.
        //
        // This method is called when we flush, stop, restart, receive a new
        // sample, or receive a marker.
        //-------------------------------------------------------------------

        void ProcessSamplesFromQueue(FlushState bFlushData)
        {
            // Enumerate all of the samples/markers in the queue.
            while (m_SampleQueue.Count > 0)
            {
                object pUnk = null;
                IMarker pMarker = null;
                IMFSample pSample = null;

                pUnk = m_SampleQueue.Dequeue();

                // Figure out if this is a marker or a sample.
                pMarker = pUnk as IMarker;

                if (pMarker == null)
                {
                    // If this is a sample, write it to the file.
                    pSample = (IMFSample)pUnk;
                }

                try
                {
                    // Now handle the sample/marker appropriately.
                    if (pMarker != null)
                    {
                        SendMarkerEvent(pMarker, bFlushData);
                    }
                    else
                    {
                        Debug.Assert(pSample != null);    // Not a marker, must be a sample
                        if (bFlushData == FlushState.WriteSamples)
                        {
                            WriteSampleToFile(pSample);
                        }
                    }
                }
                finally
                {
                    //SAFE_RELEASE(pUnk);
                    SafeRelease(pMarker);
                    SafeRelease(pSample);
                }
            }     // while loop
        }

        //-------------------------------------------------------------------
        // Name: WriteSampleToFile
        // Description: Output one media sample to the file.
        //-------------------------------------------------------------------

        void WriteSampleToFile(IMFSample pSample)
        {
            int hr;
            int i;
            long time;
            int cBufferCount; // Number of buffers in the sample.
            IntPtr pData;
            int cbData = 0;
            int cbWritten = 0;

            // Get the time stamp
            hr = pSample.GetSampleTime(out time);
            MFError.ThrowExceptionForHR(hr);

            // If the time stamp is too early, just discard this sample.
            if (time < m_StartTime)
            {
                return;
            }

            // Note: If there is no time stamp on the sample, proceed anyway.

            // Find how many buffers are in this sample.
            hr = pSample.GetBufferCount(out cBufferCount);
            MFError.ThrowExceptionForHR(hr);

            // Loop through all the buffers in the sample.
            for (int iBuffer = 0; iBuffer < cBufferCount; iBuffer++)
            {
                IMFMediaBuffer pBuffer = null;

                hr = pSample.GetBufferByIndex(iBuffer, out pBuffer);
                MFError.ThrowExceptionForHR(hr);

                try
                {
                    // Lock the buffer and write the data to the file.
                    hr = pBuffer.Lock(out pData, out i, out cbData);
                    MFError.ThrowExceptionForHR(hr);

                    hr = m_pByteStream.Write(pData, cbData, out cbWritten);
                    MFError.ThrowExceptionForHR(hr);

                    hr = pBuffer.Unlock();
                    MFError.ThrowExceptionForHR(hr);

                    // Update the running tally of bytes written.
                    m_cbDataWritten += cbData;
                }
                finally
                {
                    SafeRelease(pBuffer);
                }
            }   // for loop
        }

        //-------------------------------------------------------------------
        // Name: SendMarkerEvent
        // Description: Saned a marker event.
        // 
        // pMarker: Pointer to our custom IMarker interface, which holds
        //          the marker information.
        //-------------------------------------------------------------------

        void SendMarkerEvent(IMarker pMarker, FlushState fs)
        {
            int hrStatus = 0;  // Status code for marker event.

            if (fs == FlushState.DropSamples)
            {
                hrStatus = E_Abort;
            }

            PropVariant var = new PropVariant();

            // Get the context data.
            pMarker.GetContext(var);

            QueueEvent(MediaEventType.MEStreamSinkMarker, Guid.Empty, hrStatus, var);

            var.Clear();
        }

        //-------------------------------------------------------------------
        // Name: DispatchFinalize
        // Description: Complete a BeginFinalize request.
        //-------------------------------------------------------------------

        void DispatchFinalize(CAsyncOperation pOp)
        {
            int hr;
            int cbSize = 0;
            int cbWritten = 0;

            try
            {
                WAV_FILE_HEADER header = new WAV_FILE_HEADER();

                int cbFileSize = m_cbDataWritten + Marshal.SizeOf(typeof(WAV_FILE_HEADER)) - Marshal.SizeOf(typeof(RIFFCHUNK));

                // Write any samples left in the queue...
                ProcessSamplesFromQueue(FlushState.WriteSamples);

                // Now we're done writing all of the audio data. 

                // Fill in the RIFF headers...
                hr = MFExtern.MFCreateWaveFormatExFromMFMediaType(m_pCurrentType, out header.WaveFormat, out cbSize, MFWaveFormatExConvertFlags.Normal);
                MFError.ThrowExceptionForHR(hr);

                header.FileHeader = new RIFFCHUNK();
                header.FileHeader.fcc = new FourCC('R', 'I', 'F', 'F');
                header.FileHeader.cb = cbFileSize;
                header.fccWaveType = new FourCC('W', 'A', 'V', 'E');

                header.WaveHeader = new RIFFCHUNK();
                header.WaveHeader.fcc = new FourCC('f', 'm', 't', ' ');
                header.WaveHeader.cb = Marshal.SizeOf(typeof(WaveFormatEx));

                header.DataHeader = new RIFFCHUNK();
                header.DataHeader.fcc = new FourCC('d', 'a', 't', 'a');
                header.DataHeader.cb = m_cbDataWritten;

                // Move the file pointer back to the start of the file and write the
                // RIFF headers.
                hr = m_pByteStream.SetCurrentPosition(0);
                MFError.ThrowExceptionForHR(hr);

                IntPtr ip = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WAV_FILE_HEADER)));

                try
                {
                    Marshal.StructureToPtr(header, ip, false);
                    hr = m_pByteStream.Write(ip, Marshal.SizeOf(typeof(WAV_FILE_HEADER)), out cbWritten);
                    MFError.ThrowExceptionForHR(hr);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(ip);
                }

                // Close the byte stream.
                hr = m_pByteStream.Close();
                MFError.ThrowExceptionForHR(hr);

                hr = 0;
            }
            catch (Exception e)
            {
                hr = Marshal.GetHRForException(e);
            }

            // Set the async status and invoke the callback.
            hr = m_pFinalizeResult.SetStatus(hr);
            MFError.ThrowExceptionForHR(hr);
            hr = MFExtern.MFInvokeCallback(m_pFinalizeResult);
            MFError.ThrowExceptionForHR(hr);
        }

        void InitializePCMWaveFormat(out WaveFormatEx pWav, PCM_Audio_Format_Params param)
        {
            pWav = new WaveFormatEx();

            pWav.wFormatTag = 1;
            pWav.cbSize = 0;

            pWav.nChannels = param.nChannels;
            pWav.nSamplesPerSec = param.nSamplesPerSec;
            pWav.wBitsPerSample = param.wBitsPerSample;

            // Derived values
            pWav.nBlockAlign = (short)(pWav.nChannels * (pWav.wBitsPerSample / 8));
            pWav.nAvgBytesPerSec = pWav.nSamplesPerSec * pWav.nBlockAlign;
        }

        void CheckShutdown()
        {
            if (m_IsShutdown)
            {
                throw new COMException("Sink is shutdown", MFError.MF_E_SHUTDOWN);
            }
        }

        //-------------------------------------------------------------------
        // Name: ValidateWaveFormat
        // Description: Validates a WAVEFORMATEX structure. 
        //
        // Just to keep the sample as simple as possible, we only accept 
        // uncompressed PCM formats.
        //-------------------------------------------------------------------

        void ValidateWaveFormat(WaveFormatEx pWav, int cbSize)
        {
            if (pWav.wFormatTag != 1)
            {
                throw new COMException("wFormatTag", MFError.MF_E_INVALIDMEDIATYPE);
            }

            if (pWav.nChannels != 1 && pWav.nChannels != 2)
            {
                throw new COMException("nChannels", MFError.MF_E_INVALIDMEDIATYPE);
            }

            if (pWav.wBitsPerSample != 8 && pWav.wBitsPerSample != 16)
            {
                throw new COMException("wBitsPerSample", MFError.MF_E_INVALIDMEDIATYPE);
            }

            if (pWav.cbSize != 0)
            {
                throw new COMException("cbSize", MFError.MF_E_INVALIDMEDIATYPE);
            }

            // Make sure block alignment was calculated correctly.
            if (pWav.nBlockAlign != pWav.nChannels * (pWav.wBitsPerSample / 8))
            {
                throw new COMException("nBlockAlign", MFError.MF_E_INVALIDMEDIATYPE);
            }

            // Make sure average bytes per second was calculated correctly.
            if (pWav.nAvgBytesPerSec != pWav.nSamplesPerSec * pWav.nBlockAlign)
            {
                throw new COMException("nAvgBytesPerSec", MFError.MF_E_INVALIDMEDIATYPE);
            }

            // Everything checked out.
        }

        #endregion

        #region IMFAsyncCallback Members

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            throw new COMException("The method or operation is not implemented.", E_NotImplemented);
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            try
            {
                OnDispatchWorkItem(pAsyncResult);
            }
            finally
            {
                SafeRelease(pAsyncResult);
            }
            return S_Ok;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            TRACE("CWavStream::Dispose");

            SafeRelease(m_pEventQueue);
            SafeRelease(m_pByteStream);
            SafeRelease(m_pCurrentType);
            SafeRelease(m_pFinalizeResult);

            m_pEventQueue = null;
            m_pByteStream = null;
            m_pCurrentType = null;
            m_pFinalizeResult = null;

            //m_pSink.Dispose();  // break deadly embrace
            m_pSink = null;
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////

    [System.Security.SuppressUnmanagedCodeSecurity,
    Guid("3AC82233-933C-43a9-AF3D-ADC94EABF406"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMarker
    {
        void GetMarkerType(out MFStreamSinkMarkerType pType);
        void GetMarkerValue(PropVariant pvar);
        void GetContext([In, Out] PropVariant pvar);
    }

    internal class CMarker : COMBase, IMarker, IDisposable
    {
        //////////////////////
        // CMarker class
        // Holds information from IMFStreamSink::PlaceMarker
        // 
        MFStreamSinkMarkerType m_eMarkerType;
        PropVariant m_varMarkerValue;
        PropVariant m_varContextValue;

        public CMarker(
            MFStreamSinkMarkerType eMarkerType,
            ConstPropVariant pvarMarkerValue,     // Can be null.
            ConstPropVariant pvarContextValue    // Can be null.
            )
        {
            TRACE("CMarker::MFStreamSinkMarkerType");
            m_eMarkerType = eMarkerType;

            // Copy the marker data.
            if (pvarMarkerValue != null)
            {
                m_varMarkerValue = new PropVariant(pvarMarkerValue);
            }
            else
            {
                m_varMarkerValue = new PropVariant();
            }

            if (pvarContextValue != null)
            {
                m_varContextValue = new PropVariant(pvarContextValue);
            }
            else
            {
                m_varContextValue = new PropVariant();
            }
        }

        ~CMarker()
        {
            TRACE("~CMarker");
            if (m_varMarkerValue != null)
            {
                m_varMarkerValue.Clear();
            }

            if (m_varContextValue != null)
            {
                m_varContextValue.Clear();
            }
        }

        // IMarker methods
        public void GetMarkerType(out MFStreamSinkMarkerType pType)
        {
            TRACE("CMarker::GetMarkerType");
            pType = m_eMarkerType;
        }

        public void GetMarkerValue(PropVariant pvar)
        {
            TRACE("CMarker::GetMarkerValue");
            if (pvar == null)
            {
                throw new COMException("null variant", E_Pointer);
            }

            m_varMarkerValue.Copy(pvar);
        }

        public void GetContext(PropVariant pvar)
        {
            TRACE("CMarker::GetContext");

            if (pvar == null)
            {
                throw new COMException("null variant", E_Pointer);
            }
            m_varContextValue.Copy(pvar);
        }

        #region IDisposable Members

        public void Dispose()
        {
            TRACE("CMarker::Dispose");

            if (m_varMarkerValue != null)
            {
                m_varMarkerValue.Clear();
                m_varMarkerValue = null;
            }

            if (m_varContextValue != null)
            {
                m_varContextValue.Clear();
                m_varContextValue = null;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    internal class CAsyncOperation : COMBase, IDisposable
    {
        public enum StreamOperation
        {
            OpSetMediaType = 0,
            OpStart,
            OpRestart,
            OpPause,
            OpStop,
            OpProcessSample,
            OpPlaceMarker,
            OpFinalize,

            Op_Count = OpFinalize + 1  // Number of operations
        }

        public StreamOperation m_op;   // The operation to perform.

        /////////////////////////////////////////////////////////////////////////////////////////////
        //
        // CAsyncOperation class. - Private class used by CWavStream class.
        //
        /////////////////////////////////////////////////////////////////////////////////////////////

        public CAsyncOperation(StreamOperation op)
        {
            TRACE(string.Format("CAsyncOperation::CAsyncOperation ({0})", op.ToString()));
            m_op = op;
        }

        ~CAsyncOperation()
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            TRACE(string.Format("~CAsyncOperation ({0})", m_op.ToString()));
            GC.SuppressFinalize(this);
        }

        #endregion
    }

}
