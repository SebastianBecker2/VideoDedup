namespace VideoDedup
{

    partial class VideoComparisonPreviewDlg
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
            this.components = new System.ComponentModel.Container();
            this.TlpResult = new System.Windows.Forms.TableLayoutPanel();
            this.GrbThirdLevelLoad = new System.Windows.Forms.GroupBox();
            this.TlpThirdLevelLoad = new System.Windows.Forms.TableLayoutPanel();
            this.GrbFirstLevelLoad = new System.Windows.Forms.GroupBox();
            this.TlpFirstLevelLoad = new System.Windows.Forms.TableLayoutPanel();
            this.GrbSecondLevelLoad = new System.Windows.Forms.GroupBox();
            this.TlpSecondLevelLoad = new System.Windows.Forms.TableLayoutPanel();
            this.LblResult = new System.Windows.Forms.Label();
            this.PnlResult = new System.Windows.Forms.Panel();
            this.GrbResult = new System.Windows.Forms.GroupBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOkay = new System.Windows.Forms.Button();
            this.TlpSettings = new System.Windows.Forms.TableLayoutPanel();
            this.NumMaxImageComparison = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnStartComparison = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtLeftFilePath = new System.Windows.Forms.TextBox();
            this.TxtRightFilePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.NumMaxDifferentImages = new System.Windows.Forms.NumericUpDown();
            this.NumMaxDifferentPercentage = new System.Windows.Forms.NumericUpDown();
            this.BtnSelectLeftFilePath = new System.Windows.Forms.Button();
            this.BtnSelectRightFilePath = new System.Windows.Forms.Button();
            this.StatusTimer = new System.Windows.Forms.Timer(this.components);
            this.TlpResult.SuspendLayout();
            this.GrbThirdLevelLoad.SuspendLayout();
            this.GrbFirstLevelLoad.SuspendLayout();
            this.GrbSecondLevelLoad.SuspendLayout();
            this.PnlResult.SuspendLayout();
            this.GrbResult.SuspendLayout();
            this.TlpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxImageComparison)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentImages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentPercentage)).BeginInit();
            this.SuspendLayout();
            // 
            // TlpResult
            // 
            this.TlpResult.AutoSize = true;
            this.TlpResult.ColumnCount = 1;
            this.TlpResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpResult.Controls.Add(this.GrbThirdLevelLoad, 0, 3);
            this.TlpResult.Controls.Add(this.GrbFirstLevelLoad, 0, 1);
            this.TlpResult.Controls.Add(this.GrbSecondLevelLoad, 0, 2);
            this.TlpResult.Controls.Add(this.LblResult, 0, 0);
            this.TlpResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpResult.Location = new System.Drawing.Point(3, 16);
            this.TlpResult.Name = "TlpResult";
            this.TlpResult.RowCount = 4;
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.Size = new System.Drawing.Size(850, 113);
            this.TlpResult.TabIndex = 0;
            // 
            // GrbThirdLevelLoad
            // 
            this.GrbThirdLevelLoad.AutoSize = true;
            this.GrbThirdLevelLoad.Controls.Add(this.TlpThirdLevelLoad);
            this.GrbThirdLevelLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbThirdLevelLoad.Location = new System.Drawing.Point(3, 91);
            this.GrbThirdLevelLoad.Name = "GrbThirdLevelLoad";
            this.GrbThirdLevelLoad.Size = new System.Drawing.Size(844, 19);
            this.GrbThirdLevelLoad.TabIndex = 3;
            this.GrbThirdLevelLoad.TabStop = false;
            this.GrbThirdLevelLoad.Text = "Third level image load";
            this.GrbThirdLevelLoad.Visible = false;
            // 
            // TlpThirdLevelLoad
            // 
            this.TlpThirdLevelLoad.AutoSize = true;
            this.TlpThirdLevelLoad.ColumnCount = 3;
            this.TlpThirdLevelLoad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.TlpThirdLevelLoad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.TlpThirdLevelLoad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.TlpThirdLevelLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpThirdLevelLoad.Location = new System.Drawing.Point(3, 16);
            this.TlpThirdLevelLoad.Name = "TlpThirdLevelLoad";
            this.TlpThirdLevelLoad.RowCount = 1;
            this.TlpThirdLevelLoad.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpThirdLevelLoad.Size = new System.Drawing.Size(838, 0);
            this.TlpThirdLevelLoad.TabIndex = 0;
            // 
            // GrbFirstLevelLoad
            // 
            this.GrbFirstLevelLoad.AutoSize = true;
            this.GrbFirstLevelLoad.Controls.Add(this.TlpFirstLevelLoad);
            this.GrbFirstLevelLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbFirstLevelLoad.Location = new System.Drawing.Point(3, 41);
            this.GrbFirstLevelLoad.Name = "GrbFirstLevelLoad";
            this.GrbFirstLevelLoad.Size = new System.Drawing.Size(844, 19);
            this.GrbFirstLevelLoad.TabIndex = 0;
            this.GrbFirstLevelLoad.TabStop = false;
            this.GrbFirstLevelLoad.Text = "First level image load";
            // 
            // TlpFirstLevelLoad
            // 
            this.TlpFirstLevelLoad.AutoSize = true;
            this.TlpFirstLevelLoad.ColumnCount = 3;
            this.TlpFirstLevelLoad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.TlpFirstLevelLoad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.TlpFirstLevelLoad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.TlpFirstLevelLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpFirstLevelLoad.Location = new System.Drawing.Point(3, 16);
            this.TlpFirstLevelLoad.Name = "TlpFirstLevelLoad";
            this.TlpFirstLevelLoad.RowCount = 1;
            this.TlpFirstLevelLoad.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpFirstLevelLoad.Size = new System.Drawing.Size(838, 0);
            this.TlpFirstLevelLoad.TabIndex = 0;
            // 
            // GrbSecondLevelLoad
            // 
            this.GrbSecondLevelLoad.AutoSize = true;
            this.GrbSecondLevelLoad.Controls.Add(this.TlpSecondLevelLoad);
            this.GrbSecondLevelLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbSecondLevelLoad.Location = new System.Drawing.Point(3, 66);
            this.GrbSecondLevelLoad.Name = "GrbSecondLevelLoad";
            this.GrbSecondLevelLoad.Size = new System.Drawing.Size(844, 19);
            this.GrbSecondLevelLoad.TabIndex = 1;
            this.GrbSecondLevelLoad.TabStop = false;
            this.GrbSecondLevelLoad.Text = "Second level image load";
            this.GrbSecondLevelLoad.Visible = false;
            // 
            // TlpSecondLevelLoad
            // 
            this.TlpSecondLevelLoad.AutoSize = true;
            this.TlpSecondLevelLoad.ColumnCount = 3;
            this.TlpSecondLevelLoad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpSecondLevelLoad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.TlpSecondLevelLoad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.TlpSecondLevelLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpSecondLevelLoad.Location = new System.Drawing.Point(3, 16);
            this.TlpSecondLevelLoad.Name = "TlpSecondLevelLoad";
            this.TlpSecondLevelLoad.RowCount = 1;
            this.TlpSecondLevelLoad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpSecondLevelLoad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.TlpSecondLevelLoad.Size = new System.Drawing.Size(838, 0);
            this.TlpSecondLevelLoad.TabIndex = 0;
            // 
            // LblResult
            // 
            this.LblResult.AutoSize = true;
            this.LblResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblResult.Location = new System.Drawing.Point(3, 0);
            this.LblResult.Name = "LblResult";
            this.LblResult.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.LblResult.Size = new System.Drawing.Size(844, 38);
            this.LblResult.TabIndex = 2;
            this.LblResult.Text = "Result text";
            this.LblResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PnlResult
            // 
            this.PnlResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PnlResult.AutoScroll = true;
            this.PnlResult.Controls.Add(this.GrbResult);
            this.PnlResult.Location = new System.Drawing.Point(12, 158);
            this.PnlResult.Name = "PnlResult";
            this.PnlResult.Size = new System.Drawing.Size(862, 420);
            this.PnlResult.TabIndex = 2;
            // 
            // GrbResult
            // 
            this.GrbResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GrbResult.AutoSize = true;
            this.GrbResult.Controls.Add(this.TlpResult);
            this.GrbResult.Location = new System.Drawing.Point(3, 3);
            this.GrbResult.Name = "GrbResult";
            this.GrbResult.Size = new System.Drawing.Size(856, 132);
            this.GrbResult.TabIndex = 1;
            this.GrbResult.TabStop = false;
            this.GrbResult.Text = "Result";
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(799, 584);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 23;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            this.BtnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOkay.Location = new System.Drawing.Point(718, 584);
            this.BtnOkay.Name = "BtnOkay";
            this.BtnOkay.Size = new System.Drawing.Size(75, 23);
            this.BtnOkay.TabIndex = 22;
            this.BtnOkay.Text = "&OK";
            this.BtnOkay.UseVisualStyleBackColor = true;
            this.BtnOkay.Click += new System.EventHandler(this.BtnOkay_Click);
            // 
            // TlpSettings
            // 
            this.TlpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TlpSettings.ColumnCount = 5;
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.Controls.Add(this.NumMaxImageComparison, 2, 2);
            this.TlpSettings.Controls.Add(this.label1, 0, 0);
            this.TlpSettings.Controls.Add(this.BtnStartComparison, 3, 4);
            this.TlpSettings.Controls.Add(this.label2, 0, 1);
            this.TlpSettings.Controls.Add(this.TxtLeftFilePath, 1, 0);
            this.TlpSettings.Controls.Add(this.TxtRightFilePath, 1, 1);
            this.TlpSettings.Controls.Add(this.label3, 0, 2);
            this.TlpSettings.Controls.Add(this.label4, 0, 3);
            this.TlpSettings.Controls.Add(this.label5, 0, 4);
            this.TlpSettings.Controls.Add(this.NumMaxDifferentImages, 2, 3);
            this.TlpSettings.Controls.Add(this.NumMaxDifferentPercentage, 2, 4);
            this.TlpSettings.Controls.Add(this.BtnSelectLeftFilePath, 4, 0);
            this.TlpSettings.Controls.Add(this.BtnSelectRightFilePath, 4, 1);
            this.TlpSettings.Location = new System.Drawing.Point(12, 12);
            this.TlpSettings.Name = "TlpSettings";
            this.TlpSettings.RowCount = 5;
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.Size = new System.Drawing.Size(862, 140);
            this.TlpSettings.TabIndex = 24;
            // 
            // NumMaxImageComparison
            // 
            this.NumMaxImageComparison.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.NumMaxImageComparison.Location = new System.Drawing.Point(193, 60);
            this.NumMaxImageComparison.Name = "NumMaxImageComparison";
            this.NumMaxImageComparison.Size = new System.Drawing.Size(63, 20);
            this.NumMaxImageComparison.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Left File:";
            // 
            // BtnStartComparison
            // 
            this.BtnStartComparison.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TlpSettings.SetColumnSpan(this.BtnStartComparison, 2);
            this.BtnStartComparison.Location = new System.Drawing.Point(736, 115);
            this.BtnStartComparison.Name = "BtnStartComparison";
            this.BtnStartComparison.Size = new System.Drawing.Size(123, 22);
            this.BtnStartComparison.TabIndex = 22;
            this.BtnStartComparison.Text = "Start comparison";
            this.BtnStartComparison.UseVisualStyleBackColor = true;
            this.BtnStartComparison.Click += new System.EventHandler(this.BtnStartComparison_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Right File:";
            // 
            // TxtLeftFilePath
            // 
            this.TxtLeftFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TlpSettings.SetColumnSpan(this.TxtLeftFilePath, 3);
            this.TxtLeftFilePath.Location = new System.Drawing.Point(63, 4);
            this.TxtLeftFilePath.Name = "TxtLeftFilePath";
            this.TxtLeftFilePath.Size = new System.Drawing.Size(766, 20);
            this.TxtLeftFilePath.TabIndex = 2;
            // 
            // TxtRightFilePath
            // 
            this.TxtRightFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TlpSettings.SetColumnSpan(this.TxtRightFilePath, 3);
            this.TxtRightFilePath.Location = new System.Drawing.Point(63, 32);
            this.TxtRightFilePath.Name = "TxtRightFilePath";
            this.TxtRightFilePath.Size = new System.Drawing.Size(766, 20);
            this.TxtRightFilePath.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.TlpSettings.SetColumnSpan(this.label3, 2);
            this.label3.Location = new System.Drawing.Point(35, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Number of Images to compare:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.TlpSettings.SetColumnSpan(this.label4, 2);
            this.label4.Location = new System.Drawing.Point(3, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(184, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Accepted number of different Images:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.TlpSettings.SetColumnSpan(this.label5, 2);
            this.label5.Location = new System.Drawing.Point(12, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Accepted percentage of difference:";
            // 
            // NumMaxDifferentImages
            // 
            this.NumMaxDifferentImages.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.NumMaxDifferentImages.Location = new System.Drawing.Point(193, 88);
            this.NumMaxDifferentImages.Name = "NumMaxDifferentImages";
            this.NumMaxDifferentImages.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDifferentImages.TabIndex = 24;
            // 
            // NumMaxDifferentPercentage
            // 
            this.NumMaxDifferentPercentage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.NumMaxDifferentPercentage.Location = new System.Drawing.Point(193, 116);
            this.NumMaxDifferentPercentage.Name = "NumMaxDifferentPercentage";
            this.NumMaxDifferentPercentage.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDifferentPercentage.TabIndex = 25;
            // 
            // BtnSelectLeftFilePath
            // 
            this.BtnSelectLeftFilePath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnSelectLeftFilePath.Location = new System.Drawing.Point(835, 3);
            this.BtnSelectLeftFilePath.Name = "BtnSelectLeftFilePath";
            this.BtnSelectLeftFilePath.Size = new System.Drawing.Size(24, 22);
            this.BtnSelectLeftFilePath.TabIndex = 20;
            this.BtnSelectLeftFilePath.Text = "...";
            this.BtnSelectLeftFilePath.UseVisualStyleBackColor = true;
            this.BtnSelectLeftFilePath.Click += new System.EventHandler(this.BtnSelectLeftFilePath_Click);
            // 
            // BtnSelectRightFilePath
            // 
            this.BtnSelectRightFilePath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnSelectRightFilePath.Location = new System.Drawing.Point(835, 31);
            this.BtnSelectRightFilePath.Name = "BtnSelectRightFilePath";
            this.BtnSelectRightFilePath.Size = new System.Drawing.Size(24, 22);
            this.BtnSelectRightFilePath.TabIndex = 20;
            this.BtnSelectRightFilePath.Text = "...";
            this.BtnSelectRightFilePath.UseVisualStyleBackColor = true;
            this.BtnSelectRightFilePath.Click += new System.EventHandler(this.BtnSelectRightFilePath_Click);
            // 
            // StatusTimer
            // 
            this.StatusTimer.Tick += new System.EventHandler(this.HandleStatusTimerTick);
            // 
            // VideoComparisonPreview
            // 
            this.AcceptButton = this.BtnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(886, 619);
            this.Controls.Add(this.TlpSettings);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOkay);
            this.Controls.Add(this.PnlResult);
            this.Name = "VideoComparisonPreview";
            this.Text = "VideoComparisonPreview";
            this.TlpResult.ResumeLayout(false);
            this.TlpResult.PerformLayout();
            this.GrbThirdLevelLoad.ResumeLayout(false);
            this.GrbThirdLevelLoad.PerformLayout();
            this.GrbFirstLevelLoad.ResumeLayout(false);
            this.GrbFirstLevelLoad.PerformLayout();
            this.GrbSecondLevelLoad.ResumeLayout(false);
            this.GrbSecondLevelLoad.PerformLayout();
            this.PnlResult.ResumeLayout(false);
            this.PnlResult.PerformLayout();
            this.GrbResult.ResumeLayout(false);
            this.GrbResult.PerformLayout();
            this.TlpSettings.ResumeLayout(false);
            this.TlpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxImageComparison)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentImages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentPercentage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpResult;
        private System.Windows.Forms.Panel PnlResult;
        private System.Windows.Forms.GroupBox GrbFirstLevelLoad;
        private System.Windows.Forms.TableLayoutPanel TlpFirstLevelLoad;
        private System.Windows.Forms.GroupBox GrbSecondLevelLoad;
        private System.Windows.Forms.TableLayoutPanel TlpSecondLevelLoad;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnOkay;
        private System.Windows.Forms.TableLayoutPanel TlpSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtLeftFilePath;
        private System.Windows.Forms.TextBox TxtRightFilePath;
        private System.Windows.Forms.Button BtnSelectLeftFilePath;
        private System.Windows.Forms.Button BtnSelectRightFilePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown NumMaxImageComparison;
        private System.Windows.Forms.NumericUpDown NumMaxDifferentImages;
        private System.Windows.Forms.NumericUpDown NumMaxDifferentPercentage;
        private System.Windows.Forms.GroupBox GrbResult;
        private System.Windows.Forms.Label LblResult;
        private System.Windows.Forms.GroupBox GrbThirdLevelLoad;
        private System.Windows.Forms.TableLayoutPanel TlpThirdLevelLoad;
        private System.Windows.Forms.Timer StatusTimer;
        private System.Windows.Forms.Button BtnStartComparison;
    }
}
