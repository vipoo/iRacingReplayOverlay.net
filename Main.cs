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
using iRacingReplayOverlay.Support;
using iRacingReplayOverlay.Video;
using iRacingSDK;
using iRacingSDK.Support;
using MediaFoundation.Net;
using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    public partial class Main : Form
    {
        System.Windows.Forms.Timer aTimer;
        int guessedProgessedAmount;
        const int GuessFinalizationRequiredSeconds = 25;
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
            iracingEvents.NewSessionData += iracingEvents_NewSessionData;
            iracingEvents.Connected += iracingEvents_Connected;
            iracingEvents.Disconnected += iracingEvents_Disconnected;
            iracingEvents.StartListening();

            workingFolderTextBox.Text = Settings.Default.WorkingFolder;

            logMessagges = new LogMessages();
            Trace.Listeners.Add(new MyListener(logMessagges.TraceMessage));

            LogListener.ToFile(GetDefaultLogFileName());

            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal);
            TraceInfo.WriteLine("Local user config path: {0}", config.FilePath);
            TraceInfo.WriteLine("Application Version: {0}", GetDeployedVersionString());

            fileWatchTimer = new System.Windows.Forms.Timer();
            fileWatchTimer.Interval = 10;
            fileWatchTimer.Tick += (s, a) => OnGameDataFileChanged();
            fileWatchTimer.Start();

            aTimer = new System.Windows.Forms.Timer();
            aTimer.Interval = 500;
            aTimer.Tick += (s, a) => GuessFinializeProgress();

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
            fbd.Filter = "Mpeg 4|*.mp4|AVI Files|*.avi|All files (*.*)|*.*";
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

            iRacingProcess = new IRacingReplay()
                .WithEncodingOf(videoBitRate: videoBitRateNumber * 1000000, audioBitRate: (int)audioBitRate.SelectedItem / 8)
                .WithFiles(sourceFile: sourceVideoTextBox.Text)
                .OverlayRaceDataOntoVideo(OnTranscoderProgress, OnTranscoderCompleted, OnTranscoderReadFramesCompleted, highlightVideoOnly.Checked)
                .InTheBackground(errorMessage => {
                    OnTranscoderCompleted();
                    LogListener.ToFile(GetDefaultLogFileName());
                });
        }

        void OnTranscoderReadFramesCompleted()
        {
            guessedProgessedAmount = (transcodeProgressBar.Maximum - transcodeProgressBar.Value) / ((int)GuessFinalizationRequiredSeconds * 2);
            aTimer.Start();
        }

        void OnTranscoderProgress(long timestamp, long duration)
        {
            duration += GuessFinalizationRequiredSeconds.FromSecondsToNano();
            transcodeProgressBar.Value = (int)(timestamp * transcodeProgressBar.Maximum / duration);
        }

        void OnTranscoderCompleted()
        {
            State = States.Idle;
        }
        
        void GuessFinializeProgress()
        {
            transcodeProgressBar.Value = Math.Min(transcodeProgressBar.Value + guessedProgessedAmount, transcodeProgressBar.Maximum-5);
            if (transcodeProgressBar.Value == transcodeProgressBar.Maximum - 5)
                aTimer.Stop();
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
                errorSourceVideoLabel.Text = "*Video files does not exist";
                errorSourceVideoLabel.Visible = true;
                VideoDetailLabel.Visible = false;
                return;
            }
            
            if( !File.Exists(Path.ChangeExtension(sourceVideoTextBox.Text, ".xml")))
            {
                errorSourceVideoLabel.Text = "*There is no associated captured game data (xml file name based on the name of the source input video)";
                errorSourceVideoLabel.Visible = true;
                VideoDetailLabel.Visible = false;
                return;
            }

            try
            {
                var details = VideoAttributes.For(sourceVideoTextBox.Text);

                foreach (var d in details.SupportedAudioBitRates)
                    audioBitRate.Items.Add(d);

                audioBitRate.SelectedItem = Settings.Default.audioBitRate;

                VideoDetailLabel.Text = "Frame Rate: {0}, Frame Size: {1}x{2}, Bit Rate: {3}Mb".F(details.FrameRate, details.FrameSize.Width, details.FrameSize.Height, details.BitRate == 0 ? "-- " : details.BitRate.ToString());
                errorSourceVideoLabel.Visible = false;
                VideoDetailLabel.Visible = true;
            }
            catch(Exception ex)
            {
                TraceDebug.WriteLine("*Error reading the video file. {0}\r\n{1}".F(ex.Message, ex.StackTrace));

                errorSourceVideoLabel.Text = "*Error reading the video file. {0}".F(ex.Message);
                errorSourceVideoLabel.Visible = true;
                VideoDetailLabel.Visible = false;

                lookForAudioBitRates = new System.Windows.Forms.Timer();
                lookForAudioBitRates.Tick += sourceVideoTextBox_TextChanged;
                lookForAudioBitRates.Interval = 1000;
                lookForAudioBitRates.Start();
            }
        }

        bool IsReadyForTranscoding()
        {
            if (sourceVideoTextBox.Text == null || sourceVideoTextBox.Text.Length == 0)
                return false;

            var audioBitRateValid = audioBitRate.SelectedItem != null;

            var hasXmlFile = File.Exists(Path.ChangeExtension(sourceVideoTextBox.Text, ".xml"));

            return (hasXmlFile && File.Exists(sourceVideoTextBox.Text)) && audioBitRateValid;
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

            iRacingProcess = new IRacingReplay(shortTestOnly: TestOnlyCheckBox.Checked)
                .WithWorkingFolder(workingFolderTextBox.Text)
                .AnalyseRace(() => { AnalysingRaceLabel.Visible = false; CapturingRaceLabel.Visible = true; })
                .CaptureOpeningScenes()
                .CaptureRace(videoFileName =>
                {
                    sourceVideoTextBox.Text = videoFileName;

                    LogListener.MoveToFile(Path.ChangeExtension(videoFileName, "log"));
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
            MessageBox.Show("iRacing Replay Application\nCopyright Dean Netherton\nVersion {0}".F(GetDeployedVersionString()));
        }

        string GetDeployedVersionString()
        {
            var version = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion : new Version("1.0.0.0");

            var isBeta = this.GetType().Assembly.GetName().Name.ToLower().Contains("beta");

            var betaText = isBeta ? " beta" : "";

            return "{0}.{1}.{2}.{3}{4}".F(version.Major, version.MajorRevision, version.Minor, version.MinorRevision, betaText);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f = new TestVideoConversion();
            f.ShowDialog();
        }
    }
}
