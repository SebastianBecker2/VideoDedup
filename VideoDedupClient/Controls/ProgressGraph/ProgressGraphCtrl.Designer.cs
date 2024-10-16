namespace VideoDedupClient.Controls.ProgressGraph
{
    partial class ProgressGraphCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Pv = new OxyPlot.WindowsForms.PlotView();
            SuspendLayout();
            // 
            // Pv
            // 
            Pv.Dock = DockStyle.Fill;
            Pv.Location = new Point(0, 0);
            Pv.Name = "Pv";
            Pv.PanCursor = Cursors.Hand;
            Pv.Size = new Size(479, 111);
            Pv.TabIndex = 0;
            Pv.Text = "plotView1";
            Pv.ZoomHorizontalCursor = Cursors.SizeWE;
            Pv.ZoomRectangleCursor = Cursors.SizeNWSE;
            Pv.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // ProgressGraphCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Pv);
            Name = "ProgressGraphCtrl";
            Size = new Size(479, 111);
            ResumeLayout(false);
        }

        #endregion

        private OxyPlot.WindowsForms.PlotView Pv;
    }
}
