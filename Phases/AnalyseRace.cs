// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
//
// Copyright 2020 Merlin Cooper 
// https://github.com/MerlinCooper/iRacingReplayDirector
//
// iRacingReplayOverlay is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayOverlay is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.
//

using iRacingReplayOverlay.Phases.Analysis;
using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Phases.Direction;
using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;


namespace iRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        int raceStartFrameNumber = 0;
        internal Incidents incidents;

        //create classes needed to analze race as global variables in the iRacingReplay instance 
        internal OverlayData overlayData = new OverlayData();
        internal RemovalEdits removalEdits;
        internal CommentaryMessages commentaryMessages;
        internal RecordPitStop recordPitStop;
        internal RecordFastestLaps fastestLaps;
        internal ReplayControl replayControl;
        internal SessionDataCapture sessionDataCapture;
        internal SampleFilter captureLeaderBoardEveryHalfSecond;
        internal SampleFilter captureCamDriverEveryQuaterSecond;
        internal SampleFilter captureCamDriverEvery4Seconds;


        public void _AnalyseRace(Action onComplete)
        {
            var hwnd = Win32.Messages.FindWindow(null, "iRacing.com Simulator");
            Win32.Messages.ShowWindow(hwnd, Win32.Messages.SW_SHOWNORMAL);
            Win32.Messages.SetForegroundWindow(hwnd);
            Thread.Sleep(Settings.Default.PeriodWaitForIRacingSwitch);

            var data = iRacing.GetDataFeed()
                .WithCorrectedPercentages()
                .AtSpeed(16)
                .RaceOnly()
                .First(d => d.Telemetry.SessionState == SessionState.Racing);

            raceStartFrameNumber = data.Telemetry.ReplayFrameNum - (60 * 20);

            if (raceStartFrameNumber < 0)
            {
                TraceInfo.WriteLine("Unable to start capturing at 20 seconds prior to race start.  Starting at start of replay file.");
                raceStartFrameNumber = 0;
            }

            TraceDebug.WriteLine(data.Telemetry.ToString());
            
            AnalyseIncidents();                                                         //Analyse incidents
            AnalyseRaceSituations(new iRacingConnection().GetBufferedDataFeed());       //Analyse race situation (all) by playing out replay at 16x speed. 

            onComplete();
        }

        void AnalyseIncidents()
        {
            iRacing.Replay.MoveToFrame(raceStartFrameNumber);

            incidents = new Incidents();

            if (!Settings.Default.DisableIncidentsSearch)
            {
                var incidentSamples = iRacing.GetDataFeed().RaceIncidents2(Settings.Default.IncidentScanWait, shortTestOnly ? 12 : int.MaxValue);

                foreach (var data in incidentSamples)
                    incidents.Process(data);
            }
        }

        //Analyse race situations at maximum replay speed w/o recording.  
        void AnalyseRaceSituations(IEnumerable<DataSample> samples)
        {
            //Start iRacing Replay from the beginning with maximum speed (16x)
            iRacing.Replay.MoveToFrame(raceStartFrameNumber);
            iRacing.Replay.SetSpeed(16);

            //copied from iRacing.Capturing because race events in app V1.0.x.x are identified during capturing the whole video. 
            //var overlayData = new OverlayData();
            removalEdits = new RemovalEdits(overlayData.RaceEvents);
            commentaryMessages = new CommentaryMessages(overlayData);
            recordPitStop = new RecordPitStop(commentaryMessages);
            fastestLaps = new RecordFastestLaps(overlayData);
            replayControl = new ReplayControl(samples.First().SessionData, incidents, removalEdits, TrackCameras);
            sessionDataCapture = new SessionDataCapture(overlayData);
            captureLeaderBoardEveryHalfSecond = new SampleFilter(TimeSpan.FromSeconds(0.5),
                new CaptureLeaderBoard(overlayData, commentaryMessages, removalEdits).Process);
            captureCamDriverEveryQuaterSecond = new SampleFilter(TimeSpan.FromSeconds(0.25),
                    new CaptureCamDriver(overlayData).Process);

            captureCamDriverEvery4Seconds = new SampleFilter(TimeSpan.FromSeconds(4),
                new LogCamDriver().Process);


            ApplyFirstLapCameraDirection(samples, replayControl);

            samples = samples
                .VerifyReplayFrames()
                .WithCorrectedPercentages()
                .WithCorrectedDistances()
                .WithFastestLaps()
                .WithFinishingStatus()
                .WithPitStopCounts()
                .TakeUntil(3.Seconds()).Of(d => d.Telemetry.LeaderHasFinished && d.Telemetry.RaceCars.All(c => c.HasSeenCheckeredFlag || c.HasRetired || c.TrackSurface != TrackLocation.OnTrack))
                .TakeUntil(3.Seconds()).AfterReplayPaused();
            samples = samples.AtSpeed(16);
            Settings.AppliedTimingFactor = 1.0 / 16.0;

            var startTime = DateTime.Now;

            overlayData.CapturedVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            foreach (var data in samples)
            {
                var relativeTime = DateTime.Now - startTime;

                replayControl.Process(data);
                sessionDataCapture.Process(data);
                captureLeaderBoardEveryHalfSecond.Process(data, relativeTime);
                captureCamDriverEveryQuaterSecond.Process(data, relativeTime);
                recordPitStop.Process(data, relativeTime);
                fastestLaps.Process(data, relativeTime);
                removalEdits.Process(data, relativeTime);
                captureCamDriverEvery4Seconds.Process(data, relativeTime);
            }

            removalEdits.Stop();

            TraceDebug.WriteLine("Race analysis phase completed");

            //save OverlayData into target folder for video ("working folder")
            SaveReplayScript(overlayData);
            TraceDebug.WriteLine("Replay Script saved to disk");

            iRacing.Replay.SetSpeed(0);
        }
    }
}
