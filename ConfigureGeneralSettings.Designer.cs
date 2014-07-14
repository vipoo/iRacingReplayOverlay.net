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
            this.label1 = new System.Windows.Forms.Label();
            this.cameraStickyPeriod = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.preferredDriverNameTextBox = new System.Windows.Forms.TextBox();
            this.battleGap = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.youTubeUserName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.youTubePassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.battleStickyPeriod = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.battleFactor = new System.Windows.Forms.TextBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.helpText = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(601, 229);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(109, 37);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            this.okButton.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 16);
            this.label1.TabIndex = 12;
            this.label1.Text = "Time between camera switches:";
            // 
            // cameraStickyPeriod
            // 
            this.cameraStickyPeriod.Location = new System.Drawing.Point(300, 12);
            this.cameraStickyPeriod.Name = "cameraStickyPeriod";
            this.cameraStickyPeriod.Size = new System.Drawing.Size(134, 22);
            this.cameraStickyPeriod.TabIndex = 1;
            this.cameraStickyPeriod.Tag = "The time period that must elapsed before a new random camera is selected.";
            this.cameraStickyPeriod.TextChanged += new System.EventHandler(this.MaxTimeBetweenCameraSwitchesTextBox_TextChanged);
            this.cameraStickyPeriod.Enter += new System.EventHandler(this.OnFocus);
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
            this.label3.Location = new System.Drawing.Point(13, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(269, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "Preferred driver names (comma separated):";
            // 
            // preferredDriverNameTextBox
            // 
            this.preferredDriverNameTextBox.Location = new System.Drawing.Point(300, 154);
            this.preferredDriverNameTextBox.Name = "preferredDriverNameTextBox";
            this.preferredDriverNameTextBox.Size = new System.Drawing.Size(134, 22);
            this.preferredDriverNameTextBox.TabIndex = 5;
            this.preferredDriverNameTextBox.Tag = "A comma seperated list of names, to preference the random selection of drivers.  " +
    "Only applicable when no battles are happening.";
            this.preferredDriverNameTextBox.TextChanged += new System.EventHandler(this.PreferredDriverNameTextBox_TextChanged);
            this.preferredDriverNameTextBox.Enter += new System.EventHandler(this.OnFocus);
            // 
            // battleGap
            // 
            this.battleGap.Location = new System.Drawing.Point(300, 66);
            this.battleGap.Name = "battleGap";
            this.battleGap.Size = new System.Drawing.Size(134, 22);
            this.battleGap.TabIndex = 3;
            this.battleGap.Tag = "The approximate amount of time between cars to determine if they are battling.  D" +
    "efault 1 second";
            this.battleGap.TextChanged += new System.EventHandler(this.MaxTimeForInterestingEventTextBox_TextChanged);
            this.battleGap.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(440, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 16);
            this.label4.TabIndex = 18;
            this.label4.Text = "seconds";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(206, 16);
            this.label5.TabIndex = 19;
            this.label5.Text = "Time gap between cars for battle:";
            // 
            // youTubeUserName
            // 
            this.youTubeUserName.Location = new System.Drawing.Point(300, 201);
            this.youTubeUserName.Name = "youTubeUserName";
            this.youTubeUserName.Size = new System.Drawing.Size(134, 22);
            this.youTubeUserName.TabIndex = 6;
            this.youTubeUserName.Tag = "Your YouTube username to allow publishing to your youtube account";
            this.youTubeUserName.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 204);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 16);
            this.label6.TabIndex = 20;
            this.label6.Text = "YouTube Username:";
            // 
            // youTubePassword
            // 
            this.youTubePassword.Location = new System.Drawing.Point(300, 229);
            this.youTubePassword.Name = "youTubePassword";
            this.youTubePassword.PasswordChar = '*';
            this.youTubePassword.Size = new System.Drawing.Size(134, 22);
            this.youTubePassword.TabIndex = 7;
            this.youTubePassword.Tag = "Your youtube password.";
            this.youTubePassword.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 232);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(130, 16);
            this.label7.TabIndex = 22;
            this.label7.Text = "YouTube Password:";
            // 
            // battleStickyPeriod
            // 
            this.battleStickyPeriod.Location = new System.Drawing.Point(300, 40);
            this.battleStickyPeriod.Name = "battleStickyPeriod";
            this.battleStickyPeriod.Size = new System.Drawing.Size(134, 22);
            this.battleStickyPeriod.TabIndex = 2;
            this.battleStickyPeriod.Tag = "The time period that must elapsed before a new battle is randomly selected.";
            this.battleStickyPeriod.TextChanged += new System.EventHandler(this.battleStickyPeriod_TextChanged);
            this.battleStickyPeriod.Enter += new System.EventHandler(this.OnFocus);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 43);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(186, 16);
            this.label8.TabIndex = 24;
            this.label8.Text = "Time between battle switches:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(440, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 16);
            this.label9.TabIndex = 25;
            this.label9.Text = "seconds";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 101);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(160, 16);
            this.label10.TabIndex = 28;
            this.label10.Text = "Factor for battle selection.";
            // 
            // battleFactor
            // 
            this.battleFactor.Location = new System.Drawing.Point(300, 98);
            this.battleFactor.Name = "battleFactor";
            this.battleFactor.Size = new System.Drawing.Size(134, 22);
            this.battleFactor.TabIndex = 4;
            this.battleFactor.Tag = resources.GetString("battleFactor.Tag");
            this.battleFactor.TextChanged += new System.EventHandler(this.battleFactor_TextChanged);
            this.battleFactor.Enter += new System.EventHandler(this.OnFocus);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox3.Location = new System.Drawing.Point(522, 12);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(10, 204);
            this.pictureBox3.TabIndex = 37;
            this.pictureBox3.TabStop = false;
            // 
            // helpText
            // 
            this.helpText.Location = new System.Drawing.Point(540, 43);
            this.helpText.Name = "helpText";
            this.helpText.Size = new System.Drawing.Size(170, 173);
            this.helpText.TabIndex = 38;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(540, 12);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(135, 16);
            this.label11.TabIndex = 39;
            this.label11.Text = "Description of setting:";
            // 
            // ConfigureGeneralSettings
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(722, 286);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.helpText);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.battleFactor);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.battleStickyPeriod);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.youTubePassword);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.youTubeUserName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.battleGap);
            this.Controls.Add(this.preferredDriverNameTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cameraStickyPeriod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConfigureGeneralSettings";
            this.Text = "Configure Video Capture";
            this.Load += new System.EventHandler(this.ConfigureGeneralSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox cameraStickyPeriod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox preferredDriverNameTextBox;
        private System.Windows.Forms.TextBox battleGap;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox youTubeUserName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox youTubePassword;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox battleStickyPeriod;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox battleFactor;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label helpText;
        private System.Windows.Forms.Label label11;

    }
}