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
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.helpText = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
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
            // panel
            // 
            this.panel.AutoScroll = true;
            this.panel.Location = new System.Drawing.Point(13, 13);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(538, 290);
            this.panel.TabIndex = 41;
            // 
            // ConfigureGeneralSettings
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(767, 324);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.helpText);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.okButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConfigureGeneralSettings";
            this.Text = "Configure Video Capture";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label helpText;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel;

    }
}