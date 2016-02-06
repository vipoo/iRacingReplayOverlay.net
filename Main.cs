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

using iRacingReplayOverlay.Phases;
using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Support;
using iRacingReplayOverlay.Video;
using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    public partial class Main : Form
    {
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern uint SetThreadExecutionState(uint esFlags);
            public const uint ES_CONTINUOUS = 0x80000000;
            public const uint ES_SYSTEM_REQUIRED = 0x00000001;
            public const uint ES_DISPLAY_REQUIRED = 0x00000002;
        }

        System.Windows.Forms.Timer fileWatchTimer;

        int videoBitRateNumber = 15;
        IRacingReplay iRacingProcess;

        enum States {Idle, CapturingGameData, Transcoding};
        States _states = States.Idle;

        System.Windows.Forms.Timer lookForAudioBitRates;
        LogMessages logMessagges;
        const string DefaultLogFileName = "general.log";
        SessionData lastSession;
        bool isConnected;

        States State
        {
            set
            {
                _states = value;
                StateUpdated();
            }
            get
            {
                return _states;
            }
        }

        void StateUpdated()
        {

            var trackCamerasDefined =
                Settings.Default.trackCameras != null &&
                lastSession != null &&
                Settings.Default.trackCameras.Any(tc => tc.TrackName == lastSession.WeekendInfo.TrackDisplayName) &&
                Settings.Default.trackCameras.RaceStart[lastSession.WeekendInfo.TrackDisplayName] != null &&
                Settings.Default.trackCameras.Incident[lastSession.WeekendInfo.TrackDisplayName] != null &&
                Settings.Default.trackCameras.LastLap[lastSession.WeekendInfo.TrackDisplayName] != null;

            switch (_states)
            {
                case States.Idle:
                    BeginProcessButton.Enabled = Directory.Exists(workingFolderTextBox.Text) && isConnected && trackCamerasDefined;
                    configureTrackCamerasLabel.Visible = isConnected && !trackCamerasDefined;
                    transcodeVideoButton.Enabled = IsReadyForTranscoding();
                    transcodeCancelButton.Visible = false;
                    transcodeProgressBar.Visible = false;
                    transcodeProgressBar.Value = 0;
                    videoBitRate.Enabled =
                    audioBitRate.Enabled =
                    workingFolderTextBox.Enabled =
                    workingFolderButton.Enabled =
                    sourceVideoTextBox.Enabled =
                    sourceVideoButton.Enabled = true;
                    break;

                case States.CapturingGameData:
                    BeginProcessButton.Enabled = false;
                    transcodeVideoButton.Enabled = false;
                    transcodeCancelButton.Visible = false;
                    transcodeProgressBar.Visible = false;
                    videoBitRate.Enabled =
                    audioBitRate.Enabled =
                    workingFolderTextBox.Enabled =
                    workingFolderButton.Enabled =
                    sourceVideoTextBox.Enabled =
                    sourceVideoButton.Enabled = false;
                    break;

                case States.Transcoding:
                    BeginProcessButton.Enabled = Directory.Exists(workingFolderTextBox.Text) && isConnected && trackCamerasDefined;
                    configureTrackCamerasLabel.Visible = isConnected && !trackCamerasDefined;
                    transcodeVideoButton.Enabled = false;
                    transcodeCancelButton.Visible = true;
                    transcodeProgressBar.Visible = true;
                    videoBitRate.Enabled =
                    audioBitRate.Enabled =
                    workingFolderTextBox.Enabled =
                    workingFolderButton.Enabled =
                    sourceVideoTextBox.Enabled =
                    sourceVideoButton.Enabled = false;
                    break;
            }
        }

        public Main()
        {
            iracingEvents = new iRacingEvents();

            InitializeComponent();
        }

        string GetDefaultLogFileName()
        {
            if (Directory.Exists(workingFolderTextBox.Text))
                return Path.Combine(workingFolderTextBox.Text, "general.log");

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "general.log");
        }

        void Main_Load(object sender, EventArgs e)
        {
            changeVersionButton.Visible = File.Exists(Settings.Default.MainExecPath);

            Settings.Default.SettingChanging += Default_SettingChanging;
            iracingEvents.NewSessionData += iracingEvents_NewSessionData;
            iracingEvents.Connected += iracingEvents_Connected;
            iracingEvents.Disconnected += iracingEvents_Disconnected;
            iracingEvents.StartListening();

            workingFolderTextBox.Text = Settings.Default.WorkingFolder;

            logMessagges = new LogMessages();
            Trace.Listeners.Add(new MyListener(logMessagges.TraceMessage));

            LogListener.ToFile(GetDefaultLogFileName());

            new Task(LogSystemInformation).Start();

            fileWatchTimer = new System.Windows.Forms.Timer();
            fileWatchTimer.Interval = 10;
            fileWatchTimer.Tick += (s, a) => OnGameDataFileChanged();
            fileWatchTimer.Start();

            videoBitRate.Text = Settings.Default.videoBitRate.ToString();
            sourceVideoTextBox.Text = Settings.Default.lastVideoFile;

            BeginProcessButton.Enabled = false;

            iRacingProcess = new IRacingReplay()
                .WhenIRacingStarts(() => 
                {
                    BeginProcessButton.Enabled = true;
                    workingFolderTextBox_TextChanged(null, null); 
                    ProcessErrorMessageLabel.Visible = false; 
                    WaitingForIRacingLabel.Visible = false;
                })
                .InTheBackground(errorMessage => { });
        }

        void LogSystemInformation()
        {
            var StringBuilder1 = new StringBuilder(string.Empty);
            try
            {
                var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal);
                TraceDebug.WriteLine("Local user config path: {0}", config.FilePath);
                TraceDebug.WriteLine("Application Version: {0}", AboutBox1.AssemblyVersion);

                TraceDebug.WriteLine(Environment.Is64BitOperatingSystem ? "OS: 64x" : "OS: 32x");
                TraceDebug.WriteLine("ProcessorCount:  {0}".F(Environment.ProcessorCount));

                foreach (DriveInfo DriveInfo1 in DriveInfo.GetDrives())
                    TraceDebug.WriteLine("Drive: {0}, Type: {1}, Format: {2}, Size: {3}, AvailableFreeSpace: {4}".F(
                        GetValue(() => DriveInfo1.Name),
                        GetValue(() => DriveInfo1.DriveType.ToString()),
                        GetValue(() => DriveInfo1.DriveFormat),
                        GetValue(() => DriveInfo1.TotalSize.ToString()), 
                        GetValue(() => DriveInfo1.AvailableFreeSpace.ToString())));

                TraceDebug.WriteLine("SystemPageSize:  {0}".F(Environment.SystemPageSize));
                TraceDebug.WriteLine("Version:  {0}".F(Environment.Version));

                LogDeviceInformation("Win32_Processor");
                LogDeviceInformation("Win32_VideoController");
            }
            catch
            {
            }
        }

        private string GetValue(Func<string> fn)
        {
            try
            {
                return fn();
            }
            catch
            {
                return "";
            }
        }

        void LogDeviceInformation(string stringIn)
        {
            var mc = new ManagementClass(stringIn);
            var instances = mc.GetInstances();
            var properties = mc.Properties;
            foreach (var instance in instances)
                foreach (var property in properties)
                    TraceDebug.WriteLine("{0}: {1}".F(property.Name, GetValue(() => instance.Properties[property.Name].Value.ToString())));
        }

        private void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            try
            {
                TraceDebug.WriteLine("Setting Changed: key: {0}, name: {1}, value: {2}".F(e.SettingKey, e.SettingName, e.NewValue));
            }
            catch(Exception ex)
            {
                TraceDebug.WriteLine("Unable to log setting change");
                TraceDebug.WriteLine(ex.Message);
            }
        }

        void iracingEvents_Disconnected()
        {
            isConnected = false;
            StateUpdated();
        }

        void iracingEvents_Connected()
        {
            isConnected = true;
            StateUpdated();
        }

        void iracingEvents_NewSessionData(DataSample data)
        {
            lastSession = data.SessionData;
            StateUpdated();
        }

        void workingFolderButton_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = workingFolderTextBox.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
                workingFolderTextBox.Text = fbd.SelectedPath;
        }

        void sourceVideoButton_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog();
            fbd.Filter = "Replay Script Data|*.replayscript|XML|*.xml|All files (*.*)|*.*";
            fbd.FileName = sourceVideoTextBox.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
                sourceVideoTextBox.Text = fbd.FileName;
        }

        void TranscodeVideo_Click(object sender, EventArgs e)
        {
            Settings.Default.audioBitRate = (int)audioBitRate.SelectedItem;
            Settings.Default.Save();

            State = States.Transcoding;

            LogListener.ToFile(Path.ChangeExtension(sourceVideoTextBox.Text, "log"));

            NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);

            iRacingProcess = new IRacingReplay()
                .WithEncodingOf(videoBitRate: videoBitRateNumber * 1000000, audioBitRate: (int)audioBitRate.SelectedItem / 8)
                .WithOverlayFile(overlayFile: sourceVideoTextBox.Text)
                .OverlayRaceDataOntoVideo(OnTranscoderProgress, OnTranscoderCompleted, highlightVideoOnly.Checked)
                .InTheBackground(errorMessage => {
                    OnTranscoderCompleted();
                    LogListener.ToFile(GetDefaultLogFileName());
                    NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS);
                });
        }

        void OnTranscoderProgress(long timestamp, long duration)
        {
            transcodeProgressBar.Value = Math.Min(transcodeProgressBar.Maximum, (int)(timestamp * transcodeProgressBar.Maximum / duration));
        }

        void OnTranscoderCompleted()
        {
            State = States.Idle;
        }
        
        void transcodeCancel_Click(object sender, EventArgs e)
        {
            iRacingProcess.RequestAbort();
            State = States.Idle;
        }

        void sourceVideoTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.lastVideoFile = sourceVideoTextBox.Text;
            Settings.Default.Save();

            if (lookForAudioBitRates != null )
            {
                lookForAudioBitRates.Stop();
                lookForAudioBitRates.Dispose();
                lookForAudioBitRates = null;
            }

            OnGameDataFileChanged();
            audioBitRate.Items.Clear();

            if (sourceVideoTextBox.Text.Trim() == "")
            {
                errorSourceVideoLabel.Visible = false;
                VideoDetailLabel.Visible = false;
                return;

            }

            if (!File.Exists(sourceVideoTextBox.Text) ) 
            {
                errorSourceVideoLabel.Text = "*File does not exist";
                errorSourceVideoLabel.Visible = true;
                VideoDetailLabel.Visible = false;
                return;
            }
            
            try
            {
                var data = OverlayData.FromFile(sourceVideoTextBox.Text);
                var fileName = data.VideoFiles.Last().FileName;

                if (!File.Exists(fileName))
                {
                    errorSourceVideoLabel.Text = "*Captured video does not exist: " + fileName;
                    errorSourceVideoLabel.Visible = true;
                    VideoDetailLabel.Visible = false;
                    return;
                }

                var details = VideoAttributes.For(fileName);

                foreach (var d in details.SupportedAudioBitRates)
                    audioBitRate.Items.Add(d);

                audioBitRate.SelectedItem = Settings.Default.audioBitRate;

                VideoDetailLabel.Text = "Frame Rate: {0}, Frame Size: {1}x{2}, Bit Rate: {3}Mb".F(details.FrameRate, details.FrameSize.Width, details.FrameSize.Height, details.BitRate == 0 ? "-- " : details.BitRate.ToString());
                errorSourceVideoLabel.Visible = false;
                VideoDetailLabel.Visible = true;
            }
            catch(Exception ex)
            {
                errorSourceVideoLabel.Text = "*Error reading the video file. {0}".F(ex.Message);
                errorSourceVideoLabel.Visible = true;
                VideoDetailLabel.Visible = false;

                lookForAudioBitRates = new System.Windows.Forms.Timer();
                lookForAudioBitRates.Tick += sourceVideoTextBox_TextChanged;
                lookForAudioBitRates.Interval = 5000;
                lookForAudioBitRates.Start();
            }
        }

        bool IsReadyForTranscoding()
        {
            if (sourceVideoTextBox.Text == null || sourceVideoTextBox.Text.Length == 0)
                return false;

            var audioBitRateValid = audioBitRate.SelectedItem != null;

            var hasXmlFile = File.Exists(sourceVideoTextBox.Text);

            return hasXmlFile && audioBitRateValid;
        }

        void OnGameDataFileChanged()
        {
            if (State == States.Idle)
                transcodeVideoButton.Enabled = IsReadyForTranscoding();
        }

        void videoBitRate_TextChanged(object sender, EventArgs e)
        {
            if(int.TryParse(videoBitRate.Text, out videoBitRateNumber))
            {
                Settings.Default.videoBitRate = videoBitRateNumber;
                Settings.Default.Save();
            }
        }

        void BeginProcessButton_Click(object sender, EventArgs e)
        {
            BeginProcessButton.Enabled = false;
            AnalysingRaceLabel.Visible = true;
            State = States.CapturingGameData;

            LogListener.ToFile(workingFolderTextBox.Text + "\\capture.log");

            NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED | NativeMethods.ES_DISPLAY_REQUIRED);

            iRacingProcess = new IRacingReplay(shortTestOnly: TestOnlyCheckBox.Checked)
                .WithWorkingFolder(workingFolderTextBox.Text)
                .AnalyseRace(() => { AnalysingRaceLabel.Visible = false; CapturingRaceLabel.Visible = true; })
                .CaptureOpeningScenes()
                .CaptureRace(overlayFileName =>
                {
                    sourceVideoTextBox.Text = overlayFileName;

                    LogListener.MoveToFile(Path.ChangeExtension(overlayFileName, "log"));
                })
                .CloseIRacing()
                .InTheBackground(errorMessage =>
                {
                    workingFolderTextBox_TextChanged(null, null);
                    WaitingForIRacingLabel.Visible = false;
                    AnalysingRaceLabel.Visible = false;
                    CapturingRaceLabel.Visible = false;
                    ProcessErrorMessageLabel.Visible = true;
                    ProcessErrorMessageLabel.Text = errorMessage;
                    State = States.Idle;
                    StateUpdated();

                    LogListener.ToFile(GetDefaultLogFileName());

                    WindowState = FormWindowState.Minimized;
                    Show();
                    WindowState = FormWindowState.Normal;
                    this.BringToFront();

                    if (errorMessage == null && transcodeVideoButton.Enabled && EncodeVideoAfterCapture.Checked)
                    {
                        tabControl1.SelectedIndex = 1;
                        Thread.Sleep(1000);
                        TranscodeVideo_Click(null, null);
                    }
                    else
                        NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS);
                });
        }

        void SettingsButton_Click(object sender, EventArgs e)
        {
            var settings = new ConfigureTrackCameras();
            settings.ShowDialog();
            StateUpdated();
        }

        void configureVideoCaptureButton_Click(object sender, EventArgs e)
        {
            var settings = new ConfigureGeneralSettings();
            settings.ShowDialog();
        }

        void logMessagesButton_Click(object sender, EventArgs e)
        {
            logMessagges.Show();
        }

        void workingFolderTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.WorkingFolder = workingFolderTextBox.Text;
            Settings.Default.Save();

            StateUpdated();
        }

        public iRacingEvents iracingEvents { get; set; }

        private void verifyVideoCaptureButton_Click(object sender, EventArgs e)
        {
            var f = new TestVideoCapture();
            f.ShowDialog();

            workingFolderTextBox.Text = Settings.Default.WorkingFolder;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (new AboutBox1()).ShowDialog(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f = new TestVideoConversion();
            f.ShowDialog();
        }

        private void Main_Activated(object sender, EventArgs e)
        {
            if(!AwsKeys.HaveKeys)
                return;

            if(!Settings.Default.HaveAskedAboutUsage)
            {
                Settings.Default.HaveAskedAboutUsage = true;
                Settings.Default.Save();
                (new UsageDataRequest()).ShowDialog();
            }
        }

        private void changeVersionButton_Click(object sender, EventArgs e)
        {
            Process.Start(Settings.Default.MainExecPath, "-update");
            this.Close();
        }

        private void configurePluginsButton_Click(object sender, EventArgs e)
        {
            var f = new ConfigurePlugins();
            f.ShowDialog();
        }
    }
}
