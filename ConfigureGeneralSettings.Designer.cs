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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureGeneralSettings));
            this.okButton = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.helpText = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.battleFactor = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.battleStickyPeriod = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.youTubePassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.youTubeUserName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.battleGap = new System.Windows.Forms.TextBox();
            this.preferredDriverNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cameraStickyPeriod = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.followLeaderAtRaceStartPeriodTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(637, 266);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(109, 37);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            this.okButton.Enter += new System.EventHandler(this.OnFocus);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox3.Location = new System.Drawing.Point(558, 6);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(11, 297);
            this.pictureBox3.TabIndex = 37;
            this.pictureBox3.TabStop = false;
            // 
            // helpText
            // 
            this.helpText.Location = new System.Drawing.Point(576, 37);
            this.helpText.Name = "helpText";
            this.helpText.Size = new System.Drawing.Size(170, 226);
            this.helpText.TabIndex = 38;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(576, 6);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(135, 16);
            this.label11.TabIndex = 39;
            this.label11.Text = "Description of setting:";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.followLeaderAtRaceStartPeriodTextBox);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.battleFactor);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.battleStickyPeriod);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.youTubePassword);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.youTubeUserName);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.battleGap);
            this.panel1.Controls.Add(this.preferredDriverNameTextBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cameraStickyPeriod);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(539, 291);
            this.panel1.TabIndex = 40;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 183);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(162, 16);
            this.label12.TabIndex = 12;
            this.label12.Text = "Hot Key for Video Capture";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(301, 180);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(134, 22);
            this.textBox1.TabIndex = 13;
            this.textBox1.Tag = "The hotkey used by your video capture program.  At this time, this can not be cha" +
    "nged in iRacing Replay Director.  Ensure your video capture software uses ALT+F9" +
    " or F9";
            this.textBox1.Text = "ALT + F9";
            this.textBox1.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 126);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(160, 16);
            this.label10.TabIndex = 10;
            this.label10.Text = "Factor for battle selection.";
            // 
            // battleFactor
            // 
            this.battleFactor.Location = new System.Drawing.Point(301, 123);
            this.battleFactor.Name = "battleFactor";
            this.battleFactor.Size = new System.Drawing.Size(134, 22);
            this.battleFactor.TabIndex = 11;
            this.battleFactor.Tag = resources.GetString("battleFactor.Tag");
            this.battleFactor.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(441, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 16);
            this.label9.TabIndex = 6;
            this.label9.Text = "seconds";
            // 
            // battleStickyPeriod
            // 
            this.battleStickyPeriod.Location = new System.Drawing.Point(301, 39);
            this.battleStickyPeriod.Name = "battleStickyPeriod";
            this.battleStickyPeriod.Size = new System.Drawing.Size(134, 22);
            this.battleStickyPeriod.TabIndex = 5;
            this.battleStickyPeriod.Tag = "The time period that must elapsed before a new battle is randomly selected.";
            this.battleStickyPeriod.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(186, 16);
            this.label8.TabIndex = 4;
            this.label8.Text = "Time between battle switches:";
            // 
            // youTubePassword
            // 
            this.youTubePassword.Location = new System.Drawing.Point(300, 315);
            this.youTubePassword.Name = "youTubePassword";
            this.youTubePassword.PasswordChar = '*';
            this.youTubePassword.Size = new System.Drawing.Size(134, 22);
            this.youTubePassword.TabIndex = 19;
            this.youTubePassword.Tag = "Your youtube password.";
            this.youTubePassword.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 318);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(130, 16);
            this.label7.TabIndex = 18;
            this.label7.Text = "YouTube Password:";
            // 
            // youTubeUserName
            // 
            this.youTubeUserName.Location = new System.Drawing.Point(301, 287);
            this.youTubeUserName.Name = "youTubeUserName";
            this.youTubeUserName.Size = new System.Drawing.Size(134, 22);
            this.youTubeUserName.TabIndex = 17;
            this.youTubeUserName.Tag = "Your YouTube username to allow publishing to your youtube account";
            this.youTubeUserName.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 290);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 16);
            this.label6.TabIndex = 16;
            this.label6.Text = "YouTube Username:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(206, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Time gap between cars for battle:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(441, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "seconds";
            // 
            // battleGap
            // 
            this.battleGap.Location = new System.Drawing.Point(301, 65);
            this.battleGap.Name = "battleGap";
            this.battleGap.Size = new System.Drawing.Size(134, 22);
            this.battleGap.TabIndex = 8;
            this.battleGap.Tag = "The approximate amount of time between cars to determine if they are battling.  D" +
    "efault 1 second";
            this.battleGap.Enter += new System.EventHandler(this.OnFocus);
            // 
            // preferredDriverNameTextBox
            // 
            this.preferredDriverNameTextBox.Location = new System.Drawing.Point(301, 240);
            this.preferredDriverNameTextBox.Name = "preferredDriverNameTextBox";
            this.preferredDriverNameTextBox.Size = new System.Drawing.Size(134, 22);
            this.preferredDriverNameTextBox.TabIndex = 15;
            this.preferredDriverNameTextBox.Tag = "A comma seperated list of driver names, to preference in camera selection.";
            this.preferredDriverNameTextBox.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(269, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "Preferred driver names (comma separated):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(441, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "seconds";
            // 
            // cameraStickyPeriod
            // 
            this.cameraStickyPeriod.Location = new System.Drawing.Point(301, 11);
            this.cameraStickyPeriod.Name = "cameraStickyPeriod";
            this.cameraStickyPeriod.Size = new System.Drawing.Size(134, 22);
            this.cameraStickyPeriod.TabIndex = 2;
            this.cameraStickyPeriod.Tag = "The time period that must elapsed before a new random camera is selected.";
            this.cameraStickyPeriod.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time between camera switches:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 96);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(199, 16);
            this.label13.TabIndex = 20;
            this.label13.Text = "Time to track leader at race start";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(440, 96);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(60, 16);
            this.label14.TabIndex = 22;
            this.label14.Text = "seconds";
            // 
            // followLeaderAtRaceStartPeriodTextBox
            // 
            this.followLeaderAtRaceStartPeriodTextBox.Location = new System.Drawing.Point(300, 93);
            this.followLeaderAtRaceStartPeriodTextBox.Name = "followLeaderAtRaceStartPeriodTextBox";
            this.followLeaderAtRaceStartPeriodTextBox.Size = new System.Drawing.Size(134, 22);
            this.followLeaderAtRaceStartPeriodTextBox.TabIndex = 21;
            this.followLeaderAtRaceStartPeriodTextBox.Tag = "The amount of time, to stay focused on the leader, at race start.  After this per" +
    "iod, the normal camera and driver tracking rules will apply.";
            this.followLeaderAtRaceStartPeriodTextBox.Enter += new System.EventHandler(this.OnFocus);
            // 
            // ConfigureGeneralSettings
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(777, 325);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.helpText);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.okButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConfigureGeneralSettings";
            this.Text = "Configure Video Capture";
            this.Load += new System.EventHandler(this.ConfigureGeneralSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label helpText;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox battleFactor;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox battleStickyPeriod;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox youTubePassword;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox youTubeUserName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox battleGap;
        private System.Windows.Forms.TextBox preferredDriverNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox cameraStickyPeriod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox followLeaderAtRaceStartPeriodTextBox;

    }
}