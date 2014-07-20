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

using Google.GData.Client;
using Google.GData.Client.ResumableUpload;
using Google.GData.Extensions.MediaRss;
using Google.GData.YouTube;
using iRacingReplayOverlay.Phases;
using iRacingReplayOverlay.Support;
using iRacingReplayOverlay.Video;
using iRacingSDK;
using iRacingSDK.Support;
using MediaFoundation.Net;
using System;
using System.Diagnostics;
using System.IO;
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
            switch (_states)
            {
                case States.Idle:
                    BeginProcessButton.Enabled = Directory.Exists(workingFolderTextBox.Text) && iRacing.IsConnected;
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
                    BeginProcessButton.Enabled = Directory.Exists(workingFolderTextBox.Text) && iRacing.IsConnected;
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
            workingFolderTextBox.Text = Settings.Default.WorkingFolder;

            youTubeCredentialsRequired.Visible = Settings.Default.YouTubeCredentials == null || Settings.Default.YouTubeCredentials.Blank;

            logMessagges = new LogMessages();
            Trace.Listeners.Add(new MyListener(logMessagges.TraceMessage));

            LogListener.ToFile(GetDefaultLogFileName());

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
                .OverlayRaceDataOntoVideo(OnTranscoderProgress, OnTranscoderCompleted, OnTranscoderReadFramesCompleted)
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

            if (!File.Exists(sourceVideoTextBox.Text))
                return;

            try
            {
                var details = VideoAttributes.For(sourceVideoTextBox.Text);

                foreach (var d in details.SupportedAudioBitRates)
                    audioBitRate.Items.Add(d);

                audioBitRate.SelectedItem = Settings.Default.audioBitRate;

                VideoDetailLabel.Text = "Frame Rate: {0}, Frame Size: {1}x{2}, Bit Rate: {3}Mb".F(details.FrameRate, details.FrameSize.Width, details.FrameSize.Height, details.BitRate == 0 ? "-- " : details.BitRate.ToString());
            }
            catch(Exception)
            {
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

            errorSourceVideoLabel.Visible = !File.Exists(Path.ChangeExtension(sourceVideoTextBox.Text, ".xml"));
            return (!errorSourceVideoLabel.Visible && File.Exists(sourceVideoTextBox.Text)) && audioBitRateValid;
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
                .CaptureRace((videoFileName, errorMessage) =>
                {
                    ProcessErrorMessageLabel.Text = errorMessage;
                    ProcessErrorMessageLabel.Visible = errorMessage != null;
                    CapturingRaceLabel.Visible = false;
                    sourceVideoTextBox.Text = videoFileName;
                    State = States.Idle;

                    LogListener.MoveToFile(Path.ChangeExtension(videoFileName, "log"));
                })
                .CloseIRacing()
                .InTheBackground(errorMessage =>
                {
                    if (errorMessage == null && transcodeVideoButton.Enabled && EncodeVideoAfterCapture.Checked)
                    {
                        tabControl1.SelectedIndex = 1;
                        TranscodeVideo_Click(null, null);
                    }

                    workingFolderTextBox_TextChanged(null, null);
                    WaitingForIRacingLabel.Visible = false;
                    AnalysingRaceLabel.Visible = false;
                    CapturingRaceLabel.Visible = false;
                    ProcessErrorMessageLabel.Visible = true;
                    ProcessErrorMessageLabel.Text = errorMessage;

                    LogListener.ToFile(GetDefaultLogFileName());

                    WindowState = FormWindowState.Minimized;
                    Show();
                    WindowState = FormWindowState.Normal;
                    this.BringToFront();
                });
        }

        void SettingsButton_Click(object sender, EventArgs e)
        {
            var settings = new ConfigureTrackCameras();
            settings.ShowDialog();
        }

        void configureVideoCaptureButton_Click(object sender, EventArgs e)
        {
            var settings = new ConfigureGeneralSettings();
            settings.ShowDialog();
            youTubeCredentialsRequired.Visible = Settings.Default.YouTubeCredentials.Blank;
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

        void MainUploadVideoFileButton_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog();
            fbd.Filter = "WMV|*.wmv|All files (*.*)|*.*";
            fbd.FileName = sourceVideoTextBox.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
                MainUploadVideoFile.Text = fbd.FileName;
        }

        void HighlightsUploadVideoButton_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog();
            fbd.Filter = "Highlights WMV|*.highlights.wmv|All files (*.*)|*.*";
            fbd.FileName = sourceVideoTextBox.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
                HighlightsUploadVideoFile.Text = fbd.FileName;
        }

        void UploadToYouTubeButton_Click(object sender, EventArgs e)
        {
            UploadToYouTube(HighlightsUploadVideoFile.Text, 
                () => UploadToYouTube(MainUploadVideoFile.Text));
        }

        void UploadToYouTube(string filename, Action completed = null)
        {
            var createUploadUrl = new Uri("http://uploads.gdata.youtube.com/resumable/feeds/api/users/default/uploads");
            var link = new AtomLink(createUploadUrl.AbsoluteUri) { Rel = ResumableUploader.CreateMediaRelation };

            var newVideo = new Google.YouTube.Video
            {
                Title = Path.GetFileNameWithoutExtension(filename),
                Keywords = "SimLimitedRacing, SimLimitediRacing, Sim Limited Racing, Sim Limited iRacing, iRacing, SLR, SimRacing, Gaming, Racing"
            };
            newVideo.Tags.Add(new MediaCategory("Autos", YouTubeNameTable.CategorySchema));
            newVideo.YouTubeEntry.Private = true;
            newVideo.YouTubeEntry.MediaSource = new MediaFileSource(filename, "video/x-ms-wmv");
            newVideo.YouTubeEntry.Links.Add(link);

            var cred = Settings.Default.YouTubeCredentials;
            var cla = new ClientLoginAuthenticator("iRacingReplayOverlay", ServiceNames.YouTube, cred.UserName, cred.FreePassword)
            {
                DeveloperKey = YouTubeKey.ClientId
            };

            var ru = new ResumableUploader(chunkSize: 1);
            ru.AsyncOperationCompleted += (s, e) => {
                ru_AsyncOperationCompleted(s, e);
                if (completed != null)
                    completed();
            };
            ru.AsyncOperationProgress += ru_AsyncOperationProgress;
            ru.InsertAsync(cla, newVideo.YouTubeEntry, "inserter");
            
            EnableForUpload(false);
            UploadProgress.Visible = true;
            uploadingFileLabel.Text = "Uploading " + Path.GetFileName(filename);
            uploadingFileLabel.Visible = true;
        }

        void ru_AsyncOperationProgress(object sender, AsyncOperationProgressEventArgs e)
        {
            UploadProgress.Value = e.ProgressPercentage;
        }

        void ru_AsyncOperationCompleted(object sender, AsyncOperationCompletedEventArgs e)
        {
            EnableForUpload(true);
            UploadProgress.Visible = false;
            uploadingFileLabel.Visible = false;
        }

        void MainUploadVideoFile_TextChanged(object sender, System.EventArgs e)
        {
            EnableUploadButton();
            HighlightsUploadVideoFile.Text = Path.ChangeExtension(MainUploadVideoFile.Text, ".highlights.wmv");
        }

        void HighlightsUploadVideoFile_TextChanged(object sender, System.EventArgs e)
        {
            EnableUploadButton();
        }

        void EnableForUpload(bool enabled)
        {
            MainUploadVideoFile.Enabled = enabled;
            HighlightsUploadVideoFile.Enabled = enabled;
            MainUploadVideoFileButton.Enabled = enabled;
            HighlightsUploadVideoButton.Enabled = enabled;
            UploadToYouTubeButton.Enabled = enabled;
        }

        void EnableUploadButton()
        {
            UploadToYouTubeButton.Enabled = MainUploadVideoFile.Text.Length > 0 && HighlightsUploadVideoFile.Text.Length > 0;
        }
    }
}
