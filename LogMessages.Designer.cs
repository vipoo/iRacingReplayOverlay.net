namespace IRacingReplayOverlay
{
    partial class LogMessages
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.TraceMessageTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // TraceMessageTextBox
            // 
            this.TraceMessageTextBox.BackColor = System.Drawing.Color.Black;
            this.TraceMessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TraceMessageTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TraceMessageTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TraceMessageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TraceMessageTextBox.ForeColor = System.Drawing.Color.Lime;
            this.TraceMessageTextBox.Location = new System.Drawing.Point(0, 0);
            this.TraceMessageTextBox.MaxLength = 1040000;
            this.TraceMessageTextBox.Multiline = true;
            this.TraceMessageTextBox.Name = "TraceMessageTextBox";
            this.TraceMessageTextBox.ReadOnly = true;
            this.TraceMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TraceMessageTextBox.Size = new System.Drawing.Size(927, 540);
            this.TraceMessageTextBox.TabIndex = 1;
            this.TraceMessageTextBox.TabStop = false;
            this.TraceMessageTextBox.Text = "";
            this.TraceMessageTextBox.WordWrap = false;
            // 
            // LogMessages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(927, 540);
            this.Controls.Add(this.TraceMessageTextBox);
            this.Controls.Add(this.flowLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "LogMessages";
            this.Text = "LogMessages";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox TraceMessageTextBox;
    }
}