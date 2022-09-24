namespace CustomSelectFileDlg
{
    partial class ButtonListDlg
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
            this.lsbButtonList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lsbButtonList
            // 
            this.lsbButtonList.FormattingEnabled = true;
            this.lsbButtonList.HorizontalScrollbar = true;
            this.lsbButtonList.ItemHeight = 15;
            this.lsbButtonList.Location = new System.Drawing.Point(0, 0);
            this.lsbButtonList.Margin = new System.Windows.Forms.Padding(0);
            this.lsbButtonList.Name = "lsbButtonList";
            this.lsbButtonList.Size = new System.Drawing.Size(50, 49);
            this.lsbButtonList.TabIndex = 1;
            // 
            // ButtonListDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(500, 200);
            this.Controls.Add(this.lsbButtonList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ButtonListDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ButtonListDlg";
            this.ResumeLayout(false);

        }

        #endregion
        private ListBox lsbButtonList;
    }
}
