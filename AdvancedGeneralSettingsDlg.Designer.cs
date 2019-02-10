namespace iRacingReplayOverlay
{
    partial class AdvanceGeneralSettingsDlg
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
            this.incidents_settings_tab = new System.Windows.Forms.TabControl();
            this.tabPageTiming = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.durationHighlight = new System.Windows.Forms.MaskedTextBox();
            this.timeToFocusLeaderRaceFinish = new System.Windows.Forms.MaskedTextBox();
            this.timeToFocusLeaderRaceStart = new System.Windows.Forms.MaskedTextBox();
            this.battleSwitchTimeInput = new System.Windows.Forms.MaskedTextBox();
            this.cameraSwitchTimeInput = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxSettingDescription = new System.Windows.Forms.TextBox();
            this.tabPageIncidents = new System.Windows.Forms.TabPage();
            this.tabPageDrivers = new System.Windows.Forms.TabPage();
            this.ok_button = new System.Windows.Forms.Button();
            this.cancel_button = new System.Windows.Forms.Button();
            this.incidents_settings_tab.SuspendLayout();
            this.tabPageTiming.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // incidents_settings_tab
            // 
            this.incidents_settings_tab.Controls.Add(this.tabPageTiming);
            this.incidents_settings_tab.Controls.Add(this.tabPageIncidents);
            this.incidents_settings_tab.Controls.Add(this.tabPageDrivers);
            this.incidents_settings_tab.Location = new System.Drawing.Point(16, 18);
            this.incidents_settings_tab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.incidents_settings_tab.Name = "incidents_settings_tab";
            this.incidents_settings_tab.SelectedIndex = 0;
            this.incidents_settings_tab.Size = new System.Drawing.Size(520, 564);
            this.incidents_settings_tab.TabIndex = 0;
            // 
            // tabPageTiming
            // 
            this.tabPageTiming.Controls.Add(this.groupBox1);
            this.tabPageTiming.Controls.Add(this.groupBox2);
            this.tabPageTiming.Location = new System.Drawing.Point(4, 28);
            this.tabPageTiming.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageTiming.Name = "tabPageTiming";
            this.tabPageTiming.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageTiming.Size = new System.Drawing.Size(512, 532);
            this.tabPageTiming.TabIndex = 0;
            this.tabPageTiming.Text = "Timing";
            this.tabPageTiming.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.durationHighlight);
            this.groupBox1.Controls.Add(this.timeToFocusLeaderRaceFinish);
            this.groupBox1.Controls.Add(this.timeToFocusLeaderRaceStart);
            this.groupBox1.Controls.Add(this.battleSwitchTimeInput);
            this.groupBox1.Controls.Add(this.cameraSwitchTimeInput);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(21, 19);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(480, 246);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // durationHighlight
            // 
            this.durationHighlight.Location = new System.Drawing.Point(309, 175);
            this.durationHighlight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.durationHighlight.Mask = "000.0 sec";
            this.durationHighlight.Name = "durationHighlight";
            this.durationHighlight.Size = new System.Drawing.Size(132, 27);
            this.durationHighlight.TabIndex = 3;
            this.durationHighlight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // timeToFocusLeaderRaceFinish
            // 
            this.timeToFocusLeaderRaceFinish.Location = new System.Drawing.Point(309, 137);
            this.timeToFocusLeaderRaceFinish.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.timeToFocusLeaderRaceFinish.Mask = "000.0 sec";
            this.timeToFocusLeaderRaceFinish.Name = "timeToFocusLeaderRaceFinish";
            this.timeToFocusLeaderRaceFinish.Size = new System.Drawing.Size(132, 27);
            this.timeToFocusLeaderRaceFinish.TabIndex = 3;
            this.timeToFocusLeaderRaceFinish.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // timeToFocusLeaderRaceStart
            // 
            this.timeToFocusLeaderRaceStart.Location = new System.Drawing.Point(309, 99);
            this.timeToFocusLeaderRaceStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.timeToFocusLeaderRaceStart.Mask = "000.0 sec";
            this.timeToFocusLeaderRaceStart.Name = "timeToFocusLeaderRaceStart";
            this.timeToFocusLeaderRaceStart.Size = new System.Drawing.Size(132, 27);
            this.timeToFocusLeaderRaceStart.TabIndex = 3;
            this.timeToFocusLeaderRaceStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // battleSwitchTimeInput
            // 
            this.battleSwitchTimeInput.Location = new System.Drawing.Point(309, 61);
            this.battleSwitchTimeInput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.battleSwitchTimeInput.Mask = "000.0 sec";
            this.battleSwitchTimeInput.Name = "battleSwitchTimeInput";
            this.battleSwitchTimeInput.Size = new System.Drawing.Size(132, 27);
            this.battleSwitchTimeInput.TabIndex = 3;
            this.battleSwitchTimeInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cameraSwitchTimeInput
            // 
            this.cameraSwitchTimeInput.Location = new System.Drawing.Point(309, 23);
            this.cameraSwitchTimeInput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cameraSwitchTimeInput.Mask = "000.0 sec";
            this.cameraSwitchTimeInput.Name = "cameraSwitchTimeInput";
            this.cameraSwitchTimeInput.Size = new System.Drawing.Size(132, 27);
            this.cameraSwitchTimeInput.TabIndex = 3;
            this.cameraSwitchTimeInput.Tag = "45.0";
            this.cameraSwitchTimeInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.cameraSwitchTimeInput.Enter += new System.EventHandler(this.cameraSwitchTimeInput_Enter);
            this.cameraSwitchTimeInput.MouseHover += new System.EventHandler(this.cameraSwitchTimeInput_MouseHover);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 186);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(186, 19);
            this.label5.TabIndex = 1;
            this.label5.Text = "Duration of Highlight Video";
            this.label5.Click += new System.EventHandler(this.label1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 146);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(211, 19);
            this.label4.TabIndex = 1;
            this.label4.Text = "Time to track leader on last lap";
            this.label4.Click += new System.EventHandler(this.label1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 107);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(222, 19);
            this.label3.TabIndex = 1;
            this.label3.Text = "Time to track leader at race start";
            this.label3.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 67);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(204, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "Time between battle switches";
            this.label2.Click += new System.EventHandler(this.label1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time between camera switches";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxSettingDescription);
            this.groupBox2.Location = new System.Drawing.Point(21, 329);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(480, 164);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Description of Setting";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // textBoxSettingDescription
            // 
            this.textBoxSettingDescription.Location = new System.Drawing.Point(15, 29);
            this.textBoxSettingDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxSettingDescription.Multiline = true;
            this.textBoxSettingDescription.Name = "textBoxSettingDescription";
            this.textBoxSettingDescription.Size = new System.Drawing.Size(456, 124);
            this.textBoxSettingDescription.TabIndex = 0;
            this.textBoxSettingDescription.TextChanged += new System.EventHandler(this.textBoxSettingDescription_TextChanged);
            // 
            // tabPageIncidents
            // 
            this.tabPageIncidents.Location = new System.Drawing.Point(4, 22);
            this.tabPageIncidents.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageIncidents.Name = "tabPageIncidents";
            this.tabPageIncidents.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageIncidents.Size = new System.Drawing.Size(512, 538);
            this.tabPageIncidents.TabIndex = 1;
            this.tabPageIncidents.Text = "Incidents";
            this.tabPageIncidents.UseVisualStyleBackColor = true;
            // 
            // tabPageDrivers
            // 
            this.tabPageDrivers.Location = new System.Drawing.Point(4, 22);
            this.tabPageDrivers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageDrivers.Name = "tabPageDrivers";
            this.tabPageDrivers.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageDrivers.Size = new System.Drawing.Size(512, 538);
            this.tabPageDrivers.TabIndex = 2;
            this.tabPageDrivers.Text = "Drivers";
            this.tabPageDrivers.UseVisualStyleBackColor = true;
            // 
            // ok_button
            // 
            this.ok_button.Location = new System.Drawing.Point(573, 50);
            this.ok_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ok_button.Name = "ok_button";
            this.ok_button.Size = new System.Drawing.Size(137, 45);
            this.ok_button.TabIndex = 1;
            this.ok_button.Text = "OK";
            this.ok_button.UseVisualStyleBackColor = true;
            this.ok_button.Click += new System.EventHandler(this.button1_Click);
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(573, 114);
            this.cancel_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(137, 45);
            this.cancel_button.TabIndex = 1;
            this.cancel_button.Text = "Cancel";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.button1_Click);
            // 
            // AdvanceGeneralSettingsDlg
            // 
            this.AccessibleDescription = "General_Settings_Dialog";
            this.AccessibleName = "GeneralSettingsDlg";
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(755, 615);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.ok_button);
            this.Controls.Add(this.incidents_settings_tab);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "AdvanceGeneralSettingsDlg";
            this.Text = "Configure General Settings";
            this.Load += new System.EventHandler(this.AdvanceGeneralSettingsDlg_Load);
            this.incidents_settings_tab.ResumeLayout(false);
            this.tabPageTiming.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl incidents_settings_tab;
        private System.Windows.Forms.TabPage tabPageTiming;
        private System.Windows.Forms.TabPage tabPageIncidents;
        private System.Windows.Forms.Button ok_button;
        private System.Windows.Forms.Button cancel_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox durationHighlight;
        private System.Windows.Forms.MaskedTextBox timeToFocusLeaderRaceFinish;
        private System.Windows.Forms.MaskedTextBox timeToFocusLeaderRaceStart;
        private System.Windows.Forms.MaskedTextBox battleSwitchTimeInput;
        private System.Windows.Forms.MaskedTextBox cameraSwitchTimeInput;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage tabPageDrivers;
        private System.Windows.Forms.TextBox textBoxSettingDescription;
    }
}