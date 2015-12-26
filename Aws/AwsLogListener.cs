// This file is part of iRacingReplayOverlay.
//
// Copyright 2016 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net

using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using iRacingSDK.Support;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace iRacingReplayOverlay
{
    public class AwsLogListener : TraceListener
    {
        readonly Timer timer;
        readonly BlockingCollection<InputLogEvent> items = new BlockingCollection<InputLogEvent>(100);

        protected override void Dispose(bool disposing)
        {
            if (timer != null)
                timer.Dispose();

            Writer(null);

            base.Dispose(disposing);
        }

        public AwsLogListener()
        {
            int start = 0;
            int period = 5000;
            timer = new Timer(Writer, null, start, period);
        }

        void Writer(object state)
        {
            var logEvents = new List<InputLogEvent>();

            var more = true;
            while (more)
            {
                InputLogEvent item;
                more = items.TryTake(out item);
                if (more)
                    logEvents.Add(item);
            }

            if (logEvents.Count == 0)
                return;

            if (!Settings.Default.SendUsageData)
                return;

            using (var logs = new AmazonCloudWatchLogsClient(AwsKeys.AccessKey, AwsKeys.SecretKey, RegionEndpoint.APSoutheast2))
            {
                var request = new PutLogEventsRequest(AwsKeys.GroupName, AwsKeys.StreamName, logEvents);

                var describeLogStreamsRequest = new DescribeLogStreamsRequest(AwsKeys.GroupName);
                var describeLogStreamsResponse = logs.DescribeLogStreams(describeLogStreamsRequest);
                var logStreams = describeLogStreamsResponse.LogStreams;
                var token = logStreams.First().UploadSequenceToken;

                request.SequenceToken = token;

                logs.PutLogEvents(request);
            }
        }

        public override void Write(string message)
        {
            items.Add(new InputLogEvent { Message = "{0}: {1}".F(Settings.Default.TrackingID, message), Timestamp = DateTime.Now });
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }
    }
}
