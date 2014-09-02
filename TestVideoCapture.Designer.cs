namespace iRacingReplayOverlay
{
    partial class TestVideoCapture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestVideoCapture));
            this.TraceMessageTextBox = new System.Windows.Forms.TextBox();
            this.testVideoCaptureButton = new System.Windows.Forms.Button();
            this.workingFolderButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.workingFolderTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // TraceMessageTextBox
            // 
            this.TraceMessageTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TraceMessageTextBox.BackColor = System.Drawing.Color.Black;
            this.TraceMessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TraceMessageTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TraceMessageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TraceMessageTextBox.ForeColor = System.Drawing.Color.Lime;
            this.TraceMessageTextBox.Location = new System.Drawing.Point(13, 90);
            this.TraceMessageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.TraceMessageTextBox.MaxLength = 1040000;
            this.TraceMessageTextBox.Multiline = true;
            this.TraceMessageTextBox.Name = "TraceMessageTextBox";
            this.TraceMessageTextBox.ReadOnly = true;
            this.TraceMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TraceMessageTextBox.Size = new System.Drawing.Size(744, 362);
            this.TraceMessageTextBox.TabIndex = 2;
            this.TraceMessageTextBox.TabStop = false;
            this.TraceMessageTextBox.WordWrap = false;
            // 
            // testVideoCaptureButton
            // 
            this.testVideoCaptureButton.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.testVideoCaptureButton.Location = new System.Drawing.Point(571, 13);
            this.testVideoCaptureButton.Margin = new System.Windows.Forms.Padding(4);
            this.testVideoCaptureButton.Name = "testVideoCaptureButton";
            this.testVideoCaptureButton.Size = new System.Drawing.Size(186, 47);
            this.testVideoCaptureButton.TabIndex = 54;
            this.testVideoCaptureButton.Text = "Verify Video Capture";
            this.testVideoCaptureButton.UseVisualStyleBackColor = true;
            this.testVideoCaptureButton.Click += new System.EventHandler(this.testVideoCaptureButton_Click);
            // 
            // workingFolderButton
            // 
            this.workingFolderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.workingFolderButton.Location = new System.Drawing.Point(500, 17);
            this.workingFolderButton.Name = "workingFolderButton";
            this.workingFolderButton.Size = new System.Drawing.Size(64, 26);
            this.workingFolderButton.TabIndex = 53;
            this.workingFolderButton.Text = "browse";
            this.workingFolderButton.UseVisualStyleBackColor = true;
            this.workingFolderButton.Click += new System.EventHandler(this.workingFolderButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(151, 18);
            this.label5.TabIndex = 52;
            this.label5.Text = "Video Capture Folder:";
            // 
            // workingFolderTextBox
            // 
            this.workingFolderTextBox.Location = new System.Drawing.Point(167, 18);
            this.workingFolderTextBox.Name = "workingFolderTextBox";
            this.workingFolderTextBox.Size = new System.Drawing.Size(327, 24);
            this.workingFolderTextBox.TabIndex = 51;
            this.workingFolderTextBox.TextChanged += new System.EventHandler(this.workingFolderTextBox_TextChanged);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox3.Location = new System.Drawing.Point(765, 13);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(10, 435);
            this.pictureBox3.TabIndex = 55;
            this.pictureBox3.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(782, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 435);
            this.label1.TabIndex = 56;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // TestVideoCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 465);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.testVideoCaptureButton);
            this.Controls.Add(this.workingFolderButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.workingFolderTextBox);
            this.Controls.Add(this.TraceMessageTextBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "TestVideoCapture";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Verify Video Capture";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TestVideoCapture_FormClosed);
            this.Load += new System.EventHandler(this.TestVideoCapture_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TraceMessageTextBox;
        private System.Windows.Forms.Button testVideoCaptureButton;
        private System.Windows.Forms.Button workingFolderButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox workingFolderTextBox;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label1;
    }
}