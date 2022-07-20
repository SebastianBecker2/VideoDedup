namespace CustomSelectFileDlg
{
    partial class PathBox
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
            this.TxtPath = new CustomSelectFileDlg.ResizableTextBox();
            this.RbaButtons = new CustomSelectFileDlg.ResizableButtonArray();
            this.SuspendLayout();
            // 
            // TxtPath
            // 
            this.TxtPath.BackColor = System.Drawing.SystemColors.Window;
            this.TxtPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtPath.Location = new System.Drawing.Point(0, 0);
            this.TxtPath.Margin = new System.Windows.Forms.Padding(0);
            this.TxtPath.MinimumSize = new System.Drawing.Size(0, 25);
            this.TxtPath.Name = "TxtPath";
            this.TxtPath.Size = new System.Drawing.Size(436, 25);
            this.TxtPath.TabIndex = 0;
            this.TxtPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleTxtPathKeyDown);
            this.TxtPath.Leave += new System.EventHandler(this.HandleTxtPathLeave);
            // 
            // RbaButtons
            // 
            this.RbaButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RbaButtons.Elements = null;
            this.RbaButtons.Location = new System.Drawing.Point(0, 0);
            this.RbaButtons.Name = "RbaButtons";
            this.RbaButtons.Size = new System.Drawing.Size(436, 25);
            this.RbaButtons.TabIndex = 3;
            this.RbaButtons.Visible = false;
            this.RbaButtons.ElementClick += new System.EventHandler<CustomSelectFileDlg.EventArgs.ElementClickEventArgs>(this.HandleRbaButtonsElementClick);
            this.RbaButtons.Click += new System.EventHandler(this.HandleRbaButtonsClick);
            // 
            // PathBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.RbaButtons);
            this.Controls.Add(this.TxtPath);
            this.MaximumSize = new System.Drawing.Size(999999999, 27);
            this.MinimumSize = new System.Drawing.Size(0, 27);
            this.Name = "PathBox";
            this.Size = new System.Drawing.Size(436, 25);
            this.ResumeLayout(false);

        }

        #endregion

        private ResizableTextBox TxtPath;
        private ResizableButtonArray RbaButtons;
    }
}
