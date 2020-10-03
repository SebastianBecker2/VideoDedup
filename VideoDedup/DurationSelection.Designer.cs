namespace VideoDedup
{
    partial class DurationSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DurationSelection));
            this.MaximumSlider = new System.Windows.Forms.TrackBar();
            this.LblDurationInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.MinimumSlider = new System.Windows.Forms.TrackBar();
            this.TxtMinimumDuration = new System.Windows.Forms.TextBox();
            this.TxtMaximumDuration = new System.Windows.Forms.TextBox();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.MaximumSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinimumSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // MaximumSlider
            // 
            this.MaximumSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MaximumSlider.Location = new System.Drawing.Point(12, 240);
            this.MaximumSlider.Name = "MaximumSlider";
            this.MaximumSlider.Size = new System.Drawing.Size(415, 45);
            this.MaximumSlider.TabIndex = 0;
            this.MaximumSlider.Scroll += new System.EventHandler(this.MaximumSlider_Scroll);
            // 
            // LblDurationInfo
            // 
            this.LblDurationInfo.AutoSize = true;
            this.LblDurationInfo.Location = new System.Drawing.Point(12, 13);
            this.LblDurationInfo.Name = "LblDurationInfo";
            this.LblDurationInfo.Size = new System.Drawing.Size(238, 13);
            this.LblDurationInfo.TabIndex = 1;
            this.LblDurationInfo.Text = "Videos are between 00:00:00 and 00:00:00 long.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select minimum duration:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 198);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select maximum duration:";
            // 
            // MinimumSlider
            // 
            this.MinimumSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MinimumSlider.Location = new System.Drawing.Point(12, 116);
            this.MinimumSlider.Name = "MinimumSlider";
            this.MinimumSlider.Size = new System.Drawing.Size(415, 45);
            this.MinimumSlider.TabIndex = 4;
            this.MinimumSlider.Scroll += new System.EventHandler(this.MinimumSlider_Scroll);
            // 
            // TxtMinimumDuration
            // 
            this.TxtMinimumDuration.Location = new System.Drawing.Point(12, 82);
            this.TxtMinimumDuration.Name = "TxtMinimumDuration";
            this.TxtMinimumDuration.Size = new System.Drawing.Size(121, 20);
            this.TxtMinimumDuration.TabIndex = 5;
            this.TxtMinimumDuration.TextChanged += new System.EventHandler(this.TxtMinimumDuration_TextChanged);
            // 
            // TxtMaximumDuration
            // 
            this.TxtMaximumDuration.Location = new System.Drawing.Point(12, 214);
            this.TxtMaximumDuration.Name = "TxtMaximumDuration";
            this.TxtMaximumDuration.Size = new System.Drawing.Size(121, 20);
            this.TxtMaximumDuration.TabIndex = 6;
            this.TxtMaximumDuration.TextChanged += new System.EventHandler(this.TxtMaximumDuration_TextChanged);
            // 
            // BtnOK
            // 
            this.BtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOK.Location = new System.Drawing.Point(271, 302);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 7;
            this.BtnOK.Text = "&OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(352, 302);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 8;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // DurationSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 337);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.TxtMaximumDuration);
            this.Controls.Add(this.TxtMinimumDuration);
            this.Controls.Add(this.MinimumSlider);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LblDurationInfo);
            this.Controls.Add(this.MaximumSlider);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DurationSelection";
            this.Text = "DurationSelection";
            ((System.ComponentModel.ISupportInitialize)(this.MaximumSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinimumSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar MaximumSlider;
        private System.Windows.Forms.Label LblDurationInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar MinimumSlider;
        private System.Windows.Forms.TextBox TxtMinimumDuration;
        private System.Windows.Forms.TextBox TxtMaximumDuration;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnCancel;
    }
}