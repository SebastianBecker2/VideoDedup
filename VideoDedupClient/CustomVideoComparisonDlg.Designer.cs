namespace VideoDedup
{

    partial class CustomVideoComparisonDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomVideoComparisonDlg));
            this.TlpResult = new System.Windows.Forms.TableLayoutPanel();
            this.TlpThirdLoadLevel = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.LblThirdLoadLevel = new System.Windows.Forms.Label();
            this.GrbThirdLoadLevel = new System.Windows.Forms.GroupBox();
            this.TlpThirdLoadLevelResult = new System.Windows.Forms.TableLayoutPanel();
            this.TlpSecondLoadLevel = new System.Windows.Forms.TableLayoutPanel();
            this.LblSecondLoadLevel = new System.Windows.Forms.Label();
            this.PibSecondLoadLevel = new System.Windows.Forms.PictureBox();
            this.GrbSecondLoadLevel = new System.Windows.Forms.GroupBox();
            this.TlpSecondLoadLevelResult = new System.Windows.Forms.TableLayoutPanel();
            this.TlpFirstLoadLevel = new System.Windows.Forms.TableLayoutPanel();
            this.PibFirstLoadLevel = new System.Windows.Forms.PictureBox();
            this.LblFirstLoadLevel = new System.Windows.Forms.Label();
            this.GrbFirstLoadLevel = new System.Windows.Forms.GroupBox();
            this.TlpFirstLoadLevelResult = new System.Windows.Forms.TableLayoutPanel();
            this.GrbVideoTimeline = new System.Windows.Forms.GroupBox();
            this.TlpVideoTimeline = new System.Windows.Forms.TableLayoutPanel();
            this.TlpProgress = new System.Windows.Forms.TableLayoutPanel();
            this.LblResult = new System.Windows.Forms.Label();
            this.PgbComparisonProgress = new System.Windows.Forms.ProgressBar();
            this.TxtLeftFileInfo = new System.Windows.Forms.TextBox();
            this.TxtRightFileInfo = new System.Windows.Forms.TextBox();
            this.PnlResult = new System.Windows.Forms.Panel();
            this.GrbResult = new System.Windows.Forms.GroupBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOkay = new System.Windows.Forms.Button();
            this.TlpSettings = new System.Windows.Forms.TableLayoutPanel();
            this.TrbMaxImageComparison = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnStartComparison = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtLeftFilePath = new System.Windows.Forms.TextBox();
            this.TxtRightFilePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BtnSelectLeftFilePath = new System.Windows.Forms.Button();
            this.BtnSelectRightFilePath = new System.Windows.Forms.Button();
            this.TrbMaxDifferentImages = new System.Windows.Forms.TrackBar();
            this.TrbMaxDifferentPercentage = new System.Windows.Forms.TrackBar();
            this.NumMaxImageComparison = new System.Windows.Forms.NumericUpDown();
            this.NumMaxDifferentImages = new System.Windows.Forms.NumericUpDown();
            this.NumMaxDifferentPercentage = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.RdbSortByProcessingOrder = new System.Windows.Forms.RadioButton();
            this.RdbSortByTimeline = new System.Windows.Forms.RadioButton();
            this.StatusTimer = new System.Windows.Forms.Timer(this.components);
            this.btnClose = new System.Windows.Forms.Button();
            this.GrbSettings = new System.Windows.Forms.GroupBox();
            this.TlpResult.SuspendLayout();
            this.TlpThirdLoadLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.GrbThirdLoadLevel.SuspendLayout();
            this.TlpSecondLoadLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PibSecondLoadLevel)).BeginInit();
            this.GrbSecondLoadLevel.SuspendLayout();
            this.TlpFirstLoadLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PibFirstLoadLevel)).BeginInit();
            this.GrbFirstLoadLevel.SuspendLayout();
            this.GrbVideoTimeline.SuspendLayout();
            this.TlpProgress.SuspendLayout();
            this.PnlResult.SuspendLayout();
            this.GrbResult.SuspendLayout();
            this.TlpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrbMaxImageComparison)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrbMaxDifferentImages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrbMaxDifferentPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxImageComparison)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentImages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentPercentage)).BeginInit();
            this.GrbSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // TlpResult
            // 
            this.TlpResult.AutoSize = true;
            this.TlpResult.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TlpResult.ColumnCount = 2;
            this.TlpResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpResult.Controls.Add(this.TlpThirdLoadLevel, 0, 4);
            this.TlpResult.Controls.Add(this.TlpSecondLoadLevel, 0, 3);
            this.TlpResult.Controls.Add(this.TlpFirstLoadLevel, 0, 2);
            this.TlpResult.Controls.Add(this.GrbVideoTimeline, 0, 5);
            this.TlpResult.Controls.Add(this.TlpProgress, 0, 0);
            this.TlpResult.Controls.Add(this.TxtLeftFileInfo, 0, 1);
            this.TlpResult.Controls.Add(this.TxtRightFileInfo, 1, 1);
            this.TlpResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpResult.Location = new System.Drawing.Point(3, 16);
            this.TlpResult.Name = "TlpResult";
            this.TlpResult.RowCount = 6;
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpResult.Size = new System.Drawing.Size(1033, 383);
            this.TlpResult.TabIndex = 0;
            // 
            // TlpThirdLoadLevel
            // 
            this.TlpThirdLoadLevel.AutoSize = true;
            this.TlpThirdLoadLevel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TlpThirdLoadLevel.ColumnCount = 2;
            this.TlpResult.SetColumnSpan(this.TlpThirdLoadLevel, 2);
            this.TlpThirdLoadLevel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpThirdLoadLevel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpThirdLoadLevel.Controls.Add(this.pictureBox1, 0, 0);
            this.TlpThirdLoadLevel.Controls.Add(this.LblThirdLoadLevel, 1, 0);
            this.TlpThirdLoadLevel.Controls.Add(this.GrbThirdLoadLevel, 0, 1);
            this.TlpThirdLoadLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpThirdLoadLevel.Location = new System.Drawing.Point(3, 292);
            this.TlpThirdLoadLevel.Name = "TlpThirdLoadLevel";
            this.TlpThirdLoadLevel.RowCount = 2;
            this.TlpThirdLoadLevel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpThirdLoadLevel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpThirdLoadLevel.Size = new System.Drawing.Size(1027, 63);
            this.TlpThirdLoadLevel.TabIndex = 33;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VideoDedup.Properties.Resources.ArrowUpGray;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.ThirdLoadLevelHeaderClicked);
            // 
            // LblThirdLoadLevel
            // 
            this.LblThirdLoadLevel.AutoSize = true;
            this.LblThirdLoadLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblThirdLoadLevel.Location = new System.Drawing.Point(41, 0);
            this.LblThirdLoadLevel.Name = "LblThirdLoadLevel";
            this.LblThirdLoadLevel.Size = new System.Drawing.Size(983, 38);
            this.LblThirdLoadLevel.TabIndex = 1;
            this.LblThirdLoadLevel.Text = "Third level image load";
            this.LblThirdLoadLevel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblThirdLoadLevel.Click += new System.EventHandler(this.ThirdLoadLevelHeaderClicked);
            // 
            // GrbThirdLoadLevel
            // 
            this.GrbThirdLoadLevel.AutoSize = true;
            this.TlpThirdLoadLevel.SetColumnSpan(this.GrbThirdLoadLevel, 2);
            this.GrbThirdLoadLevel.Controls.Add(this.TlpThirdLoadLevelResult);
            this.GrbThirdLoadLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbThirdLoadLevel.Location = new System.Drawing.Point(3, 41);
            this.GrbThirdLoadLevel.Name = "GrbThirdLoadLevel";
            this.GrbThirdLoadLevel.Size = new System.Drawing.Size(1021, 19);
            this.GrbThirdLoadLevel.TabIndex = 3;
            this.GrbThirdLoadLevel.TabStop = false;
            // 
            // TlpThirdLoadLevelResult
            // 
            this.TlpThirdLoadLevelResult.AutoSize = true;
            this.TlpThirdLoadLevelResult.ColumnCount = 1;
            this.TlpThirdLoadLevelResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpThirdLoadLevelResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpThirdLoadLevelResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpThirdLoadLevelResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpThirdLoadLevelResult.Location = new System.Drawing.Point(3, 16);
            this.TlpThirdLoadLevelResult.Name = "TlpThirdLoadLevelResult";
            this.TlpThirdLoadLevelResult.RowCount = 1;
            this.TlpThirdLoadLevelResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpThirdLoadLevelResult.Size = new System.Drawing.Size(1015, 0);
            this.TlpThirdLoadLevelResult.TabIndex = 0;
            // 
            // TlpSecondLoadLevel
            // 
            this.TlpSecondLoadLevel.AutoSize = true;
            this.TlpSecondLoadLevel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TlpSecondLoadLevel.ColumnCount = 2;
            this.TlpResult.SetColumnSpan(this.TlpSecondLoadLevel, 2);
            this.TlpSecondLoadLevel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSecondLoadLevel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSecondLoadLevel.Controls.Add(this.LblSecondLoadLevel, 1, 0);
            this.TlpSecondLoadLevel.Controls.Add(this.PibSecondLoadLevel, 0, 0);
            this.TlpSecondLoadLevel.Controls.Add(this.GrbSecondLoadLevel, 0, 1);
            this.TlpSecondLoadLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpSecondLoadLevel.Location = new System.Drawing.Point(3, 223);
            this.TlpSecondLoadLevel.Name = "TlpSecondLoadLevel";
            this.TlpSecondLoadLevel.RowCount = 2;
            this.TlpSecondLoadLevel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpSecondLoadLevel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpSecondLoadLevel.Size = new System.Drawing.Size(1027, 63);
            this.TlpSecondLoadLevel.TabIndex = 33;
            // 
            // LblSecondLoadLevel
            // 
            this.LblSecondLoadLevel.AutoSize = true;
            this.LblSecondLoadLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblSecondLoadLevel.Location = new System.Drawing.Point(41, 0);
            this.LblSecondLoadLevel.Name = "LblSecondLoadLevel";
            this.LblSecondLoadLevel.Size = new System.Drawing.Size(983, 38);
            this.LblSecondLoadLevel.TabIndex = 2;
            this.LblSecondLoadLevel.Text = "Second level image load";
            this.LblSecondLoadLevel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblSecondLoadLevel.Click += new System.EventHandler(this.SecondLoadLevelHeaderClicked);
            // 
            // PibSecondLoadLevel
            // 
            this.PibSecondLoadLevel.Image = global::VideoDedup.Properties.Resources.ArrowUpGray;
            this.PibSecondLoadLevel.Location = new System.Drawing.Point(3, 3);
            this.PibSecondLoadLevel.Name = "PibSecondLoadLevel";
            this.PibSecondLoadLevel.Size = new System.Drawing.Size(32, 32);
            this.PibSecondLoadLevel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PibSecondLoadLevel.TabIndex = 0;
            this.PibSecondLoadLevel.TabStop = false;
            this.PibSecondLoadLevel.Click += new System.EventHandler(this.SecondLoadLevelHeaderClicked);
            // 
            // GrbSecondLoadLevel
            // 
            this.GrbSecondLoadLevel.AutoSize = true;
            this.TlpSecondLoadLevel.SetColumnSpan(this.GrbSecondLoadLevel, 2);
            this.GrbSecondLoadLevel.Controls.Add(this.TlpSecondLoadLevelResult);
            this.GrbSecondLoadLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbSecondLoadLevel.Location = new System.Drawing.Point(3, 41);
            this.GrbSecondLoadLevel.Name = "GrbSecondLoadLevel";
            this.GrbSecondLoadLevel.Size = new System.Drawing.Size(1021, 19);
            this.GrbSecondLoadLevel.TabIndex = 1;
            this.GrbSecondLoadLevel.TabStop = false;
            // 
            // TlpSecondLoadLevelResult
            // 
            this.TlpSecondLoadLevelResult.AutoSize = true;
            this.TlpSecondLoadLevelResult.ColumnCount = 1;
            this.TlpSecondLoadLevelResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpSecondLoadLevelResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpSecondLoadLevelResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpSecondLoadLevelResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpSecondLoadLevelResult.Location = new System.Drawing.Point(3, 16);
            this.TlpSecondLoadLevelResult.Name = "TlpSecondLoadLevelResult";
            this.TlpSecondLoadLevelResult.RowCount = 1;
            this.TlpSecondLoadLevelResult.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpSecondLoadLevelResult.Size = new System.Drawing.Size(1015, 0);
            this.TlpSecondLoadLevelResult.TabIndex = 0;
            // 
            // TlpFirstLoadLevel
            // 
            this.TlpFirstLoadLevel.AutoSize = true;
            this.TlpFirstLoadLevel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TlpFirstLoadLevel.ColumnCount = 2;
            this.TlpResult.SetColumnSpan(this.TlpFirstLoadLevel, 2);
            this.TlpFirstLoadLevel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpFirstLoadLevel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpFirstLoadLevel.Controls.Add(this.PibFirstLoadLevel, 0, 0);
            this.TlpFirstLoadLevel.Controls.Add(this.LblFirstLoadLevel, 1, 0);
            this.TlpFirstLoadLevel.Controls.Add(this.GrbFirstLoadLevel, 0, 1);
            this.TlpFirstLoadLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpFirstLoadLevel.Location = new System.Drawing.Point(3, 154);
            this.TlpFirstLoadLevel.Name = "TlpFirstLoadLevel";
            this.TlpFirstLoadLevel.RowCount = 2;
            this.TlpFirstLoadLevel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpFirstLoadLevel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpFirstLoadLevel.Size = new System.Drawing.Size(1027, 63);
            this.TlpFirstLoadLevel.TabIndex = 33;
            // 
            // PibFirstLoadLevel
            // 
            this.PibFirstLoadLevel.Image = global::VideoDedup.Properties.Resources.ArrowUpGray;
            this.PibFirstLoadLevel.Location = new System.Drawing.Point(3, 3);
            this.PibFirstLoadLevel.Name = "PibFirstLoadLevel";
            this.PibFirstLoadLevel.Size = new System.Drawing.Size(32, 32);
            this.PibFirstLoadLevel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PibFirstLoadLevel.TabIndex = 0;
            this.PibFirstLoadLevel.TabStop = false;
            this.PibFirstLoadLevel.Click += new System.EventHandler(this.FirstLoadLevelHeaderClicked);
            // 
            // LblFirstLoadLevel
            // 
            this.LblFirstLoadLevel.AutoSize = true;
            this.LblFirstLoadLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblFirstLoadLevel.Location = new System.Drawing.Point(41, 0);
            this.LblFirstLoadLevel.Name = "LblFirstLoadLevel";
            this.LblFirstLoadLevel.Size = new System.Drawing.Size(983, 38);
            this.LblFirstLoadLevel.TabIndex = 1;
            this.LblFirstLoadLevel.Text = "First level image load";
            this.LblFirstLoadLevel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblFirstLoadLevel.Click += new System.EventHandler(this.FirstLoadLevelHeaderClicked);
            // 
            // GrbFirstLoadLevel
            // 
            this.GrbFirstLoadLevel.AutoSize = true;
            this.TlpFirstLoadLevel.SetColumnSpan(this.GrbFirstLoadLevel, 2);
            this.GrbFirstLoadLevel.Controls.Add(this.TlpFirstLoadLevelResult);
            this.GrbFirstLoadLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbFirstLoadLevel.Location = new System.Drawing.Point(3, 41);
            this.GrbFirstLoadLevel.Name = "GrbFirstLoadLevel";
            this.GrbFirstLoadLevel.Size = new System.Drawing.Size(1021, 19);
            this.GrbFirstLoadLevel.TabIndex = 0;
            this.GrbFirstLoadLevel.TabStop = false;
            // 
            // TlpFirstLoadLevelResult
            // 
            this.TlpFirstLoadLevelResult.AutoSize = true;
            this.TlpFirstLoadLevelResult.ColumnCount = 1;
            this.TlpFirstLoadLevelResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpFirstLoadLevelResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpFirstLoadLevelResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpFirstLoadLevelResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpFirstLoadLevelResult.Location = new System.Drawing.Point(3, 16);
            this.TlpFirstLoadLevelResult.Name = "TlpFirstLoadLevelResult";
            this.TlpFirstLoadLevelResult.RowCount = 1;
            this.TlpFirstLoadLevelResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpFirstLoadLevelResult.Size = new System.Drawing.Size(1015, 0);
            this.TlpFirstLoadLevelResult.TabIndex = 0;
            // 
            // GrbVideoTimeline
            // 
            this.GrbVideoTimeline.AutoSize = true;
            this.TlpResult.SetColumnSpan(this.GrbVideoTimeline, 2);
            this.GrbVideoTimeline.Controls.Add(this.TlpVideoTimeline);
            this.GrbVideoTimeline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbVideoTimeline.Location = new System.Drawing.Point(3, 361);
            this.GrbVideoTimeline.Name = "GrbVideoTimeline";
            this.GrbVideoTimeline.Size = new System.Drawing.Size(1027, 19);
            this.GrbVideoTimeline.TabIndex = 5;
            this.GrbVideoTimeline.TabStop = false;
            this.GrbVideoTimeline.Visible = false;
            // 
            // TlpVideoTimeline
            // 
            this.TlpVideoTimeline.AutoSize = true;
            this.TlpVideoTimeline.ColumnCount = 1;
            this.TlpVideoTimeline.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpVideoTimeline.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpVideoTimeline.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpVideoTimeline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpVideoTimeline.Location = new System.Drawing.Point(3, 16);
            this.TlpVideoTimeline.Name = "TlpVideoTimeline";
            this.TlpVideoTimeline.RowCount = 1;
            this.TlpVideoTimeline.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpVideoTimeline.Size = new System.Drawing.Size(1021, 0);
            this.TlpVideoTimeline.TabIndex = 0;
            // 
            // TlpProgress
            // 
            this.TlpProgress.AutoSize = true;
            this.TlpProgress.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TlpProgress.ColumnCount = 1;
            this.TlpResult.SetColumnSpan(this.TlpProgress, 2);
            this.TlpProgress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpProgress.Controls.Add(this.LblResult, 0, 0);
            this.TlpProgress.Controls.Add(this.PgbComparisonProgress, 0, 1);
            this.TlpProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpProgress.Location = new System.Drawing.Point(0, 0);
            this.TlpProgress.Margin = new System.Windows.Forms.Padding(0);
            this.TlpProgress.Name = "TlpProgress";
            this.TlpProgress.RowCount = 2;
            this.TlpProgress.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpProgress.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpProgress.Size = new System.Drawing.Size(1033, 74);
            this.TlpProgress.TabIndex = 4;
            // 
            // LblResult
            // 
            this.LblResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblResult.AutoSize = true;
            this.LblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblResult.Location = new System.Drawing.Point(3, 0);
            this.LblResult.Name = "LblResult";
            this.LblResult.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.LblResult.Size = new System.Drawing.Size(1027, 38);
            this.LblResult.TabIndex = 2;
            this.LblResult.Text = "Result text";
            this.LblResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PgbComparisonProgress
            // 
            this.PgbComparisonProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PgbComparisonProgress.Location = new System.Drawing.Point(3, 41);
            this.PgbComparisonProgress.MarqueeAnimationSpeed = 50;
            this.PgbComparisonProgress.Name = "PgbComparisonProgress";
            this.PgbComparisonProgress.Size = new System.Drawing.Size(1027, 30);
            this.PgbComparisonProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.PgbComparisonProgress.TabIndex = 3;
            // 
            // TxtLeftFileInfo
            // 
            this.TxtLeftFileInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtLeftFileInfo.Location = new System.Drawing.Point(3, 77);
            this.TxtLeftFileInfo.Multiline = true;
            this.TxtLeftFileInfo.Name = "TxtLeftFileInfo";
            this.TxtLeftFileInfo.ReadOnly = true;
            this.TxtLeftFileInfo.Size = new System.Drawing.Size(510, 71);
            this.TxtLeftFileInfo.TabIndex = 34;
            this.TxtLeftFileInfo.WordWrap = false;
            // 
            // TxtRightFileInfo
            // 
            this.TxtRightFileInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtRightFileInfo.Location = new System.Drawing.Point(519, 77);
            this.TxtRightFileInfo.Multiline = true;
            this.TxtRightFileInfo.Name = "TxtRightFileInfo";
            this.TxtRightFileInfo.ReadOnly = true;
            this.TxtRightFileInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtRightFileInfo.Size = new System.Drawing.Size(511, 71);
            this.TxtRightFileInfo.TabIndex = 34;
            // 
            // PnlResult
            // 
            this.PnlResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PnlResult.AutoScroll = true;
            this.PnlResult.Controls.Add(this.GrbResult);
            this.PnlResult.Location = new System.Drawing.Point(12, 170);
            this.PnlResult.Name = "PnlResult";
            this.PnlResult.Size = new System.Drawing.Size(1039, 638);
            this.PnlResult.TabIndex = 2;
            // 
            // GrbResult
            // 
            this.GrbResult.AutoSize = true;
            this.GrbResult.Controls.Add(this.TlpResult);
            this.GrbResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.GrbResult.Location = new System.Drawing.Point(0, 0);
            this.GrbResult.Name = "GrbResult";
            this.GrbResult.Size = new System.Drawing.Size(1039, 402);
            this.GrbResult.TabIndex = 1;
            this.GrbResult.TabStop = false;
            this.GrbResult.Text = "Result";
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(976, 814);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 1;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            this.BtnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOkay.Location = new System.Drawing.Point(895, 814);
            this.BtnOkay.Name = "BtnOkay";
            this.BtnOkay.Size = new System.Drawing.Size(75, 23);
            this.BtnOkay.TabIndex = 0;
            this.BtnOkay.Text = "&OK";
            this.BtnOkay.UseVisualStyleBackColor = true;
            this.BtnOkay.Click += new System.EventHandler(this.BtnOkay_Click);
            // 
            // TlpSettings
            // 
            this.TlpSettings.ColumnCount = 10;
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpSettings.Controls.Add(this.TrbMaxImageComparison, 2, 2);
            this.TlpSettings.Controls.Add(this.label1, 0, 0);
            this.TlpSettings.Controls.Add(this.BtnStartComparison, 8, 4);
            this.TlpSettings.Controls.Add(this.label2, 0, 1);
            this.TlpSettings.Controls.Add(this.TxtLeftFilePath, 1, 0);
            this.TlpSettings.Controls.Add(this.TxtRightFilePath, 1, 1);
            this.TlpSettings.Controls.Add(this.label3, 0, 2);
            this.TlpSettings.Controls.Add(this.label4, 0, 3);
            this.TlpSettings.Controls.Add(this.label5, 0, 4);
            this.TlpSettings.Controls.Add(this.BtnSelectLeftFilePath, 9, 0);
            this.TlpSettings.Controls.Add(this.BtnSelectRightFilePath, 9, 1);
            this.TlpSettings.Controls.Add(this.TrbMaxDifferentImages, 2, 3);
            this.TlpSettings.Controls.Add(this.TrbMaxDifferentPercentage, 2, 4);
            this.TlpSettings.Controls.Add(this.NumMaxImageComparison, 3, 2);
            this.TlpSettings.Controls.Add(this.NumMaxDifferentImages, 3, 3);
            this.TlpSettings.Controls.Add(this.NumMaxDifferentPercentage, 3, 4);
            this.TlpSettings.Controls.Add(this.label6, 5, 2);
            this.TlpSettings.Controls.Add(this.RdbSortByProcessingOrder, 6, 2);
            this.TlpSettings.Controls.Add(this.RdbSortByTimeline, 6, 3);
            this.TlpSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpSettings.Location = new System.Drawing.Point(3, 16);
            this.TlpSettings.Name = "TlpSettings";
            this.TlpSettings.RowCount = 5;
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpSettings.Size = new System.Drawing.Size(1033, 136);
            this.TlpSettings.TabIndex = 24;
            // 
            // TrbMaxImageComparison
            // 
            this.TrbMaxImageComparison.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrbMaxImageComparison.Location = new System.Drawing.Point(193, 57);
            this.TrbMaxImageComparison.Maximum = 100;
            this.TrbMaxImageComparison.Minimum = 1;
            this.TrbMaxImageComparison.Name = "TrbMaxImageComparison";
            this.TrbMaxImageComparison.Size = new System.Drawing.Size(144, 21);
            this.TrbMaxImageComparison.TabIndex = 32;
            this.TrbMaxImageComparison.TickFrequency = 2;
            this.TrbMaxImageComparison.Value = 1;
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
            this.BtnStartComparison.Location = new System.Drawing.Point(907, 111);
            this.BtnStartComparison.Name = "BtnStartComparison";
            this.BtnStartComparison.Size = new System.Drawing.Size(123, 22);
            this.BtnStartComparison.TabIndex = 7;
            this.BtnStartComparison.Text = "&Start comparison";
            this.BtnStartComparison.UseVisualStyleBackColor = true;
            this.BtnStartComparison.Click += new System.EventHandler(this.BtnStartComparison_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Right File:";
            // 
            // TxtLeftFilePath
            // 
            this.TxtLeftFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TlpSettings.SetColumnSpan(this.TxtLeftFilePath, 8);
            this.TxtLeftFilePath.Location = new System.Drawing.Point(63, 3);
            this.TxtLeftFilePath.Name = "TxtLeftFilePath";
            this.TxtLeftFilePath.Size = new System.Drawing.Size(937, 20);
            this.TxtLeftFilePath.TabIndex = 0;
            // 
            // TxtRightFilePath
            // 
            this.TxtRightFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TlpSettings.SetColumnSpan(this.TxtRightFilePath, 8);
            this.TxtRightFilePath.Location = new System.Drawing.Point(63, 30);
            this.TxtRightFilePath.Name = "TxtRightFilePath";
            this.TxtRightFilePath.Size = new System.Drawing.Size(937, 20);
            this.TxtRightFilePath.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.TlpSettings.SetColumnSpan(this.label3, 2);
            this.label3.Location = new System.Drawing.Point(35, 61);
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
            this.label4.Location = new System.Drawing.Point(3, 88);
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
            this.label5.Location = new System.Drawing.Point(12, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Accepted percentage of difference:";
            // 
            // BtnSelectLeftFilePath
            // 
            this.BtnSelectLeftFilePath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnSelectLeftFilePath.Location = new System.Drawing.Point(1006, 3);
            this.BtnSelectLeftFilePath.Name = "BtnSelectLeftFilePath";
            this.BtnSelectLeftFilePath.Size = new System.Drawing.Size(24, 21);
            this.BtnSelectLeftFilePath.TabIndex = 1;
            this.BtnSelectLeftFilePath.Text = "...";
            this.BtnSelectLeftFilePath.UseVisualStyleBackColor = true;
            this.BtnSelectLeftFilePath.Click += new System.EventHandler(this.BtnSelectLeftFilePath_Click);
            // 
            // BtnSelectRightFilePath
            // 
            this.BtnSelectRightFilePath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnSelectRightFilePath.Location = new System.Drawing.Point(1006, 30);
            this.BtnSelectRightFilePath.Name = "BtnSelectRightFilePath";
            this.BtnSelectRightFilePath.Size = new System.Drawing.Size(24, 21);
            this.BtnSelectRightFilePath.TabIndex = 3;
            this.BtnSelectRightFilePath.Text = "...";
            this.BtnSelectRightFilePath.UseVisualStyleBackColor = true;
            this.BtnSelectRightFilePath.Click += new System.EventHandler(this.BtnSelectRightFilePath_Click);
            // 
            // TrbMaxDifferentImages
            // 
            this.TrbMaxDifferentImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrbMaxDifferentImages.Location = new System.Drawing.Point(193, 84);
            this.TrbMaxDifferentImages.Maximum = 100;
            this.TrbMaxDifferentImages.Name = "TrbMaxDifferentImages";
            this.TrbMaxDifferentImages.Size = new System.Drawing.Size(144, 21);
            this.TrbMaxDifferentImages.TabIndex = 32;
            this.TrbMaxDifferentImages.TickFrequency = 2;
            // 
            // TrbMaxDifferentPercentage
            // 
            this.TrbMaxDifferentPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrbMaxDifferentPercentage.Location = new System.Drawing.Point(193, 111);
            this.TrbMaxDifferentPercentage.Maximum = 100;
            this.TrbMaxDifferentPercentage.Name = "TrbMaxDifferentPercentage";
            this.TrbMaxDifferentPercentage.Size = new System.Drawing.Size(144, 22);
            this.TrbMaxDifferentPercentage.TabIndex = 32;
            this.TrbMaxDifferentPercentage.TickFrequency = 2;
            this.TrbMaxDifferentPercentage.Value = 1;
            // 
            // NumMaxImageComparison
            // 
            this.NumMaxImageComparison.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.NumMaxImageComparison.Location = new System.Drawing.Point(343, 57);
            this.NumMaxImageComparison.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumMaxImageComparison.Name = "NumMaxImageComparison";
            this.NumMaxImageComparison.Size = new System.Drawing.Size(63, 20);
            this.NumMaxImageComparison.TabIndex = 4;
            this.NumMaxImageComparison.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NumMaxDifferentImages
            // 
            this.NumMaxDifferentImages.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.NumMaxDifferentImages.Location = new System.Drawing.Point(343, 84);
            this.NumMaxDifferentImages.Name = "NumMaxDifferentImages";
            this.NumMaxDifferentImages.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDifferentImages.TabIndex = 5;
            // 
            // NumMaxDifferentPercentage
            // 
            this.NumMaxDifferentPercentage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.NumMaxDifferentPercentage.Location = new System.Drawing.Point(343, 112);
            this.NumMaxDifferentPercentage.Name = "NumMaxDifferentPercentage";
            this.NumMaxDifferentPercentage.Size = new System.Drawing.Size(63, 20);
            this.NumMaxDifferentPercentage.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(562, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Sort images by:";
            // 
            // RdbSortByProcessingOrder
            // 
            this.RdbSortByProcessingOrder.AutoSize = true;
            this.RdbSortByProcessingOrder.Checked = true;
            this.RdbSortByProcessingOrder.Location = new System.Drawing.Point(647, 57);
            this.RdbSortByProcessingOrder.Name = "RdbSortByProcessingOrder";
            this.RdbSortByProcessingOrder.Size = new System.Drawing.Size(104, 17);
            this.RdbSortByProcessingOrder.TabIndex = 29;
            this.RdbSortByProcessingOrder.TabStop = true;
            this.RdbSortByProcessingOrder.Text = "Processing order";
            this.RdbSortByProcessingOrder.UseVisualStyleBackColor = true;
            this.RdbSortByProcessingOrder.CheckedChanged += new System.EventHandler(this.RdbSortByProcessingOrder_CheckedChanged);
            // 
            // RdbSortByTimeline
            // 
            this.RdbSortByTimeline.AutoSize = true;
            this.RdbSortByTimeline.Location = new System.Drawing.Point(647, 84);
            this.RdbSortByTimeline.Name = "RdbSortByTimeline";
            this.RdbSortByTimeline.Size = new System.Drawing.Size(64, 17);
            this.RdbSortByTimeline.TabIndex = 30;
            this.RdbSortByTimeline.Text = "Timeline";
            this.RdbSortByTimeline.UseVisualStyleBackColor = true;
            this.RdbSortByTimeline.CheckedChanged += new System.EventHandler(this.RdbSortByVideoTimeline_CheckedChanged);
            // 
            // StatusTimer
            // 
            this.StatusTimer.Tick += new System.EventHandler(this.HandleStatusTimerTick);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(976, 814);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 25;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            // 
            // GrbSettings
            // 
            this.GrbSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GrbSettings.Controls.Add(this.TlpSettings);
            this.GrbSettings.Location = new System.Drawing.Point(12, 12);
            this.GrbSettings.Name = "GrbSettings";
            this.GrbSettings.Size = new System.Drawing.Size(1039, 155);
            this.GrbSettings.TabIndex = 32;
            this.GrbSettings.TabStop = false;
            this.GrbSettings.Text = "Settings";
            // 
            // CustomVideoComparisonDlg
            // 
            this.AcceptButton = this.BtnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(1063, 849);
            this.Controls.Add(this.GrbSettings);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOkay);
            this.Controls.Add(this.PnlResult);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CustomVideoComparisonDlg";
            this.Text = "Custom Video Comparison";
            this.TlpResult.ResumeLayout(false);
            this.TlpResult.PerformLayout();
            this.TlpThirdLoadLevel.ResumeLayout(false);
            this.TlpThirdLoadLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.GrbThirdLoadLevel.ResumeLayout(false);
            this.GrbThirdLoadLevel.PerformLayout();
            this.TlpSecondLoadLevel.ResumeLayout(false);
            this.TlpSecondLoadLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PibSecondLoadLevel)).EndInit();
            this.GrbSecondLoadLevel.ResumeLayout(false);
            this.GrbSecondLoadLevel.PerformLayout();
            this.TlpFirstLoadLevel.ResumeLayout(false);
            this.TlpFirstLoadLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PibFirstLoadLevel)).EndInit();
            this.GrbFirstLoadLevel.ResumeLayout(false);
            this.GrbFirstLoadLevel.PerformLayout();
            this.GrbVideoTimeline.ResumeLayout(false);
            this.GrbVideoTimeline.PerformLayout();
            this.TlpProgress.ResumeLayout(false);
            this.TlpProgress.PerformLayout();
            this.PnlResult.ResumeLayout(false);
            this.PnlResult.PerformLayout();
            this.GrbResult.ResumeLayout(false);
            this.GrbResult.PerformLayout();
            this.TlpSettings.ResumeLayout(false);
            this.TlpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrbMaxImageComparison)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrbMaxDifferentImages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrbMaxDifferentPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxImageComparison)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentImages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumMaxDifferentPercentage)).EndInit();
            this.GrbSettings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpResult;
        private System.Windows.Forms.Panel PnlResult;
        private System.Windows.Forms.GroupBox GrbFirstLoadLevel;
        private System.Windows.Forms.TableLayoutPanel TlpFirstLoadLevelResult;
        private System.Windows.Forms.GroupBox GrbSecondLoadLevel;
        private System.Windows.Forms.TableLayoutPanel TlpSecondLoadLevelResult;
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
        private System.Windows.Forms.GroupBox GrbThirdLoadLevel;
        private System.Windows.Forms.TableLayoutPanel TlpThirdLoadLevelResult;
        private System.Windows.Forms.Timer StatusTimer;
        private System.Windows.Forms.Button BtnStartComparison;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TableLayoutPanel TlpProgress;
        private System.Windows.Forms.ProgressBar PgbComparisonProgress;
        private System.Windows.Forms.GroupBox GrbVideoTimeline;
        private System.Windows.Forms.TableLayoutPanel TlpVideoTimeline;
        private System.Windows.Forms.TrackBar TrbMaxImageComparison;
        private System.Windows.Forms.RadioButton RdbSortByProcessingOrder;
        private System.Windows.Forms.RadioButton RdbSortByTimeline;
        private System.Windows.Forms.TrackBar TrbMaxDifferentImages;
        private System.Windows.Forms.TrackBar TrbMaxDifferentPercentage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox GrbSettings;
        private System.Windows.Forms.TableLayoutPanel TlpFirstLoadLevel;
        private System.Windows.Forms.PictureBox PibFirstLoadLevel;
        private System.Windows.Forms.Label LblFirstLoadLevel;
        private System.Windows.Forms.TableLayoutPanel TlpSecondLoadLevel;
        private System.Windows.Forms.Label LblSecondLoadLevel;
        private System.Windows.Forms.PictureBox PibSecondLoadLevel;
        private System.Windows.Forms.TableLayoutPanel TlpThirdLoadLevel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label LblThirdLoadLevel;
        private System.Windows.Forms.TextBox TxtLeftFileInfo;
        private System.Windows.Forms.TextBox TxtRightFileInfo;
    }
}
