namespace CustomSelectFileDlg
{
    partial class RefreshableTreeView
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
            Trv = new TreeView();
            SuspendLayout();
            // 
            // Trv
            // 
            Trv.Dock = DockStyle.Fill;
            Trv.Location = new Point(0, 0);
            Trv.Name = "Trv";
            Trv.Size = new Size(150, 150);
            Trv.TabIndex = 0;
            // 
            // RefreshableTreeView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Trv);
            Name = "RefreshableTreeView";
            ResumeLayout(false);
        }

        #endregion

        private TreeView Trv;
    }
}
