namespace CustomSelectFileDlg
{
    partial class ResizableTextBox
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
            this.TxtInnerTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TxtInnerTextBox
            // 
            this.TxtInnerTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.TxtInnerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtInnerTextBox.Location = new System.Drawing.Point(0, 35);
            this.TxtInnerTextBox.Name = "TxtInnerTextBox";
            this.TxtInnerTextBox.Size = new System.Drawing.Size(100, 16);
            this.TxtInnerTextBox.TabIndex = 0;
            // 
            // ResizableTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.TxtInnerTextBox);
            this.Name = "ResizableTextBox";
            this.Size = new System.Drawing.Size(620, 94);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox TxtInnerTextBox;
    }
}
