namespace iRacingReplayOverlay
{
    partial class ConfigureGeneralSettings
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
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.MaxTimeBetweenCameraChangesTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PreferredDriverNameTextBox = new System.Windows.Forms.TextBox();
            this.MaxTimeForInterestingEventTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.youTubeUserName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.youTubePassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(391, 182);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(109, 37);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 16);
            this.label1.TabIndex = 12;
            this.label1.Text = "Maximum time between camera switches";
            // 
            // MaxTimeBetweenCameraChangesTextBox
            // 
            this.MaxTimeBetweenCameraChangesTextBox.Location = new System.Drawing.Point(300, 12);
            this.MaxTimeBetweenCameraChangesTextBox.Name = "MaxTimeBetweenCameraChangesTextBox";
            this.MaxTimeBetweenCameraChangesTextBox.Size = new System.Drawing.Size(134, 22);
            this.MaxTimeBetweenCameraChangesTextBox.TabIndex = 13;
            this.MaxTimeBetweenCameraChangesTextBox.TextChanged += new System.EventHandler(this.MaxTimeBetweenCameraSwitchesTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(440, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 16);
            this.label2.TabIndex = 14;
            this.label2.Text = "seconds";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(266, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "Preferred driver names (comma separated)";
            // 
            // PreferredDriverNameTextBox
            // 
            this.PreferredDriverNameTextBox.Location = new System.Drawing.Point(300, 68);
            this.PreferredDriverNameTextBox.Name = "PreferredDriverNameTextBox";
            this.PreferredDriverNameTextBox.Size = new System.Drawing.Size(134, 22);
            this.PreferredDriverNameTextBox.TabIndex = 16;
            this.PreferredDriverNameTextBox.TextChanged += new System.EventHandler(this.PreferredDriverNameTextBox_TextChanged);
            // 
            // MaxTimeForInterestingEventTextBox
            // 
            this.MaxTimeForInterestingEventTextBox.Location = new System.Drawing.Point(300, 40);
            this.MaxTimeForInterestingEventTextBox.Name = "MaxTimeForInterestingEventTextBox";
            this.MaxTimeForInterestingEventTextBox.Size = new System.Drawing.Size(134, 22);
            this.MaxTimeForInterestingEventTextBox.TabIndex = 17;
            this.MaxTimeForInterestingEventTextBox.TextChanged += new System.EventHandler(this.MaxTimeForInterestingEventTextBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(440, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 16);
            this.label4.TabIndex = 18;
            this.label4.Text = "seconds";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(240, 16);
            this.label5.TabIndex = 19;
            this.label5.Text = "Time between cars for interesting event";
            // 
            // youTubeUserName
            // 
            this.youTubeUserName.Location = new System.Drawing.Point(300, 115);
            this.youTubeUserName.Name = "youTubeUserName";
            this.youTubeUserName.Size = new System.Drawing.Size(134, 22);
            this.youTubeUserName.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 16);
            this.label6.TabIndex = 20;
            this.label6.Text = "YouTube Username:";
            // 
            // youTubePassword
            // 
            this.youTubePassword.Location = new System.Drawing.Point(300, 143);
            this.youTubePassword.Name = "youTubePassword";
            this.youTubePassword.PasswordChar = '*';
            this.youTubePassword.Size = new System.Drawing.Size(134, 22);
            this.youTubePassword.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 146);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(130, 16);
            this.label7.TabIndex = 22;
            this.label7.Text = "YouTube Password:";
            // 
            // ConfigureGeneralSettings
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(512, 231);
            this.Controls.Add(this.youTubePassword);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.youTubeUserName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.MaxTimeForInterestingEventTextBox);
            this.Controls.Add(this.PreferredDriverNameTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MaxTimeBetweenCameraChangesTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConfigureGeneralSettings";
            this.Text = "Configure Video Capture";
            this.Load += new System.EventHandler(this.ConfigureGeneralSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MaxTimeBetweenCameraChangesTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PreferredDriverNameTextBox;
        private System.Windows.Forms.TextBox MaxTimeForInterestingEventTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox youTubeUserName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox youTubePassword;
        private System.Windows.Forms.Label label7;

    }
}