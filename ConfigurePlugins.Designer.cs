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
            this.SuspendLayout();
            // 
            // pluginNames
            // 
            this.pluginNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pluginNames.FormattingEnabled = true;
            this.pluginNames.Location = new System.Drawing.Point(13, 62);
            this.pluginNames.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pluginNames.Name = "pluginNames";
            this.pluginNames.Size = new System.Drawing.Size(319, 28);
            this.pluginNames.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(384, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Version:";
            // 
            // pluginVersions
            // 
            this.pluginVersions.FormattingEnabled = true;
            this.pluginVersions.Location = new System.Drawing.Point(388, 62);
            this.pluginVersions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pluginVersions.Name = "pluginVersions";
            this.pluginVersions.Size = new System.Drawing.Size(263, 28);
            this.pluginVersions.TabIndex = 3;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(588, 268);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(109, 37);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ConfigurePlugins
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 317);
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
    }
}