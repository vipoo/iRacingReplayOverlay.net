namespace iRacingDirector.Plugin.Tester
{
    partial class Main
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
            this.browseBackgroundImageButton = new System.Windows.Forms.Button();
            this.fullSizeButton = new System.Windows.Forms.Button();
            this.halfSizeButton = new System.Windows.Forms.Button();
            this.thirdSizeButton = new System.Windows.Forms.Button();
            this.introFlashCardButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundTestImageFileName = new System.Windows.Forms.TextBox();
            this.pluginAssemblyFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.browsePluginButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // browseBackgroundImageButton
            // 
            this.browseBackgroundImageButton.Location = new System.Drawing.Point(749, 52);
            this.browseBackgroundImageButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.browseBackgroundImageButton.Name = "browseBackgroundImageButton";
            this.browseBackgroundImageButton.Size = new System.Drawing.Size(80, 27);
            this.browseBackgroundImageButton.TabIndex = 0;
            this.browseBackgroundImageButton.Text = "browse";
            this.browseBackgroundImageButton.UseVisualStyleBackColor = true;
            this.browseBackgroundImageButton.Click += new System.EventHandler(this.browseBackgroundImageButton_Click);
            // 
            // fullSizeButton
            // 
            this.fullSizeButton.Location = new System.Drawing.Point(26, 162);
            this.fullSizeButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fullSizeButton.Name = "fullSizeButton";
            this.fullSizeButton.Size = new System.Drawing.Size(155, 31);
            this.fullSizeButton.TabIndex = 1;
            this.fullSizeButton.Text = "Full Size";
            this.fullSizeButton.UseVisualStyleBackColor = true;
            this.fullSizeButton.Click += new System.EventHandler(this.fullSizeButton_Click);
            // 
            // halfSizeButton
            // 
            this.halfSizeButton.Location = new System.Drawing.Point(26, 202);
            this.halfSizeButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.halfSizeButton.Name = "halfSizeButton";
            this.halfSizeButton.Size = new System.Drawing.Size(155, 31);
            this.halfSizeButton.TabIndex = 2;
            this.halfSizeButton.Text = "Half Size";
            this.halfSizeButton.UseVisualStyleBackColor = true;
            this.halfSizeButton.Click += new System.EventHandler(this.halfSizeButton_Click);
            // 
            // thirdSizeButton
            // 
            this.thirdSizeButton.Location = new System.Drawing.Point(26, 243);
            this.thirdSizeButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.thirdSizeButton.Name = "thirdSizeButton";
            this.thirdSizeButton.Size = new System.Drawing.Size(155, 31);
            this.thirdSizeButton.TabIndex = 3;
            this.thirdSizeButton.Text = "Third Size";
            this.thirdSizeButton.UseVisualStyleBackColor = true;
            this.thirdSizeButton.Click += new System.EventHandler(this.thirdSizeButton_Click);
            // 
            // introFlashCardButton
            // 
            this.introFlashCardButton.Location = new System.Drawing.Point(260, 162);
            this.introFlashCardButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.introFlashCardButton.Name = "introFlashCardButton";
            this.introFlashCardButton.Size = new System.Drawing.Size(186, 48);
            this.introFlashCardButton.TabIndex = 4;
            this.introFlashCardButton.Text = "Intro Flash Card";
            this.introFlashCardButton.UseVisualStyleBackColor = true;
            this.introFlashCardButton.Click += new System.EventHandler(this.introFlashCardButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Test Background Image:";
            // 
            // backgroundTestImageFileName
            // 
            this.backgroundTestImageFileName.Location = new System.Drawing.Point(203, 53);
            this.backgroundTestImageFileName.Name = "backgroundTestImageFileName";
            this.backgroundTestImageFileName.Size = new System.Drawing.Size(539, 26);
            this.backgroundTestImageFileName.TabIndex = 6;
            this.backgroundTestImageFileName.Leave += new System.EventHandler(this.backgroundTestImageFileName_Leave);
            // 
            // pluginAssemblyFileName
            // 
            this.pluginAssemblyFileName.Location = new System.Drawing.Point(203, 21);
            this.pluginAssemblyFileName.Name = "pluginAssemblyFileName";
            this.pluginAssemblyFileName.Size = new System.Drawing.Size(539, 26);
            this.pluginAssemblyFileName.TabIndex = 9;
            this.pluginAssemblyFileName.Leave += new System.EventHandler(this.pluginAssemblyFileName_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "Plugin Binary (dll):";
            // 
            // browsePluginButton
            // 
            this.browsePluginButton.Location = new System.Drawing.Point(749, 20);
            this.browsePluginButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.browsePluginButton.Name = "browsePluginButton";
            this.browsePluginButton.Size = new System.Drawing.Size(80, 27);
            this.browsePluginButton.TabIndex = 7;
            this.browsePluginButton.Text = "browse";
            this.browsePluginButton.UseVisualStyleBackColor = true;
            this.browsePluginButton.Click += new System.EventHandler(this.browsePluginButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 126);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "Show Test Image:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 312);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pluginAssemblyFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.browsePluginButton);
            this.Controls.Add(this.backgroundTestImageFileName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.introFlashCardButton);
            this.Controls.Add(this.thirdSizeButton);
            this.Controls.Add(this.halfSizeButton);
            this.Controls.Add(this.fullSizeButton);
            this.Controls.Add(this.browseBackgroundImageButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button browseBackgroundImageButton;
        private System.Windows.Forms.Button fullSizeButton;
        private System.Windows.Forms.Button halfSizeButton;
        private System.Windows.Forms.Button thirdSizeButton;
        private System.Windows.Forms.Button introFlashCardButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox backgroundTestImageFileName;
        private System.Windows.Forms.TextBox pluginAssemblyFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button browsePluginButton;
        private System.Windows.Forms.Label label3;
    }
}

