// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net

namespace iRacingReplayOverlay
{
    partial class UsageDataRequest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsageDataRequest));
            this.label1 = new System.Windows.Forms.Label();
            this.sendUsageData = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.trackingId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(394, 183);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // sendUsageData
            // 
            this.sendUsageData.AutoSize = true;
            this.sendUsageData.Location = new System.Drawing.Point(12, 315);
            this.sendUsageData.Name = "sendUsageData";
            this.sendUsageData.Size = new System.Drawing.Size(228, 17);
            this.sendUsageData.TabIndex = 1;
            this.sendUsageData.Text = "Check here to allow sending of usage data";
            this.sendUsageData.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(334, 315);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // trackingId
            // 
            this.trackingId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.trackingId.Location = new System.Drawing.Point(16, 255);
            this.trackingId.MaxLength = 50;
            this.trackingId.Name = "trackingId";
            this.trackingId.ReadOnly = true;
            this.trackingId.Size = new System.Drawing.Size(391, 13);
            this.trackingId.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(223, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tracking ID: (will be logged against your data)";
            // 
            // UsageDataRequest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 350);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackingId);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.sendUsageData);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UsageDataRequest";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Usage Data";
            this.Load += new System.EventHandler(this.UsageDataRequest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox sendUsageData;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox trackingId;
        private System.Windows.Forms.Label label2;
    }
}
