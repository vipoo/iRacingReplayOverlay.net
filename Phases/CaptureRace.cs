// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
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

using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Phases.Direction;
using iRacingReplayOverlay.Support;
using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Win32;

namespace iRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        string workingFolder;
        string introVideoFileName;

        void _WithWorkingFolder(string workingFolder)
        {
            this.workingFolder = workingFolder;
        }

        void _WithIntroVideo(string fileName)
        {
            this.introVideoFileName = fileName;
        }

        void _CaptureRace(Action<string> onComplete)
        {
            _CaptureRaceTest(onComplete, new iRacingConnection().GetBufferedDataFeed());
        }

        internal void _CaptureRaceTest(Action<string> onComplete, IEnumerable<DataSample> samples)
        {
            var overlayData = new OverlayData { IntroVideoFileName = introVideoFileName };
            var removalEdits = new RemovalEdits(overlayData.RaceEvents);
            var commentaryMessages = new CommentaryMessages(overlayData);
            var videoCapture = new VideoCapture();
            var recordPitStop = new RecordPitStop(commentaryMessages);
            var fastestLaps = new RecordFastestLaps(overlayData);
            var replayControl = new ReplayControl(samples.First().SessionData, incidents, removalEdits, TrackCameras);
            var sessionDataCapture = new SessionDataCapture(overlayData);
            var captureLeaderBoardEveryHalfSecond = new SampleFilter(TimeSpan.FromSeconds(0.5),
                new CaptureLeaderBoard(overlayData, commentaryMessages, removalEdits).Process);
            var captureCamDriverEveryQuaterSecond = new SampleFilter(TimeSpan.FromSeconds(0.25),
                 new CaptureCamDriver(overlayData).Process);

            ApplyFirstLapCameraDirection(samples, replayControl);

            samples = samples
                .VerifyReplayFrames()
                .WithCorrectedPercentages()
                .WithCorrectedDistances()
                .WithFastestLaps()
                .WithFinishingStatus()
                .WithPitStopCounts()
                .TakeUntil(3.Seconds()).After(d => d.Telemetry.RaceCars.All(c => c.HasSeenCheckeredFlag || c.HasRetired || c.TrackSurface == TrackLocation.InPitStall))
                .TakeUntil(3.Seconds()).AfterReplayPaused();
            
            if (shortTestOnly)
            {
                samples = samples.AtSpeed(Settings.Default.TimingFactorForShortTest);
                Settings.AppliedTimingFactor = 1.0 / Settings.Default.TimingFactorForShortTest;
            }

            videoCapture.Activate(workingFolder);
            var startTime = DateTime.Now;

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
            }

            var fileName = videoCapture.Deactivate();

            removalEdits.Stop();

            SaveOverlayData(overlayData, fileName);

            iRacing.Replay.SetSpeed(0);

            AltTabBackToApp();

            _WithFiles(fileName);

            if (fileName == null)
                throw new Exception("Unable to determine video file name in '{0}' - possible wrong working folder".F(workingFolder));

            onComplete(fileName);
        }

        static void AltTabBackToApp()
        {
            Keyboard.keybd_event(Keyboard.VK_MENU, 0, 0, UIntPtr.Zero);
            Thread.Sleep(200);
            Keyboard.keybd_event(Keyboard.VK_TAB, 0, 0, UIntPtr.Zero);
            Thread.Sleep(200);
            Keyboard.keybd_event(Keyboard.VK_TAB, 0, Keyboard.KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(200);
            Keyboard.keybd_event(Keyboard.VK_MENU, 0, Keyboard.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        void ApplyFirstLapCameraDirection(IEnumerable<DataSample> samples, ReplayControl replayControl)
        {
            iRacing.Replay.MoveToFrame(raceStartFrameNumber);
            iRacing.Replay.SetSpeed(1);

            iRacing.Replay.Wait();
            Thread.Sleep(1000);

            replayControl.Process(samples.First());
            
            iRacing.Replay.Wait();
            Thread.Sleep(1000);
        }

        void SaveOverlayData(OverlayData overlayData, string fileName)
        {
            if (fileName == null)
                fileName = workingFolder + "/unknown_capture-{0}".F(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

            fileName = Path.ChangeExtension(fileName, ".xml");
            Trace.WriteLine("Saving overlay data to {0}".F(fileName));
            overlayData.SaveTo(fileName);
        }
    }
}
