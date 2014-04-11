using MediaFoundation;
using MediaFoundation.Net;
using MediaFoundation.Transform;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace iRacingReplayOverlay.net
{

	public struct StreamInfo
	{
		private readonly SinkStream sinkStream;
		public int sampleCount;

		public StreamInfo(SinkStream sinkStream)
		{
			this.sinkStream = sinkStream;
			this.sampleCount = 0;
		}

		public SinkStream SinkStream
		{
			get	{ return sinkStream; }
		}
	}
	
}
