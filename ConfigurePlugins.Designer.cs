namespace iRacingReplayOverlay
{
    partial class ConfigurePlugins
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
            this.pluginNames = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pluginVersions = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.currentInstalled = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pluginNames
            // 
            this.pluginNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pluginNames.FormattingEnabled = true;
            this.pluginNames.Location = new System.Drawing.Point(13, 132);
            this.pluginNames.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pluginNames.Name = "pluginNames";
            this.pluginNames.Size = new System.Drawing.Size(319, 28);
            this.pluginNames.TabIndex = 0;
            this.pluginNames.SelectedIndexChanged += new System.EventHandler(this.pluginNames_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(336, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Version:";
            // 
            // pluginVersions
            // 
            this.pluginVersions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pluginVersions.FormattingEnabled = true;
            this.pluginVersions.Location = new System.Drawing.Point(340, 132);
            this.pluginVersions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pluginVersions.Name = "pluginVersions";
            this.pluginVersions.Size = new System.Drawing.Size(319, 28);
            this.pluginVersions.TabIndex = 3;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(681, 268);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(109, 37);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Currently Installed:";
            // 
            // currentInstalled
            // 
            this.currentInstalled.Location = new System.Drawing.Point(159, 15);
            this.currentInstalled.Name = "currentInstalled";
            this.currentInstalled.ReadOnly = true;
            this.currentInstalled.Size = new System.Drawing.Size(394, 26);
            this.currentInstalled.TabIndex = 12;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Location = new System.Drawing.Point(16, 73);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(773, 10);
            this.panel1.TabIndex = 13;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(666, 132);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 29);
            this.button1.TabIndex = 14;
            this.button1.Text = "Install";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // ConfigurePlugins
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 317);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.currentInstalled);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.pluginVersions);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pluginNames);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ConfigurePlugins";
            this.Text = "ConfigurePlugins";
            this.Activated += new System.EventHandler(this.ConfigurePlugins_Activated);
            this.Load += new System.EventHandler(this.ConfigurePlugins_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox pluginNames;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox pluginVersions;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox currentInstalled;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
    }
}