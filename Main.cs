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

using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Phases.Transcoding;
using iRacingReplayOverlay.Support;
using iRacingReplayOverlay;
using iRacingReplayOverlay.Phases;
using iRacingReplayOverlay.Video;
using MediaFoundation.Net;
using System;
using System.Collections.Generic;
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

        States State
        {
            set
            {
                _states = value;
                switch(_states)
                {
                    case States.Idle:
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
            get
            {
                return _states;
            }
        }

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            logMessagges = new LogMessages();
            Trace.Listeners.Add(new MyListener(logMessagges.TraceMessage));

            fileWatchTimer = new System.Windows.Forms.Timer();
            fileWatchTimer.Interval = 10;
            fileWatchTimer.Tick += (s, a) => OnGameDataFileChanged();
            fileWatchTimer.Start();

            aTimer = new System.Windows.Forms.Timer();
            aTimer.Interval = 500;
            aTimer.Tick += (s, a) => GuessFinializeProgress();

            workingFolderTextBox.Text = Settings.Default.WorkingFolder;
            videoBitRate.Text = Settings.Default.videoBitRate.ToString();
            sourceVideoTextBox.Text = Settings.Default.lastVideoFile;

            iRacingProcess = new IRacingReplay()
                .WhenIRacingStarts(() => { BeginProcessButton.Enabled = true; ProcessErrorMessageLabel.Visible = false; WaitingForIRacingLabel.Visible = false; })
                .InTheBackground(errorMessage => { });
        }

        void workingFolderButton_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = workingFolderTextBox.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.WorkingFolder = workingFolderTextBox.Text = fbd.SelectedPath;
                Settings.Default.Save();
            }
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

            iRacingProcess
                .WithEncodingOf(videoBitRate: videoBitRateNumber * 1000000, audioBitRate: (int)audioBitRate.SelectedItem / 8)
                .WithFiles(sourceFile: sourceVideoTextBox.Text)
                .OverlayRaceDataOntoVideo(OnTranscoderProgress, OnTranscoderCompleted, OnTranscoderReadFramesCompleted)
                .InTheBackground(errorMessage => {
                    OnTranscoderCompleted();
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

        System.Windows.Forms.Timer lookForAudioBitRates;
        private LogMessages logMessagges;

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

        private void videoBitRate_TextChanged(object sender, EventArgs e)
        {
            if(int.TryParse(videoBitRate.Text, out videoBitRateNumber))
            {
                Settings.Default.videoBitRate = videoBitRateNumber;
                Settings.Default.Save();
            }
        }

        private void BeginProcessButton_Click(object sender, EventArgs e)
        {
            BeginProcessButton.Enabled = false;
            AnalysingRaceLabel.Visible = true;
            State = States.CapturingGameData;

            iRacingProcess
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
                })
                .CloseIRacing()
                .InTheBackground(errorMessage =>
                {
                    BeginProcessButton.Enabled = true;
                    WaitingForIRacingLabel.Visible = false;
                    AnalysingRaceLabel.Visible = false;
                    CapturingRaceLabel.Visible = false;
                    ProcessErrorMessageLabel.Visible = true;
                    ProcessErrorMessageLabel.Text = errorMessage;

                });
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            var settings = new ConfigureTrackCameras();
            settings.ShowDialog();
        }

        private void configureVideoCaptureButton_Click(object sender, EventArgs e)
        {
            var settings = new ConfigureGeneralSettings();
            settings.ShowDialog();
        }

        private void logMessagesButton_Click(object sender, EventArgs e)
        {
            logMessagges.Show();
        }
    }
}
