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
            this.transcodeVideo = new System.Windows.Forms.Button();
            this.captureLight = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.captureLight)).BeginInit();
            this.SuspendLayout();
            // 
            // transcodeVideo
            // 
            this.transcodeVideo.Location = new System.Drawing.Point(41, 37);
            this.transcodeVideo.Name = "transcodeVideo";
            this.transcodeVideo.Size = new System.Drawing.Size(122, 46);
            this.transcodeVideo.TabIndex = 0;
            this.transcodeVideo.Text = "Transcode Video";
            this.transcodeVideo.UseVisualStyleBackColor = true;
            this.transcodeVideo.Click += new System.EventHandler(this.TranscodeVideo_Click);
            // 
            // captureLight
            // 
            this.captureLight.Image = ((System.Drawing.Image)(resources.GetObject("captureLight.Image")));
            this.captureLight.Location = new System.Drawing.Point(664, 37);
            this.captureLight.Name = "captureLight";
            this.captureLight.Size = new System.Drawing.Size(75, 46);
            this.captureLight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.captureLight.TabIndex = 1;
            this.captureLight.TabStop = false;
            this.captureLight.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(546, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Capturing Game Data:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 318);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.captureLight);
            this.Controls.Add(this.transcodeVideo);
            this.Name = "Main";
            this.Text = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.captureLight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button transcodeVideo;
        private System.Windows.Forms.PictureBox captureLight;
        private System.Windows.Forms.Label label1;
    }
}