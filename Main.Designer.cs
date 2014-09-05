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

namespace iRacingReplayOverlay
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SettingsButton = new System.Windows.Forms.Button();
            this.configureGeneralSettingsButton = new System.Windows.Forms.Button();
            this.logMessagesButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCapture = new System.Windows.Forms.TabPage();
            this.verifyVideoCaptureButton = new System.Windows.Forms.Button();
            this.configureTrackCamerasLabel = new System.Windows.Forms.Label();
            this.TestOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.WaitingForIRacingLabel = new System.Windows.Forms.Label();
            this.ProcessErrorMessageLabel = new System.Windows.Forms.Label();
            this.CapturingRaceLabel = new System.Windows.Forms.Label();
            this.AnalysingRaceLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BeginProcessButton = new System.Windows.Forms.Button();
            this.workingFolderButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.workingFolderTextBox = new System.Windows.Forms.TextBox();
            this.tabTranscoding = new System.Windows.Forms.TabPage();
            this.highlightVideoOnly = new System.Windows.Forms.CheckBox();
            this.VideoDetailLabel = new System.Windows.Forms.Label();
            this.audioBitRate = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.videoBitRate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.errorSourceVideoLabel = new System.Windows.Forms.Label();
            this.sourceVideoButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.sourceVideoTextBox = new System.Windows.Forms.TextBox();
            this.transcodeCancelButton = new System.Windows.Forms.Button();
            this.transcodeProgressBar = new System.Windows.Forms.ProgressBar();
            this.transcodeVideoButton = new System.Windows.Forms.Button();
            this.tabUploading = new System.Windows.Forms.TabPage();
            this.UploadProgress = new System.Windows.Forms.ProgressBar();
            this.uploadingFileLabel = new System.Windows.Forms.Label();
            this.HighlightsUploadVideoButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.HighlightsUploadVideoFile = new System.Windows.Forms.TextBox();
            this.UploadToYouTubeButton = new System.Windows.Forms.Button();
            this.MainUploadVideoFileButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.MainUploadVideoFile = new System.Windows.Forms.TextBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.UploadVideoToYouTube = new System.Windows.Forms.CheckBox();
            this.EncodeVideoAfterCapture = new System.Windows.Forms.CheckBox();
            this.youTubeCredentialsRequired = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabCapture.SuspendLayout();
            this.tabTranscoding.SuspendLayout();
            this.tabUploading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // SettingsButton
            // 
            this.SettingsButton.Location = new System.Drawing.Point(210, 12);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(181, 26);
            this.SettingsButton.TabIndex = 30;
            this.SettingsButton.Text = "Configure Track Cameras";
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // configureGeneralSettingsButton
            // 
            this.configureGeneralSettingsButton.Location = new System.Drawing.Point(13, 12);
            this.configureGeneralSettingsButton.Name = "configureGeneralSettingsButton";
            this.configureGeneralSettingsButton.Size = new System.Drawing.Size(181, 26);
            this.configureGeneralSettingsButton.TabIndex = 31;
            this.configureGeneralSettingsButton.Text = "Configure General Settings";
            this.configureGeneralSettingsButton.UseVisualStyleBackColor = true;
            this.configureGeneralSettingsButton.Click += new System.EventHandler(this.configureVideoCaptureButton_Click);
            // 
            // logMessagesButton
            // 
            this.logMessagesButton.Location = new System.Drawing.Point(586, 12);
            this.logMessagesButton.Name = "logMessagesButton";
            this.logMessagesButton.Size = new System.Drawing.Size(181, 26);
            this.logMessagesButton.TabIndex = 32;
            this.logMessagesButton.Text = "View Log Messages";
            this.logMessagesButton.UseVisualStyleBackColor = true;
            this.logMessagesButton.Click += new System.EventHandler(this.logMessagesButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabCapture);
            this.tabControl1.Controls.Add(this.tabTranscoding);
            this.tabControl1.Controls.Add(this.tabUploading);
            this.tabControl1.Location = new System.Drawing.Point(13, 70);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(754, 321);
            this.tabControl1.TabIndex = 34;
            // 
            // tabCapture
            // 
            this.tabCapture.Controls.Add(this.verifyVideoCaptureButton);
            this.tabCapture.Controls.Add(this.configureTrackCamerasLabel);
            this.tabCapture.Controls.Add(this.TestOnlyCheckBox);
            this.tabCapture.Controls.Add(this.WaitingForIRacingLabel);
            this.tabCapture.Controls.Add(this.ProcessErrorMessageLabel);
            this.tabCapture.Controls.Add(this.CapturingRaceLabel);
            this.tabCapture.Controls.Add(this.AnalysingRaceLabel);
            this.tabCapture.Controls.Add(this.label2);
            this.tabCapture.Controls.Add(this.BeginProcessButton);
            this.tabCapture.Controls.Add(this.workingFolderButton);
            this.tabCapture.Controls.Add(this.label5);
            this.tabCapture.Controls.Add(this.workingFolderTextBox);
            this.tabCapture.Location = new System.Drawing.Point(4, 30);
            this.tabCapture.Name = "tabCapture";
            this.tabCapture.Padding = new System.Windows.Forms.Padding(3);
            this.tabCapture.Size = new System.Drawing.Size(746, 287);
            this.tabCapture.TabIndex = 0;
            this.tabCapture.Text = "Race Capture";
            this.tabCapture.UseVisualStyleBackColor = true;
            // 
            // verifyVideoCaptureButton
            // 
            this.verifyVideoCaptureButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.verifyVideoCaptureButton.Location = new System.Drawing.Point(586, 20);
            this.verifyVideoCaptureButton.Margin = new System.Windows.Forms.Padding(4);
            this.verifyVideoCaptureButton.Name = "verifyVideoCaptureButton";
            this.verifyVideoCaptureButton.Size = new System.Drawing.Size(153, 28);
            this.verifyVideoCaptureButton.TabIndex = 50;
            this.verifyVideoCaptureButton.Text = "Verify Video Capture";
            this.verifyVideoCaptureButton.UseVisualStyleBackColor = true;
            this.verifyVideoCaptureButton.Click += new System.EventHandler(this.verifyVideoCaptureButton_Click);
            // 
            // configureTrackCamerasLabel
            // 
            this.configureTrackCamerasLabel.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.configureTrackCamerasLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.configureTrackCamerasLabel.Location = new System.Drawing.Point(6, 195);
            this.configureTrackCamerasLabel.Name = "configureTrackCamerasLabel";
            this.configureTrackCamerasLabel.Size = new System.Drawing.Size(279, 28);
            this.configureTrackCamerasLabel.TabIndex = 49;
            this.configureTrackCamerasLabel.Text = "You need to configure the Track Cameras before you can begin to capture this race" +
    ".";
            this.configureTrackCamerasLabel.Visible = false;
            // 
            // TestOnlyCheckBox
            // 
            this.TestOnlyCheckBox.AutoSize = true;
            this.TestOnlyCheckBox.Location = new System.Drawing.Point(195, 156);
            this.TestOnlyCheckBox.Name = "TestOnlyCheckBox";
            this.TestOnlyCheckBox.Size = new System.Drawing.Size(120, 22);
            this.TestOnlyCheckBox.TabIndex = 43;
            this.TestOnlyCheckBox.Text = "Short Test Only";
            this.TestOnlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // WaitingForIRacingLabel
            // 
            this.WaitingForIRacingLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.WaitingForIRacingLabel.Location = new System.Drawing.Point(192, 111);
            this.WaitingForIRacingLabel.Name = "WaitingForIRacingLabel";
            this.WaitingForIRacingLabel.Size = new System.Drawing.Size(547, 39);
            this.WaitingForIRacingLabel.TabIndex = 42;
            this.WaitingForIRacingLabel.Text = "Waiting for iRacing to start ...";
            // 
            // ProcessErrorMessageLabel
            // 
            this.ProcessErrorMessageLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.ProcessErrorMessageLabel.Location = new System.Drawing.Point(192, 111);
            this.ProcessErrorMessageLabel.Name = "ProcessErrorMessageLabel";
            this.ProcessErrorMessageLabel.Size = new System.Drawing.Size(547, 47);
            this.ProcessErrorMessageLabel.TabIndex = 41;
            this.ProcessErrorMessageLabel.Text = "Error ...";
            this.ProcessErrorMessageLabel.Visible = false;
            // 
            // CapturingRaceLabel
            // 
            this.CapturingRaceLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.CapturingRaceLabel.Location = new System.Drawing.Point(192, 111);
            this.CapturingRaceLabel.Name = "CapturingRaceLabel";
            this.CapturingRaceLabel.Size = new System.Drawing.Size(406, 47);
            this.CapturingRaceLabel.TabIndex = 40;
            this.CapturingRaceLabel.Text = "Capturing your replay to video ...";
            this.CapturingRaceLabel.Visible = false;
            // 
            // AnalysingRaceLabel
            // 
            this.AnalysingRaceLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.AnalysingRaceLabel.Location = new System.Drawing.Point(192, 111);
            this.AnalysingRaceLabel.Name = "AnalysingRaceLabel";
            this.AnalysingRaceLabel.Size = new System.Drawing.Size(406, 47);
            this.AnalysingRaceLabel.TabIndex = 39;
            this.AnalysingRaceLabel.Text = "Analysing your race replay ...";
            this.AnalysingRaceLabel.Visible = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(594, 52);
            this.label2.TabIndex = 38;
            this.label2.Text = "Load iRacing with your race replay. You may need to press the space bar to remove" +
    " the iRacing overlay.";
            // 
            // BeginProcessButton
            // 
            this.BeginProcessButton.Enabled = false;
            this.BeginProcessButton.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BeginProcessButton.Location = new System.Drawing.Point(7, 111);
            this.BeginProcessButton.Margin = new System.Windows.Forms.Padding(4);
            this.BeginProcessButton.Name = "BeginProcessButton";
            this.BeginProcessButton.Size = new System.Drawing.Size(163, 64);
            this.BeginProcessButton.TabIndex = 37;
            this.BeginProcessButton.Text = "Begin Capture";
            this.BeginProcessButton.UseVisualStyleBackColor = true;
            this.BeginProcessButton.Click += new System.EventHandler(this.BeginProcessButton_Click);
            // 
            // workingFolderButton
            // 
            this.workingFolderButton.Location = new System.Drawing.Point(515, 20);
            this.workingFolderButton.Name = "workingFolderButton";
            this.workingFolderButton.Size = new System.Drawing.Size(64, 26);
            this.workingFolderButton.TabIndex = 36;
            this.workingFolderButton.Text = "browse";
            this.workingFolderButton.UseVisualStyleBackColor = true;
            this.workingFolderButton.Click += new System.EventHandler(this.workingFolderButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 18);
            this.label5.TabIndex = 35;
            this.label5.Text = "Video Capture Folder:";
            // 
            // workingFolderTextBox
            // 
            this.workingFolderTextBox.Location = new System.Drawing.Point(154, 20);
            this.workingFolderTextBox.Name = "workingFolderTextBox";
            this.workingFolderTextBox.Size = new System.Drawing.Size(355, 26);
            this.workingFolderTextBox.TabIndex = 34;
            this.workingFolderTextBox.TextChanged += new System.EventHandler(this.workingFolderTextBox_TextChanged);
            // 
            // tabTranscoding
            // 
            this.tabTranscoding.Controls.Add(this.highlightVideoOnly);
            this.tabTranscoding.Controls.Add(this.VideoDetailLabel);
            this.tabTranscoding.Controls.Add(this.audioBitRate);
            this.tabTranscoding.Controls.Add(this.label4);
            this.tabTranscoding.Controls.Add(this.videoBitRate);
            this.tabTranscoding.Controls.Add(this.label1);
            this.tabTranscoding.Controls.Add(this.errorSourceVideoLabel);
            this.tabTranscoding.Controls.Add(this.sourceVideoButton);
            this.tabTranscoding.Controls.Add(this.label3);
            this.tabTranscoding.Controls.Add(this.sourceVideoTextBox);
            this.tabTranscoding.Controls.Add(this.transcodeCancelButton);
            this.tabTranscoding.Controls.Add(this.transcodeProgressBar);
            this.tabTranscoding.Controls.Add(this.transcodeVideoButton);
            this.tabTranscoding.Location = new System.Drawing.Point(4, 30);
            this.tabTranscoding.Name = "tabTranscoding";
            this.tabTranscoding.Padding = new System.Windows.Forms.Padding(3);
            this.tabTranscoding.Size = new System.Drawing.Size(746, 287);
            this.tabTranscoding.TabIndex = 1;
            this.tabTranscoding.Text = "Video Encoding";
            this.tabTranscoding.UseVisualStyleBackColor = true;
            // 
            // highlightVideoOnly
            // 
            this.highlightVideoOnly.AutoSize = true;
            this.highlightVideoOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.highlightVideoOnly.Location = new System.Drawing.Point(425, 87);
            this.highlightVideoOnly.Name = "highlightVideoOnly";
            this.highlightVideoOnly.Size = new System.Drawing.Size(155, 22);
            this.highlightVideoOnly.TabIndex = 47;
            this.highlightVideoOnly.Text = "Highlight Video Only";
            this.highlightVideoOnly.UseVisualStyleBackColor = true;
            // 
            // VideoDetailLabel
            // 
            this.VideoDetailLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.VideoDetailLabel.Location = new System.Drawing.Point(134, 157);
            this.VideoDetailLabel.Name = "VideoDetailLabel";
            this.VideoDetailLabel.Size = new System.Drawing.Size(464, 21);
            this.VideoDetailLabel.TabIndex = 40;
            // 
            // audioBitRate
            // 
            this.audioBitRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.audioBitRate.FormattingEnabled = true;
            this.audioBitRate.Location = new System.Drawing.Point(565, 52);
            this.audioBitRate.Name = "audioBitRate";
            this.audioBitRate.Size = new System.Drawing.Size(173, 26);
            this.audioBitRate.TabIndex = 39;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(426, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 18);
            this.label4.TabIndex = 38;
            this.label4.Text = "Audio Bitrate Kbps";
            // 
            // videoBitRate
            // 
            this.videoBitRate.Location = new System.Drawing.Point(565, 20);
            this.videoBitRate.Name = "videoBitRate";
            this.videoBitRate.Size = new System.Drawing.Size(43, 26);
            this.videoBitRate.TabIndex = 37;
            this.videoBitRate.Text = "15";
            this.videoBitRate.TextChanged += new System.EventHandler(this.videoBitRate_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(426, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 18);
            this.label1.TabIndex = 36;
            this.label1.Text = "Video Bitrate (Mbs):";
            // 
            // errorSourceVideoLabel
            // 
            this.errorSourceVideoLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.errorSourceVideoLabel.Location = new System.Drawing.Point(131, 182);
            this.errorSourceVideoLabel.Name = "errorSourceVideoLabel";
            this.errorSourceVideoLabel.Size = new System.Drawing.Size(467, 41);
            this.errorSourceVideoLabel.TabIndex = 35;
            this.errorSourceVideoLabel.Text = "*Unable to transcode this video, as there is no associated captured game data (cs" +
    "v file name based on the name of the source input video)";
            this.errorSourceVideoLabel.Visible = false;
            // 
            // sourceVideoButton
            // 
            this.sourceVideoButton.Location = new System.Drawing.Point(674, 127);
            this.sourceVideoButton.Name = "sourceVideoButton";
            this.sourceVideoButton.Size = new System.Drawing.Size(64, 26);
            this.sourceVideoButton.TabIndex = 34;
            this.sourceVideoButton.Text = "browse";
            this.sourceVideoButton.UseVisualStyleBackColor = true;
            this.sourceVideoButton.Click += new System.EventHandler(this.sourceVideoButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 18);
            this.label3.TabIndex = 33;
            this.label3.Text = "Source Video:";
            // 
            // sourceVideoTextBox
            // 
            this.sourceVideoTextBox.Location = new System.Drawing.Point(134, 128);
            this.sourceVideoTextBox.Name = "sourceVideoTextBox";
            this.sourceVideoTextBox.Size = new System.Drawing.Size(534, 26);
            this.sourceVideoTextBox.TabIndex = 32;
            this.sourceVideoTextBox.TextChanged += new System.EventHandler(this.sourceVideoTextBox_TextChanged);
            // 
            // transcodeCancelButton
            // 
            this.transcodeCancelButton.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transcodeCancelButton.Location = new System.Drawing.Point(175, 20);
            this.transcodeCancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.transcodeCancelButton.Name = "transcodeCancelButton";
            this.transcodeCancelButton.Size = new System.Drawing.Size(163, 64);
            this.transcodeCancelButton.TabIndex = 31;
            this.transcodeCancelButton.Text = "Cancel Transcoding";
            this.transcodeCancelButton.UseVisualStyleBackColor = true;
            this.transcodeCancelButton.Visible = false;
            this.transcodeCancelButton.Click += new System.EventHandler(this.transcodeCancel_Click);
            // 
            // transcodeProgressBar
            // 
            this.transcodeProgressBar.Location = new System.Drawing.Point(23, 232);
            this.transcodeProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this.transcodeProgressBar.Maximum = 10000;
            this.transcodeProgressBar.Name = "transcodeProgressBar";
            this.transcodeProgressBar.Size = new System.Drawing.Size(700, 32);
            this.transcodeProgressBar.TabIndex = 30;
            // 
            // transcodeVideoButton
            // 
            this.transcodeVideoButton.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transcodeVideoButton.Location = new System.Drawing.Point(4, 20);
            this.transcodeVideoButton.Margin = new System.Windows.Forms.Padding(4);
            this.transcodeVideoButton.Name = "transcodeVideoButton";
            this.transcodeVideoButton.Size = new System.Drawing.Size(163, 64);
            this.transcodeVideoButton.TabIndex = 29;
            this.transcodeVideoButton.Text = "Transcode Video";
            this.transcodeVideoButton.UseVisualStyleBackColor = true;
            this.transcodeVideoButton.Click += new System.EventHandler(this.TranscodeVideo_Click);
            // 
            // tabUploading
            // 
            this.tabUploading.Controls.Add(this.UploadProgress);
            this.tabUploading.Controls.Add(this.uploadingFileLabel);
            this.tabUploading.Controls.Add(this.HighlightsUploadVideoButton);
            this.tabUploading.Controls.Add(this.label7);
            this.tabUploading.Controls.Add(this.HighlightsUploadVideoFile);
            this.tabUploading.Controls.Add(this.UploadToYouTubeButton);
            this.tabUploading.Controls.Add(this.MainUploadVideoFileButton);
            this.tabUploading.Controls.Add(this.label6);
            this.tabUploading.Controls.Add(this.MainUploadVideoFile);
            this.tabUploading.Location = new System.Drawing.Point(4, 30);
            this.tabUploading.Name = "tabUploading";
            this.tabUploading.Size = new System.Drawing.Size(746, 287);
            this.tabUploading.TabIndex = 2;
            this.tabUploading.Text = "Video Publishing";
            this.tabUploading.UseVisualStyleBackColor = true;
            // 
            // UploadProgress
            // 
            this.UploadProgress.Location = new System.Drawing.Point(15, 140);
            this.UploadProgress.Margin = new System.Windows.Forms.Padding(4);
            this.UploadProgress.Name = "UploadProgress";
            this.UploadProgress.Size = new System.Drawing.Size(718, 32);
            this.UploadProgress.TabIndex = 42;
            this.UploadProgress.Visible = false;
            // 
            // uploadingFileLabel
            // 
            this.uploadingFileLabel.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uploadingFileLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.uploadingFileLabel.Location = new System.Drawing.Point(12, 121);
            this.uploadingFileLabel.Name = "uploadingFileLabel";
            this.uploadingFileLabel.Size = new System.Drawing.Size(556, 21);
            this.uploadingFileLabel.TabIndex = 49;
            this.uploadingFileLabel.Text = "Uploading ...";
            this.uploadingFileLabel.Visible = false;
            // 
            // HighlightsUploadVideoButton
            // 
            this.HighlightsUploadVideoButton.Location = new System.Drawing.Point(669, 74);
            this.HighlightsUploadVideoButton.Name = "HighlightsUploadVideoButton";
            this.HighlightsUploadVideoButton.Size = new System.Drawing.Size(64, 26);
            this.HighlightsUploadVideoButton.TabIndex = 41;
            this.HighlightsUploadVideoButton.Text = "browse";
            this.HighlightsUploadVideoButton.UseVisualStyleBackColor = true;
            this.HighlightsUploadVideoButton.Click += new System.EventHandler(this.HighlightsUploadVideoButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 18);
            this.label7.TabIndex = 40;
            this.label7.Text = "Highlights";
            // 
            // HighlightsUploadVideoFile
            // 
            this.HighlightsUploadVideoFile.Location = new System.Drawing.Point(131, 75);
            this.HighlightsUploadVideoFile.Name = "HighlightsUploadVideoFile";
            this.HighlightsUploadVideoFile.Size = new System.Drawing.Size(532, 26);
            this.HighlightsUploadVideoFile.TabIndex = 39;
            this.HighlightsUploadVideoFile.TextChanged += new System.EventHandler(this.HighlightsUploadVideoFile_TextChanged);
            // 
            // UploadToYouTubeButton
            // 
            this.UploadToYouTubeButton.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UploadToYouTubeButton.Location = new System.Drawing.Point(570, 189);
            this.UploadToYouTubeButton.Margin = new System.Windows.Forms.Padding(4);
            this.UploadToYouTubeButton.Name = "UploadToYouTubeButton";
            this.UploadToYouTubeButton.Size = new System.Drawing.Size(163, 64);
            this.UploadToYouTubeButton.TabIndex = 38;
            this.UploadToYouTubeButton.Text = "Upload to YouTube";
            this.UploadToYouTubeButton.UseVisualStyleBackColor = true;
            this.UploadToYouTubeButton.Click += new System.EventHandler(this.UploadToYouTubeButton_Click);
            // 
            // MainUploadVideoFileButton
            // 
            this.MainUploadVideoFileButton.Location = new System.Drawing.Point(669, 30);
            this.MainUploadVideoFileButton.Name = "MainUploadVideoFileButton";
            this.MainUploadVideoFileButton.Size = new System.Drawing.Size(64, 26);
            this.MainUploadVideoFileButton.TabIndex = 37;
            this.MainUploadVideoFileButton.Text = "browse";
            this.MainUploadVideoFileButton.UseVisualStyleBackColor = true;
            this.MainUploadVideoFileButton.Click += new System.EventHandler(this.MainUploadVideoFileButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 18);
            this.label6.TabIndex = 36;
            this.label6.Text = "Main Video";
            // 
            // MainUploadVideoFile
            // 
            this.MainUploadVideoFile.Location = new System.Drawing.Point(131, 30);
            this.MainUploadVideoFile.Name = "MainUploadVideoFile";
            this.MainUploadVideoFile.Size = new System.Drawing.Size(532, 26);
            this.MainUploadVideoFile.TabIndex = 35;
            this.MainUploadVideoFile.TextChanged += new System.EventHandler(this.MainUploadVideoFile_TextChanged);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox3.Location = new System.Drawing.Point(17, 104);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(745, 5);
            this.pictureBox3.TabIndex = 36;
            this.pictureBox3.TabStop = false;
            // 
            // UploadVideoToYouTube
            // 
            this.UploadVideoToYouTube.AutoSize = true;
            this.UploadVideoToYouTube.Location = new System.Drawing.Point(258, 411);
            this.UploadVideoToYouTube.Name = "UploadVideoToYouTube";
            this.UploadVideoToYouTube.Size = new System.Drawing.Size(182, 22);
            this.UploadVideoToYouTube.TabIndex = 47;
            this.UploadVideoToYouTube.Text = "Upload Video to YouTube";
            this.UploadVideoToYouTube.UseVisualStyleBackColor = true;
            this.UploadVideoToYouTube.Visible = false;
            // 
            // EncodeVideoAfterCapture
            // 
            this.EncodeVideoAfterCapture.AutoSize = true;
            this.EncodeVideoAfterCapture.Location = new System.Drawing.Point(17, 411);
            this.EncodeVideoAfterCapture.Name = "EncodeVideoAfterCapture";
            this.EncodeVideoAfterCapture.Size = new System.Drawing.Size(198, 22);
            this.EncodeVideoAfterCapture.TabIndex = 46;
            this.EncodeVideoAfterCapture.Text = "Encode Video After Capture";
            this.EncodeVideoAfterCapture.UseVisualStyleBackColor = true;
            // 
            // youTubeCredentialsRequired
            // 
            this.youTubeCredentialsRequired.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.youTubeCredentialsRequired.ForeColor = System.Drawing.Color.DarkRed;
            this.youTubeCredentialsRequired.Location = new System.Drawing.Point(484, 411);
            this.youTubeCredentialsRequired.Name = "youTubeCredentialsRequired";
            this.youTubeCredentialsRequired.Size = new System.Drawing.Size(279, 28);
            this.youTubeCredentialsRequired.TabIndex = 48;
            this.youTubeCredentialsRequired.Text = "*Must enter your YouTube username and password before uploading";
            this.youTubeCredentialsRequired.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(681, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 26);
            this.button1.TabIndex = 49;
            this.button1.Text = "About ...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 463);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.youTubeCredentialsRequired);
            this.Controls.Add(this.UploadVideoToYouTube);
            this.Controls.Add(this.EncodeVideoAfterCapture);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.logMessagesButton);
            this.Controls.Add(this.configureGeneralSettingsButton);
            this.Controls.Add(this.SettingsButton);
            this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabCapture.ResumeLayout(false);
            this.tabCapture.PerformLayout();
            this.tabTranscoding.ResumeLayout(false);
            this.tabTranscoding.PerformLayout();
            this.tabUploading.ResumeLayout(false);
            this.tabUploading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SettingsButton;
        private System.Windows.Forms.Button configureGeneralSettingsButton;
        private System.Windows.Forms.Button logMessagesButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabCapture;
        private System.Windows.Forms.CheckBox TestOnlyCheckBox;
        private System.Windows.Forms.Label WaitingForIRacingLabel;
        private System.Windows.Forms.Label ProcessErrorMessageLabel;
        private System.Windows.Forms.Label CapturingRaceLabel;
        private System.Windows.Forms.Label AnalysingRaceLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BeginProcessButton;
        private System.Windows.Forms.Button workingFolderButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox workingFolderTextBox;
        private System.Windows.Forms.TabPage tabTranscoding;
        private System.Windows.Forms.Label VideoDetailLabel;
        private System.Windows.Forms.ComboBox audioBitRate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox videoBitRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label errorSourceVideoLabel;
        private System.Windows.Forms.Button sourceVideoButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sourceVideoTextBox;
        private System.Windows.Forms.Button transcodeCancelButton;
        private System.Windows.Forms.ProgressBar transcodeProgressBar;
        private System.Windows.Forms.Button transcodeVideoButton;
        private System.Windows.Forms.TabPage tabUploading;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.CheckBox UploadVideoToYouTube;
        private System.Windows.Forms.CheckBox EncodeVideoAfterCapture;
        private System.Windows.Forms.Button HighlightsUploadVideoButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox HighlightsUploadVideoFile;
        private System.Windows.Forms.Button UploadToYouTubeButton;
        private System.Windows.Forms.Button MainUploadVideoFileButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox MainUploadVideoFile;
        private System.Windows.Forms.ProgressBar UploadProgress;
        private System.Windows.Forms.Label youTubeCredentialsRequired;
        private System.Windows.Forms.Label uploadingFileLabel;
        private System.Windows.Forms.CheckBox highlightVideoOnly;
        private System.Windows.Forms.Label configureTrackCamerasLabel;
        private System.Windows.Forms.Button verifyVideoCaptureButton;
        private System.Windows.Forms.Button button1;
    }
}