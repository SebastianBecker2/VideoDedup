namespace CustomSelectFileDlg
{
    partial class ResizableButtonArray
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
            this.TlpArray = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // TlpArray
            // 
            this.TlpArray.ColumnCount = 1;
            this.TlpArray.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpArray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpArray.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.TlpArray.Location = new System.Drawing.Point(0, 0);
            this.TlpArray.Name = "TlpArray";
            this.TlpArray.RowCount = 1;
            this.TlpArray.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpArray.Size = new System.Drawing.Size(432, 30);
            this.TlpArray.TabIndex = 0;
            this.TlpArray.Click += new System.EventHandler(this.HandleTlpArray_Click);
            // 
            // ResizableButtonArray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TlpArray);
            this.Name = "ResizableButtonArray";
            this.Size = new System.Drawing.Size(432, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel TlpArray;
    }
}
