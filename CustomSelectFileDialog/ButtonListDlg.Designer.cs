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
            this.TlpButtonLists = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // TlpButtonLists
            // 
            this.TlpButtonLists.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TlpButtonLists.ColumnCount = 1;
            this.TlpButtonLists.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpButtonLists.Location = new System.Drawing.Point(0, 0);
            this.TlpButtonLists.Margin = new System.Windows.Forms.Padding(0);
            this.TlpButtonLists.Name = "TlpButtonLists";
            this.TlpButtonLists.RowCount = 1;
            this.TlpButtonLists.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpButtonLists.Size = new System.Drawing.Size(178, 139);
            this.TlpButtonLists.TabIndex = 2;
            // 
            // ButtonListDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(220, 160);
            this.Controls.Add(this.TlpButtonLists);
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
        private TableLayoutPanel TlpButtonLists;
    }
}
