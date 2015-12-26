// This file is part of iRacingReplayOverlay.
//
// Copyright 2016 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net

using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime;
using iRacingSDK.Support;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Timers;

namespace iRacingReplayOverlay
{
    public class AwsLogListener : TraceListener
    {
        static Timer timer;
        static readonly BlockingCollection<InputLogEvent> items = new BlockingCollection<InputLogEvent>(100);
        static string trackingId = Settings.Default.TrackingID;

        protected override void Dispose(bool disposing)
        {
            if (timer != null)
            {
                var t = timer;
                timer = null;
                t.Stop();
                t.Dispose();
            }

            Writer(null, null);
            Settings.Default.PropertyChanged -= Default_PropertyChanged;

            base.Dispose(disposing);
        }

        public AwsLogListener()
        {
            timer = new Timer(5000);
            timer.Elapsed += Writer;
            timer.AutoReset = false;
            timer.Enabled = true;
            
            Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            trackingId = Settings.Default.TrackingID;
        }

        void Writer(object sender, ElapsedEventArgs e)
        {
            try
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
                    var request = new PutLogEventsRequest(AwsKeys.GroupName, trackingId, logEvents);

                    var describeLogStreamsRequest = new DescribeLogStreamsRequest(AwsKeys.GroupName);
                    var describeLogStreamsResponse = logs.DescribeLogStreams(describeLogStreamsRequest);
                    var logStreams = describeLogStreamsResponse.LogStreams;
                    var logStream = logStreams.FirstOrDefault(ls => ls.LogStreamName == trackingId);
                    if (logStream != null)
                    {
                        var token = logStream.UploadSequenceToken;
                        request.SequenceToken = token;
                        checkResponse(logs.PutLogEvents(request));
                    }
                    else
                    {
                        var createRequest = new CreateLogStreamRequest(AwsKeys.GroupName, trackingId);
                        checkResponse(logs.CreateLogStream(createRequest));
                        checkResponse(logs.PutLogEvents(request));
                    }
                }
            }
            catch(Exception ee)
            {
                TraceInfo.WriteLine(ee.Message);
                TraceDebug.WriteLine(ee.StackTrace);
            }
            finally
            {
                if(timer != null)
                    timer.Start();
            }
        }

        private void checkResponse(AmazonWebServiceResponse response)
        {
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return;

            var msg = string.Join(", ", response.ResponseMetadata.Metadata.Select(x => "{0}:{1}".F(x.Key, x.Value)).ToArray());
            throw new Exception(msg);
        }

        public override void Write(string message)
        {
            items.Add(new InputLogEvent { Message = message, Timestamp = DateTime.Now });
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }
    }
}
