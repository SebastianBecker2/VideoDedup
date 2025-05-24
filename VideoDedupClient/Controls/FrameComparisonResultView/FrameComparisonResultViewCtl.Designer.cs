namespace VideoDedupClient.Controls.FrameComparisonResultView
{
    using System.ComponentModel;

    partial class FrameComparisonResultViewCtl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TlpFrameComparison = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // TlpFrameComparison
            // 
            this.TlpFrameComparison.AutoSize = true;
            this.TlpFrameComparison.ColumnCount = 3;
            this.TlpFrameComparison.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.99999F));
            this.TlpFrameComparison.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpFrameComparison.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.00002F));
            this.TlpFrameComparison.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpFrameComparison.Location = new System.Drawing.Point(0, 0);
            this.TlpFrameComparison.Margin = new System.Windows.Forms.Padding(0);
            this.TlpFrameComparison.Name = "TlpFrameComparison";
            this.TlpFrameComparison.RowCount = 2;
            this.TlpFrameComparison.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpFrameComparison.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpFrameComparison.Size = new System.Drawing.Size(1046, 205);
            this.TlpFrameComparison.TabIndex = 0;
            // 
            // FrameComparisonResultViewCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.TlpFrameComparison);
            this.Name = "FrameComparisonResultViewCtl";
            this.Size = new System.Drawing.Size(1046, 205);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TableLayoutPanel TlpFrameComparison;
    }
}
