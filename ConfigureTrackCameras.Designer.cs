namespace iRacingReplayOverlay
{
    partial class ConfigureTrackCameras
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "TV1",
            "3"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "TV2",
            "3"}, -1);
            this.trackList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cameraList = new System.Windows.Forms.ListView();
            this.CameraName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Ratio = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ratioTextBox = new System.Windows.Forms.TextBox();
            this.ratioLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.totalRatio = new System.Windows.Forms.TextBox();
            this.TotalErrorLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // trackList
            // 
            this.trackList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.trackList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackList.FormattingEnabled = true;
            this.trackList.Location = new System.Drawing.Point(111, 23);
            this.trackList.Margin = new System.Windows.Forms.Padding(4);
            this.trackList.Name = "trackList";
            this.trackList.Size = new System.Drawing.Size(545, 24);
            this.trackList.TabIndex = 0;
            this.trackList.SelectedIndexChanged += new System.EventHandler(this.trackList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Track Name:";
            // 
            // cameraList
            // 
            this.cameraList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.cameraList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CameraName,
            this.Ratio});
            this.cameraList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cameraList.FullRowSelect = true;
            this.cameraList.GridLines = true;
            this.cameraList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.cameraList.HideSelection = false;
            this.cameraList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.cameraList.Location = new System.Drawing.Point(17, 70);
            this.cameraList.Margin = new System.Windows.Forms.Padding(4);
            this.cameraList.Name = "cameraList";
            this.cameraList.Size = new System.Drawing.Size(294, 347);
            this.cameraList.TabIndex = 2;
            this.cameraList.UseCompatibleStateImageBehavior = false;
            this.cameraList.View = System.Windows.Forms.View.Details;
            this.cameraList.SelectedIndexChanged += new System.EventHandler(this.cameraList_SelectedIndexChanged);
            // 
            // CameraName
            // 
            this.CameraName.Text = "Camera Name";
            this.CameraName.Width = 184;
            // 
            // Ratio
            // 
            this.Ratio.Text = "Ratio %";
            this.Ratio.Width = 76;
            // 
            // ratioTextBox
            // 
            this.ratioTextBox.Location = new System.Drawing.Point(469, 70);
            this.ratioTextBox.Name = "ratioTextBox";
            this.ratioTextBox.Size = new System.Drawing.Size(30, 22);
            this.ratioTextBox.TabIndex = 3;
            this.ratioTextBox.Text = "3";
            this.ratioTextBox.TextChanged += new System.EventHandler(this.ratioTextBox_TextChanged);
            // 
            // ratioLabel
            // 
            this.ratioLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ratioLabel.Location = new System.Drawing.Point(319, 73);
            this.ratioLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ratioLabel.Name = "ratioLabel";
            this.ratioLabel.Size = new System.Drawing.Size(143, 19);
            this.ratioLabel.TabIndex = 4;
            this.ratioLabel.Text = "TV1 has a 1 in";
            this.ratioLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(502, 72);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "% chance of being used.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 435);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "Total Ratio Percentages:";
            // 
            // totalRatio
            // 
            this.totalRatio.Location = new System.Drawing.Point(178, 435);
            this.totalRatio.Name = "totalRatio";
            this.totalRatio.Size = new System.Drawing.Size(30, 22);
            this.totalRatio.TabIndex = 7;
            this.totalRatio.Text = "3";
            // 
            // TotalErrorLabel
            // 
            this.TotalErrorLabel.AutoSize = true;
            this.TotalErrorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalErrorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TotalErrorLabel.Location = new System.Drawing.Point(224, 435);
            this.TotalErrorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TotalErrorLabel.Name = "TotalErrorLabel";
            this.TotalErrorLabel.Size = new System.Drawing.Size(221, 16);
            this.TotalErrorLabel.TabIndex = 8;
            this.TotalErrorLabel.Text = "* Must be less than  or equal to100%";
            this.TotalErrorLabel.Visible = false;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(547, 414);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(109, 37);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(325, 219);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(331, 43);
            this.label4.TabIndex = 10;
            this.label4.Text = "TV1 will default if by chance no other camera activates";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(325, 149);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(331, 70);
            this.label5.TabIndex = 11;
            this.label5.Text = "To load the cameras for your track, you need to have iRacing running, with your r" +
    "equired track, before opening this settings dialog box.";
            // 
            // TrackCameraPerferences
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(679, 480);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.TotalErrorLabel);
            this.Controls.Add(this.totalRatio);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ratioLabel);
            this.Controls.Add(this.ratioTextBox);
            this.Controls.Add(this.cameraList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackList);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TrackCameraPerferences";
            this.Text = "TrackCameraPerferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrackCameraPerferences_FormClosing);
            this.Load += new System.EventHandler(this.TrackCameraPerferences_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox trackList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView cameraList;
        private System.Windows.Forms.ColumnHeader CameraName;
        private System.Windows.Forms.ColumnHeader Ratio;
        private System.Windows.Forms.TextBox ratioTextBox;
        private System.Windows.Forms.Label ratioLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox totalRatio;
        private System.Windows.Forms.Label TotalErrorLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;

    }
}