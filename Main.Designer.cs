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

namespace iRacingReplayOverlay.net
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.transcodeVideoButton = new System.Windows.Forms.Button();
            this.captureLight = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.transcodeProgressBar = new System.Windows.Forms.ProgressBar();
            this.transcodeCancelButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sourceVideoTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sourceVideoButton = new System.Windows.Forms.Button();
            this.sourceGameDataButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.sourceGameDataTextBox = new System.Windows.Forms.TextBox();
            this.workingFolderButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.workingFolderTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.captureLight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // transcodeVideoButton
            // 
            this.transcodeVideoButton.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transcodeVideoButton.Location = new System.Drawing.Point(28, 231);
            this.transcodeVideoButton.Margin = new System.Windows.Forms.Padding(4);
            this.transcodeVideoButton.Name = "transcodeVideoButton";
            this.transcodeVideoButton.Size = new System.Drawing.Size(163, 64);
            this.transcodeVideoButton.TabIndex = 0;
            this.transcodeVideoButton.Text = "Transcode Video";
            this.transcodeVideoButton.UseVisualStyleBackColor = true;
            this.transcodeVideoButton.Click += new System.EventHandler(this.TranscodeVideo_Click);
            // 
            // captureLight
            // 
            this.captureLight.Image = ((System.Drawing.Image)(resources.GetObject("captureLight.Image")));
            this.captureLight.Location = new System.Drawing.Point(362, 105);
            this.captureLight.Margin = new System.Windows.Forms.Padding(4);
            this.captureLight.Name = "captureLight";
            this.captureLight.Size = new System.Drawing.Size(56, 37);
            this.captureLight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.captureLight.TabIndex = 1;
            this.captureLight.TabStop = false;
            this.captureLight.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(437, 105);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Capturing Game Data";
            // 
            // transcodeProgressBar
            // 
            this.transcodeProgressBar.Location = new System.Drawing.Point(45, 413);
            this.transcodeProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this.transcodeProgressBar.Maximum = 10000;
            this.transcodeProgressBar.Name = "transcodeProgressBar";
            this.transcodeProgressBar.Size = new System.Drawing.Size(611, 32);
            this.transcodeProgressBar.TabIndex = 3;
            // 
            // transcodeCancelButton
            // 
            this.transcodeCancelButton.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transcodeCancelButton.Location = new System.Drawing.Point(199, 231);
            this.transcodeCancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.transcodeCancelButton.Name = "transcodeCancelButton";
            this.transcodeCancelButton.Size = new System.Drawing.Size(163, 64);
            this.transcodeCancelButton.TabIndex = 4;
            this.transcodeCancelButton.Text = "Cancel Transcoding";
            this.transcodeCancelButton.UseVisualStyleBackColor = true;
            this.transcodeCancelButton.Visible = false;
            this.transcodeCancelButton.Click += new System.EventHandler(this.transcodeCancel_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Location = new System.Drawing.Point(13, 202);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(680, 4);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 101);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(184, 80);
            this.label2.TabIndex = 6;
            this.label2.Text = "Press ALT+F9 to begin capturing game data";
            // 
            // sourceVideoTextBox
            // 
            this.sourceVideoTextBox.Location = new System.Drawing.Point(158, 320);
            this.sourceVideoTextBox.Name = "sourceVideoTextBox";
            this.sourceVideoTextBox.Size = new System.Drawing.Size(464, 26);
            this.sourceVideoTextBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 320);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 18);
            this.label3.TabIndex = 8;
            this.label3.Text = "Source Video:";
            // 
            // sourceVideoButton
            // 
            this.sourceVideoButton.Location = new System.Drawing.Point(628, 320);
            this.sourceVideoButton.Name = "sourceVideoButton";
            this.sourceVideoButton.Size = new System.Drawing.Size(64, 26);
            this.sourceVideoButton.TabIndex = 9;
            this.sourceVideoButton.Text = "browse";
            this.sourceVideoButton.UseVisualStyleBackColor = true;
            this.sourceVideoButton.Click += new System.EventHandler(this.sourceVideoButton_Click);
            // 
            // sourceGameDataButton
            // 
            this.sourceGameDataButton.Location = new System.Drawing.Point(628, 352);
            this.sourceGameDataButton.Name = "sourceGameDataButton";
            this.sourceGameDataButton.Size = new System.Drawing.Size(64, 26);
            this.sourceGameDataButton.TabIndex = 12;
            this.sourceGameDataButton.Text = "browse";
            this.sourceGameDataButton.UseVisualStyleBackColor = true;
            this.sourceGameDataButton.Click += new System.EventHandler(this.sourceGameDataButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 352);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 18);
            this.label4.TabIndex = 11;
            this.label4.Text = "Source Game Data:";
            // 
            // sourceGameDataTextBox
            // 
            this.sourceGameDataTextBox.Location = new System.Drawing.Point(158, 352);
            this.sourceGameDataTextBox.Name = "sourceGameDataTextBox";
            this.sourceGameDataTextBox.Size = new System.Drawing.Size(464, 26);
            this.sourceGameDataTextBox.TabIndex = 10;
            // 
            // workingFolderButton
            // 
            this.workingFolderButton.Location = new System.Drawing.Point(628, 23);
            this.workingFolderButton.Name = "workingFolderButton";
            this.workingFolderButton.Size = new System.Drawing.Size(64, 26);
            this.workingFolderButton.TabIndex = 15;
            this.workingFolderButton.Text = "browse";
            this.workingFolderButton.UseVisualStyleBackColor = true;
            this.workingFolderButton.Click += new System.EventHandler(this.workingFolderButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 18);
            this.label5.TabIndex = 14;
            this.label5.Text = "Worker Folder:";
            // 
            // workingFolderTextBox
            // 
            this.workingFolderTextBox.Location = new System.Drawing.Point(134, 23);
            this.workingFolderTextBox.Name = "workingFolderTextBox";
            this.workingFolderTextBox.Size = new System.Drawing.Size(488, 26);
            this.workingFolderTextBox.TabIndex = 13;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 488);
            this.Controls.Add(this.workingFolderButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.workingFolderTextBox);
            this.Controls.Add(this.sourceGameDataButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sourceGameDataTextBox);
            this.Controls.Add(this.sourceVideoButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sourceVideoTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.transcodeCancelButton);
            this.Controls.Add(this.transcodeProgressBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.captureLight);
            this.Controls.Add(this.transcodeVideoButton);
            this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.captureLight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button transcodeVideoButton;
        private System.Windows.Forms.PictureBox captureLight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar transcodeProgressBar;
        private System.Windows.Forms.Button transcodeCancelButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sourceVideoTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button sourceVideoButton;
        private System.Windows.Forms.Button sourceGameDataButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox sourceGameDataTextBox;
        private System.Windows.Forms.Button workingFolderButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox workingFolderTextBox;
    }
}