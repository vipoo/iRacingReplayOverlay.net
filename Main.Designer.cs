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
            this.transcodeVideo = new System.Windows.Forms.Button();
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
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 318);
            this.Controls.Add(this.transcodeVideo);
            this.Name = "Main";
            this.Text = "Main";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button transcodeVideo;
    }
}