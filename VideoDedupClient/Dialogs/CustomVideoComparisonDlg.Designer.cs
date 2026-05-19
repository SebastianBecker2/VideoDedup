namespace VideoDedupClient.Dialogs
{
    using System.ComponentModel;
    using System.Windows.Forms;

    partial class CustomVideoComparisonDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            components = new Container();
            var resources = new ComponentResourceManager(typeof(CustomVideoComparisonDlg));
            TlpResult = new TableLayoutPanel();
            TlpThirdLoadLevel = new TableLayoutPanel();
            PibThirdLoadLevel = new PictureBox();
            LblThirdLoadLevel = new Label();
            GrbThirdLoadLevel = new GroupBox();
            TlpThirdLoadLevelResult = new TableLayoutPanel();
            TlpSecondLoadLevel = new TableLayoutPanel();
            LblSecondLoadLevel = new Label();
            PibSecondLoadLevel = new PictureBox();
            GrbSecondLoadLevel = new GroupBox();
            TlpSecondLoadLevelResult = new TableLayoutPanel();
            TlpFirstLoadLevel = new TableLayoutPanel();
            PibFirstLoadLevel = new PictureBox();
            LblFirstLoadLevel = new Label();
            GrbFirstLoadLevel = new GroupBox();
            TlpFirstLoadLevelResult = new TableLayoutPanel();
            GrbVideoTimeline = new GroupBox();
            TlpVideoTimeline = new TableLayoutPanel();
            TlpProgress = new TableLayoutPanel();
            LblResult = new Label();
            PgbComparisonProgress = new ProgressBar();
            TxtLeftFileInfo = new TextBox();
            TxtRightFileInfo = new TextBox();
            PnlResult = new Panel();
            GrbResult = new GroupBox();
            BtnCancel = new Button();
            BtnOkay = new Button();
            TlpSettings = new TableLayoutPanel();
            TrbMaxFrameComparison = new TrackBar();
            label1 = new Label();
            BtnStartComparison = new Button();
            label2 = new Label();
            TxtLeftFilePath = new TextBox();
            TxtRightFilePath = new TextBox();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            BtnSelectLeftFilePath = new Button();
            BtnSelectRightFilePath = new Button();
            TrbMaxDifferentFrames = new TrackBar();
            TrbMaxDifferentPercentage = new TrackBar();
            NumMaxFrameComparison = new NumericUpDown();
            NumMaxDifferentFrames = new NumericUpDown();
            NumMaxDifferentPercentage = new NumericUpDown();
            label6 = new Label();
            RdbSortByProcessingOrder = new RadioButton();
            RdbSortByTimeline = new RadioButton();
            StatusTimer = new Timer(components);
            btnClose = new Button();
            GrbSettings = new GroupBox();
            TlpResult.SuspendLayout();
            TlpThirdLoadLevel.SuspendLayout();
            ((ISupportInitialize)PibThirdLoadLevel).BeginInit();
            GrbThirdLoadLevel.SuspendLayout();
            TlpSecondLoadLevel.SuspendLayout();
            ((ISupportInitialize)PibSecondLoadLevel).BeginInit();
            GrbSecondLoadLevel.SuspendLayout();
            TlpFirstLoadLevel.SuspendLayout();
            ((ISupportInitialize)PibFirstLoadLevel).BeginInit();
            GrbFirstLoadLevel.SuspendLayout();
            GrbVideoTimeline.SuspendLayout();
            TlpProgress.SuspendLayout();
            PnlResult.SuspendLayout();
            GrbResult.SuspendLayout();
            TlpSettings.SuspendLayout();
            ((ISupportInitialize)TrbMaxFrameComparison).BeginInit();
            ((ISupportInitialize)TrbMaxDifferentFrames).BeginInit();
            ((ISupportInitialize)TrbMaxDifferentPercentage).BeginInit();
            ((ISupportInitialize)NumMaxFrameComparison).BeginInit();
            ((ISupportInitialize)NumMaxDifferentFrames).BeginInit();
            ((ISupportInitialize)NumMaxDifferentPercentage).BeginInit();
            GrbSettings.SuspendLayout();
            SuspendLayout();
            // 
            // TlpResult
            // 
            TlpResult.AutoSize = true;
            TlpResult.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TlpResult.ColumnCount = 2;
            TlpResult.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            TlpResult.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            TlpResult.Controls.Add(TlpThirdLoadLevel, 0, 4);
            TlpResult.Controls.Add(TlpSecondLoadLevel, 0, 3);
            TlpResult.Controls.Add(TlpFirstLoadLevel, 0, 2);
            TlpResult.Controls.Add(GrbVideoTimeline, 0, 5);
            TlpResult.Controls.Add(TlpProgress, 0, 0);
            TlpResult.Controls.Add(TxtLeftFileInfo, 0, 1);
            TlpResult.Controls.Add(TxtRightFileInfo, 1, 1);
            TlpResult.Dock = DockStyle.Fill;
            TlpResult.Location = new Point(4, 19);
            TlpResult.Margin = new Padding(4, 3, 4, 3);
            TlpResult.Name = "TlpResult";
            TlpResult.RowCount = 6;
            TlpResult.RowStyles.Add(new RowStyle());
            TlpResult.RowStyles.Add(new RowStyle());
            TlpResult.RowStyles.Add(new RowStyle());
            TlpResult.RowStyles.Add(new RowStyle());
            TlpResult.RowStyles.Add(new RowStyle());
            TlpResult.RowStyles.Add(new RowStyle());
            TlpResult.Size = new Size(1204, 414);
            TlpResult.TabIndex = 0;
            // 
            // TlpThirdLoadLevel
            // 
            TlpThirdLoadLevel.AutoSize = true;
            TlpThirdLoadLevel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TlpThirdLoadLevel.ColumnCount = 2;
            TlpResult.SetColumnSpan(TlpThirdLoadLevel, 2);
            TlpThirdLoadLevel.ColumnStyles.Add(new ColumnStyle());
            TlpThirdLoadLevel.ColumnStyles.Add(new ColumnStyle());
            TlpThirdLoadLevel.Controls.Add(PibThirdLoadLevel, 0, 0);
            TlpThirdLoadLevel.Controls.Add(LblThirdLoadLevel, 1, 0);
            TlpThirdLoadLevel.Controls.Add(GrbThirdLoadLevel, 0, 1);
            TlpThirdLoadLevel.Dock = DockStyle.Fill;
            TlpThirdLoadLevel.Location = new Point(4, 317);
            TlpThirdLoadLevel.Margin = new Padding(4, 3, 4, 3);
            TlpThirdLoadLevel.Name = "TlpThirdLoadLevel";
            TlpThirdLoadLevel.RowCount = 2;
            TlpThirdLoadLevel.RowStyles.Add(new RowStyle());
            TlpThirdLoadLevel.RowStyles.Add(new RowStyle());
            TlpThirdLoadLevel.Size = new Size(1196, 66);
            TlpThirdLoadLevel.TabIndex = 33;
            // 
            // PibThirdLoadLevel
            // 
            PibThirdLoadLevel.Image = Properties.Resources.ArrowUpGray;
            PibThirdLoadLevel.Location = new Point(4, 3);
            PibThirdLoadLevel.Margin = new Padding(4, 3, 4, 3);
            PibThirdLoadLevel.Name = "PibThirdLoadLevel";
            PibThirdLoadLevel.Size = new Size(32, 32);
            PibThirdLoadLevel.SizeMode = PictureBoxSizeMode.AutoSize;
            PibThirdLoadLevel.TabIndex = 0;
            PibThirdLoadLevel.TabStop = false;
            PibThirdLoadLevel.Click += ThirdLoadLevelHeaderClicked;
            // 
            // LblThirdLoadLevel
            // 
            LblThirdLoadLevel.AutoSize = true;
            LblThirdLoadLevel.Dock = DockStyle.Fill;
            LblThirdLoadLevel.Location = new Point(44, 0);
            LblThirdLoadLevel.Margin = new Padding(4, 0, 4, 0);
            LblThirdLoadLevel.Name = "LblThirdLoadLevel";
            LblThirdLoadLevel.Size = new Size(1148, 38);
            LblThirdLoadLevel.TabIndex = 1;
            LblThirdLoadLevel.Text = "Third level frame load";
            LblThirdLoadLevel.TextAlign = ContentAlignment.MiddleLeft;
            LblThirdLoadLevel.Click += ThirdLoadLevelHeaderClicked;
            // 
            // GrbThirdLoadLevel
            // 
            GrbThirdLoadLevel.AutoSize = true;
            TlpThirdLoadLevel.SetColumnSpan(GrbThirdLoadLevel, 2);
            GrbThirdLoadLevel.Controls.Add(TlpThirdLoadLevelResult);
            GrbThirdLoadLevel.Dock = DockStyle.Fill;
            GrbThirdLoadLevel.Location = new Point(4, 41);
            GrbThirdLoadLevel.Margin = new Padding(4, 3, 4, 3);
            GrbThirdLoadLevel.Name = "GrbThirdLoadLevel";
            GrbThirdLoadLevel.Padding = new Padding(4, 3, 4, 3);
            GrbThirdLoadLevel.Size = new Size(1188, 22);
            GrbThirdLoadLevel.TabIndex = 3;
            GrbThirdLoadLevel.TabStop = false;
            // 
            // TlpThirdLoadLevelResult
            // 
            TlpThirdLoadLevelResult.AutoSize = true;
            TlpThirdLoadLevelResult.ColumnCount = 1;
            TlpThirdLoadLevelResult.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TlpThirdLoadLevelResult.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23F));
            TlpThirdLoadLevelResult.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23F));
            TlpThirdLoadLevelResult.Dock = DockStyle.Fill;
            TlpThirdLoadLevelResult.Location = new Point(4, 19);
            TlpThirdLoadLevelResult.Margin = new Padding(4, 3, 4, 3);
            TlpThirdLoadLevelResult.Name = "TlpThirdLoadLevelResult";
            TlpThirdLoadLevelResult.RowCount = 1;
            TlpThirdLoadLevelResult.RowStyles.Add(new RowStyle());
            TlpThirdLoadLevelResult.Size = new Size(1180, 0);
            TlpThirdLoadLevelResult.TabIndex = 0;
            // 
            // TlpSecondLoadLevel
            // 
            TlpSecondLoadLevel.AutoSize = true;
            TlpSecondLoadLevel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TlpSecondLoadLevel.ColumnCount = 2;
            TlpResult.SetColumnSpan(TlpSecondLoadLevel, 2);
            TlpSecondLoadLevel.ColumnStyles.Add(new ColumnStyle());
            TlpSecondLoadLevel.ColumnStyles.Add(new ColumnStyle());
            TlpSecondLoadLevel.Controls.Add(LblSecondLoadLevel, 1, 0);
            TlpSecondLoadLevel.Controls.Add(PibSecondLoadLevel, 0, 0);
            TlpSecondLoadLevel.Controls.Add(GrbSecondLoadLevel, 0, 1);
            TlpSecondLoadLevel.Dock = DockStyle.Fill;
            TlpSecondLoadLevel.Location = new Point(4, 245);
            TlpSecondLoadLevel.Margin = new Padding(4, 3, 4, 3);
            TlpSecondLoadLevel.Name = "TlpSecondLoadLevel";
            TlpSecondLoadLevel.RowCount = 2;
            TlpSecondLoadLevel.RowStyles.Add(new RowStyle());
            TlpSecondLoadLevel.RowStyles.Add(new RowStyle());
            TlpSecondLoadLevel.Size = new Size(1196, 66);
            TlpSecondLoadLevel.TabIndex = 33;
            // 
            // LblSecondLoadLevel
            // 
            LblSecondLoadLevel.AutoSize = true;
            LblSecondLoadLevel.Dock = DockStyle.Fill;
            LblSecondLoadLevel.Location = new Point(44, 0);
            LblSecondLoadLevel.Margin = new Padding(4, 0, 4, 0);
            LblSecondLoadLevel.Name = "LblSecondLoadLevel";
            LblSecondLoadLevel.Size = new Size(1148, 38);
            LblSecondLoadLevel.TabIndex = 2;
            LblSecondLoadLevel.Text = "Second level frame load";
            LblSecondLoadLevel.TextAlign = ContentAlignment.MiddleLeft;
            LblSecondLoadLevel.Click += SecondLoadLevelHeaderClicked;
            // 
            // PibSecondLoadLevel
            // 
            PibSecondLoadLevel.Image = Properties.Resources.ArrowUpGray;
            PibSecondLoadLevel.Location = new Point(4, 3);
            PibSecondLoadLevel.Margin = new Padding(4, 3, 4, 3);
            PibSecondLoadLevel.Name = "PibSecondLoadLevel";
            PibSecondLoadLevel.Size = new Size(32, 32);
            PibSecondLoadLevel.SizeMode = PictureBoxSizeMode.AutoSize;
            PibSecondLoadLevel.TabIndex = 0;
            PibSecondLoadLevel.TabStop = false;
            PibSecondLoadLevel.Click += SecondLoadLevelHeaderClicked;
            // 
            // GrbSecondLoadLevel
            // 
            GrbSecondLoadLevel.AutoSize = true;
            TlpSecondLoadLevel.SetColumnSpan(GrbSecondLoadLevel, 2);
            GrbSecondLoadLevel.Controls.Add(TlpSecondLoadLevelResult);
            GrbSecondLoadLevel.Dock = DockStyle.Fill;
            GrbSecondLoadLevel.Location = new Point(4, 41);
            GrbSecondLoadLevel.Margin = new Padding(4, 3, 4, 3);
            GrbSecondLoadLevel.Name = "GrbSecondLoadLevel";
            GrbSecondLoadLevel.Padding = new Padding(4, 3, 4, 3);
            GrbSecondLoadLevel.Size = new Size(1188, 22);
            GrbSecondLoadLevel.TabIndex = 1;
            GrbSecondLoadLevel.TabStop = false;
            // 
            // TlpSecondLoadLevelResult
            // 
            TlpSecondLoadLevelResult.AutoSize = true;
            TlpSecondLoadLevelResult.ColumnCount = 1;
            TlpSecondLoadLevelResult.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TlpSecondLoadLevelResult.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23F));
            TlpSecondLoadLevelResult.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23F));
            TlpSecondLoadLevelResult.Dock = DockStyle.Fill;
            TlpSecondLoadLevelResult.Location = new Point(4, 19);
            TlpSecondLoadLevelResult.Margin = new Padding(4, 3, 4, 3);
            TlpSecondLoadLevelResult.Name = "TlpSecondLoadLevelResult";
            TlpSecondLoadLevelResult.RowCount = 1;
            TlpSecondLoadLevelResult.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TlpSecondLoadLevelResult.Size = new Size(1180, 0);
            TlpSecondLoadLevelResult.TabIndex = 0;
            // 
            // TlpFirstLoadLevel
            // 
            TlpFirstLoadLevel.AutoSize = true;
            TlpFirstLoadLevel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TlpFirstLoadLevel.ColumnCount = 2;
            TlpResult.SetColumnSpan(TlpFirstLoadLevel, 2);
            TlpFirstLoadLevel.ColumnStyles.Add(new ColumnStyle());
            TlpFirstLoadLevel.ColumnStyles.Add(new ColumnStyle());
            TlpFirstLoadLevel.Controls.Add(PibFirstLoadLevel, 0, 0);
            TlpFirstLoadLevel.Controls.Add(LblFirstLoadLevel, 1, 0);
            TlpFirstLoadLevel.Controls.Add(GrbFirstLoadLevel, 0, 1);
            TlpFirstLoadLevel.Dock = DockStyle.Fill;
            TlpFirstLoadLevel.Location = new Point(4, 173);
            TlpFirstLoadLevel.Margin = new Padding(4, 3, 4, 3);
            TlpFirstLoadLevel.Name = "TlpFirstLoadLevel";
            TlpFirstLoadLevel.RowCount = 2;
            TlpFirstLoadLevel.RowStyles.Add(new RowStyle());
            TlpFirstLoadLevel.RowStyles.Add(new RowStyle());
            TlpFirstLoadLevel.Size = new Size(1196, 66);
            TlpFirstLoadLevel.TabIndex = 33;
            // 
            // PibFirstLoadLevel
            // 
            PibFirstLoadLevel.Image = Properties.Resources.ArrowUpGray;
            PibFirstLoadLevel.Location = new Point(4, 3);
            PibFirstLoadLevel.Margin = new Padding(4, 3, 4, 3);
            PibFirstLoadLevel.Name = "PibFirstLoadLevel";
            PibFirstLoadLevel.Size = new Size(32, 32);
            PibFirstLoadLevel.SizeMode = PictureBoxSizeMode.AutoSize;
            PibFirstLoadLevel.TabIndex = 0;
            PibFirstLoadLevel.TabStop = false;
            PibFirstLoadLevel.Click += FirstLoadLevelHeaderClicked;
            // 
            // LblFirstLoadLevel
            // 
            LblFirstLoadLevel.AutoSize = true;
            LblFirstLoadLevel.Dock = DockStyle.Fill;
            LblFirstLoadLevel.Location = new Point(44, 0);
            LblFirstLoadLevel.Margin = new Padding(4, 0, 4, 0);
            LblFirstLoadLevel.Name = "LblFirstLoadLevel";
            LblFirstLoadLevel.Size = new Size(1148, 38);
            LblFirstLoadLevel.TabIndex = 1;
            LblFirstLoadLevel.Text = "First level frame load";
            LblFirstLoadLevel.TextAlign = ContentAlignment.MiddleLeft;
            LblFirstLoadLevel.Click += FirstLoadLevelHeaderClicked;
            // 
            // GrbFirstLoadLevel
            // 
            GrbFirstLoadLevel.AutoSize = true;
            TlpFirstLoadLevel.SetColumnSpan(GrbFirstLoadLevel, 2);
            GrbFirstLoadLevel.Controls.Add(TlpFirstLoadLevelResult);
            GrbFirstLoadLevel.Dock = DockStyle.Fill;
            GrbFirstLoadLevel.Location = new Point(4, 41);
            GrbFirstLoadLevel.Margin = new Padding(4, 3, 4, 3);
            GrbFirstLoadLevel.Name = "GrbFirstLoadLevel";
            GrbFirstLoadLevel.Padding = new Padding(4, 3, 4, 3);
            GrbFirstLoadLevel.Size = new Size(1188, 22);
            GrbFirstLoadLevel.TabIndex = 0;
            GrbFirstLoadLevel.TabStop = false;
            // 
            // TlpFirstLoadLevelResult
            // 
            TlpFirstLoadLevelResult.AutoSize = true;
            TlpFirstLoadLevelResult.ColumnCount = 1;
            TlpFirstLoadLevelResult.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TlpFirstLoadLevelResult.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23F));
            TlpFirstLoadLevelResult.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23F));
            TlpFirstLoadLevelResult.Dock = DockStyle.Fill;
            TlpFirstLoadLevelResult.Location = new Point(4, 19);
            TlpFirstLoadLevelResult.Margin = new Padding(4, 3, 4, 3);
            TlpFirstLoadLevelResult.Name = "TlpFirstLoadLevelResult";
            TlpFirstLoadLevelResult.RowCount = 1;
            TlpFirstLoadLevelResult.RowStyles.Add(new RowStyle());
            TlpFirstLoadLevelResult.Size = new Size(1180, 0);
            TlpFirstLoadLevelResult.TabIndex = 0;
            // 
            // GrbVideoTimeline
            // 
            GrbVideoTimeline.AutoSize = true;
            TlpResult.SetColumnSpan(GrbVideoTimeline, 2);
            GrbVideoTimeline.Controls.Add(TlpVideoTimeline);
            GrbVideoTimeline.Dock = DockStyle.Fill;
            GrbVideoTimeline.Location = new Point(4, 389);
            GrbVideoTimeline.Margin = new Padding(4, 3, 4, 3);
            GrbVideoTimeline.Name = "GrbVideoTimeline";
            GrbVideoTimeline.Padding = new Padding(4, 3, 4, 3);
            GrbVideoTimeline.Size = new Size(1196, 22);
            GrbVideoTimeline.TabIndex = 5;
            GrbVideoTimeline.TabStop = false;
            GrbVideoTimeline.Visible = false;
            // 
            // TlpVideoTimeline
            // 
            TlpVideoTimeline.AutoSize = true;
            TlpVideoTimeline.ColumnCount = 1;
            TlpVideoTimeline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TlpVideoTimeline.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23F));
            TlpVideoTimeline.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23F));
            TlpVideoTimeline.Dock = DockStyle.Fill;
            TlpVideoTimeline.Location = new Point(4, 19);
            TlpVideoTimeline.Margin = new Padding(4, 3, 4, 3);
            TlpVideoTimeline.Name = "TlpVideoTimeline";
            TlpVideoTimeline.RowCount = 1;
            TlpVideoTimeline.RowStyles.Add(new RowStyle());
            TlpVideoTimeline.Size = new Size(1188, 0);
            TlpVideoTimeline.TabIndex = 0;
            // 
            // TlpProgress
            // 
            TlpProgress.AutoSize = true;
            TlpProgress.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TlpProgress.ColumnCount = 1;
            TlpResult.SetColumnSpan(TlpProgress, 2);
            TlpProgress.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TlpProgress.Controls.Add(LblResult, 0, 0);
            TlpProgress.Controls.Add(PgbComparisonProgress, 0, 1);
            TlpProgress.Dock = DockStyle.Fill;
            TlpProgress.Location = new Point(0, 0);
            TlpProgress.Margin = new Padding(0);
            TlpProgress.Name = "TlpProgress";
            TlpProgress.RowCount = 2;
            TlpProgress.RowStyles.Add(new RowStyle());
            TlpProgress.RowStyles.Add(new RowStyle());
            TlpProgress.Size = new Size(1204, 83);
            TlpProgress.TabIndex = 4;
            // 
            // LblResult
            // 
            LblResult.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LblResult.AutoSize = true;
            LblResult.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LblResult.Location = new Point(4, 0);
            LblResult.Margin = new Padding(4, 0, 4, 0);
            LblResult.Name = "LblResult";
            LblResult.Padding = new Padding(0, 12, 0, 12);
            LblResult.Size = new Size(1196, 42);
            LblResult.TabIndex = 2;
            LblResult.Text = "Result text";
            LblResult.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PgbComparisonProgress
            // 
            PgbComparisonProgress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PgbComparisonProgress.Location = new Point(4, 45);
            PgbComparisonProgress.Margin = new Padding(4, 3, 4, 3);
            PgbComparisonProgress.MarqueeAnimationSpeed = 50;
            PgbComparisonProgress.Name = "PgbComparisonProgress";
            PgbComparisonProgress.Size = new Size(1196, 35);
            PgbComparisonProgress.Style = ProgressBarStyle.Marquee;
            PgbComparisonProgress.TabIndex = 3;
            // 
            // TxtLeftFileInfo
            // 
            TxtLeftFileInfo.Dock = DockStyle.Fill;
            TxtLeftFileInfo.Location = new Point(4, 86);
            TxtLeftFileInfo.Margin = new Padding(4, 3, 4, 3);
            TxtLeftFileInfo.Multiline = true;
            TxtLeftFileInfo.Name = "TxtLeftFileInfo";
            TxtLeftFileInfo.ReadOnly = true;
            TxtLeftFileInfo.Size = new Size(594, 81);
            TxtLeftFileInfo.TabIndex = 34;
            TxtLeftFileInfo.WordWrap = false;
            // 
            // TxtRightFileInfo
            // 
            TxtRightFileInfo.Dock = DockStyle.Fill;
            TxtRightFileInfo.Location = new Point(606, 86);
            TxtRightFileInfo.Margin = new Padding(4, 3, 4, 3);
            TxtRightFileInfo.Multiline = true;
            TxtRightFileInfo.Name = "TxtRightFileInfo";
            TxtRightFileInfo.ReadOnly = true;
            TxtRightFileInfo.ScrollBars = ScrollBars.Both;
            TxtRightFileInfo.Size = new Size(594, 81);
            TxtRightFileInfo.TabIndex = 34;
            // 
            // PnlResult
            // 
            PnlResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PnlResult.AutoScroll = true;
            PnlResult.Controls.Add(GrbResult);
            PnlResult.Location = new Point(14, 196);
            PnlResult.Margin = new Padding(4, 3, 4, 3);
            PnlResult.Name = "PnlResult";
            PnlResult.Size = new Size(1212, 736);
            PnlResult.TabIndex = 2;
            // 
            // GrbResult
            // 
            GrbResult.AutoSize = true;
            GrbResult.Controls.Add(TlpResult);
            GrbResult.Dock = DockStyle.Top;
            GrbResult.Location = new Point(0, 0);
            GrbResult.Margin = new Padding(4, 3, 4, 3);
            GrbResult.Name = "GrbResult";
            GrbResult.Padding = new Padding(4, 3, 4, 3);
            GrbResult.Size = new Size(1212, 436);
            GrbResult.TabIndex = 1;
            GrbResult.TabStop = false;
            GrbResult.Text = "Result";
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.Location = new Point(1139, 939);
            BtnCancel.Margin = new Padding(4, 3, 4, 3);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(88, 27);
            BtnCancel.TabIndex = 1;
            BtnCancel.Text = "&Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            BtnOkay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnOkay.Location = new Point(1044, 939);
            BtnOkay.Margin = new Padding(4, 3, 4, 3);
            BtnOkay.Name = "BtnOkay";
            BtnOkay.Size = new Size(88, 27);
            BtnOkay.TabIndex = 0;
            BtnOkay.Text = "&OK";
            BtnOkay.UseVisualStyleBackColor = true;
            BtnOkay.Click += BtnOkay_Click;
            // 
            // TlpSettings
            // 
            TlpSettings.ColumnCount = 10;
            TlpSettings.ColumnStyles.Add(new ColumnStyle());
            TlpSettings.ColumnStyles.Add(new ColumnStyle());
            TlpSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            TlpSettings.ColumnStyles.Add(new ColumnStyle());
            TlpSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            TlpSettings.ColumnStyles.Add(new ColumnStyle());
            TlpSettings.ColumnStyles.Add(new ColumnStyle());
            TlpSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            TlpSettings.ColumnStyles.Add(new ColumnStyle());
            TlpSettings.ColumnStyles.Add(new ColumnStyle());
            TlpSettings.Controls.Add(TrbMaxFrameComparison, 2, 2);
            TlpSettings.Controls.Add(label1, 0, 0);
            TlpSettings.Controls.Add(BtnStartComparison, 8, 4);
            TlpSettings.Controls.Add(label2, 0, 1);
            TlpSettings.Controls.Add(TxtLeftFilePath, 1, 0);
            TlpSettings.Controls.Add(TxtRightFilePath, 1, 1);
            TlpSettings.Controls.Add(label3, 0, 2);
            TlpSettings.Controls.Add(label4, 0, 3);
            TlpSettings.Controls.Add(label5, 0, 4);
            TlpSettings.Controls.Add(BtnSelectLeftFilePath, 9, 0);
            TlpSettings.Controls.Add(BtnSelectRightFilePath, 9, 1);
            TlpSettings.Controls.Add(TrbMaxDifferentFrames, 2, 3);
            TlpSettings.Controls.Add(TrbMaxDifferentPercentage, 2, 4);
            TlpSettings.Controls.Add(NumMaxFrameComparison, 3, 2);
            TlpSettings.Controls.Add(NumMaxDifferentFrames, 3, 3);
            TlpSettings.Controls.Add(NumMaxDifferentPercentage, 3, 4);
            TlpSettings.Controls.Add(label6, 5, 2);
            TlpSettings.Controls.Add(RdbSortByProcessingOrder, 6, 2);
            TlpSettings.Controls.Add(RdbSortByTimeline, 6, 3);
            TlpSettings.Dock = DockStyle.Fill;
            TlpSettings.Location = new Point(4, 19);
            TlpSettings.Margin = new Padding(4, 3, 4, 3);
            TlpSettings.Name = "TlpSettings";
            TlpSettings.RowCount = 5;
            TlpSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            TlpSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            TlpSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            TlpSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            TlpSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            TlpSettings.Size = new Size(1204, 157);
            TlpSettings.TabIndex = 24;
            // 
            // TrbMaxFrameComparison
            // 
            TrbMaxFrameComparison.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TrbMaxFrameComparison.Location = new Point(220, 65);
            TrbMaxFrameComparison.Margin = new Padding(4, 3, 4, 3);
            TrbMaxFrameComparison.Maximum = 100;
            TrbMaxFrameComparison.Minimum = 1;
            TrbMaxFrameComparison.Name = "TrbMaxFrameComparison";
            TrbMaxFrameComparison.Size = new Size(171, 25);
            TrbMaxFrameComparison.TabIndex = 32;
            TrbMaxFrameComparison.TickFrequency = 2;
            TrbMaxFrameComparison.Value = 1;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(12, 8);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(51, 15);
            label1.TabIndex = 0;
            label1.Text = "Left File:";
            // 
            // BtnStartComparison
            // 
            BtnStartComparison.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            TlpSettings.SetColumnSpan(BtnStartComparison, 2);
            BtnStartComparison.Location = new Point(1056, 129);
            BtnStartComparison.Margin = new Padding(4, 3, 4, 3);
            BtnStartComparison.Name = "BtnStartComparison";
            BtnStartComparison.Size = new Size(144, 25);
            BtnStartComparison.TabIndex = 7;
            BtnStartComparison.Text = "&Start comparison";
            BtnStartComparison.UseVisualStyleBackColor = true;
            BtnStartComparison.Click += BtnStartComparison_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(4, 39);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(59, 15);
            label2.TabIndex = 1;
            label2.Text = "Right File:";
            // 
            // TxtLeftFilePath
            // 
            TxtLeftFilePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TlpSettings.SetColumnSpan(TxtLeftFilePath, 8);
            TxtLeftFilePath.Location = new Point(71, 4);
            TxtLeftFilePath.Margin = new Padding(4, 3, 4, 3);
            TxtLeftFilePath.Name = "TxtLeftFilePath";
            TxtLeftFilePath.Size = new Size(1091, 23);
            TxtLeftFilePath.TabIndex = 0;
            // 
            // TxtRightFilePath
            // 
            TxtRightFilePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TlpSettings.SetColumnSpan(TxtRightFilePath, 8);
            TxtRightFilePath.Location = new Point(71, 35);
            TxtRightFilePath.Margin = new Padding(4, 3, 4, 3);
            TxtRightFilePath.Name = "TxtRightFilePath";
            TxtRightFilePath.Size = new Size(1091, 23);
            TxtRightFilePath.TabIndex = 2;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Right;
            label3.AutoSize = true;
            TlpSettings.SetColumnSpan(label3, 2);
            label3.Location = new Point(39, 70);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(173, 15);
            label3.TabIndex = 21;
            label3.Text = "Number of Frames to compare:";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Right;
            label4.AutoSize = true;
            TlpSettings.SetColumnSpan(label4, 2);
            label4.Location = new Point(4, 101);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(208, 15);
            label4.TabIndex = 22;
            label4.Text = "Accepted number of different Frames:";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Right;
            label5.AutoSize = true;
            TlpSettings.SetColumnSpan(label5, 2);
            label5.Location = new Point(20, 133);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(192, 15);
            label5.TabIndex = 22;
            label5.Text = "Accepted percentage of difference:";
            // 
            // BtnSelectLeftFilePath
            // 
            BtnSelectLeftFilePath.Anchor = AnchorStyles.Right;
            BtnSelectLeftFilePath.Location = new Point(1172, 3);
            BtnSelectLeftFilePath.Margin = new Padding(4, 3, 4, 3);
            BtnSelectLeftFilePath.Name = "BtnSelectLeftFilePath";
            BtnSelectLeftFilePath.Size = new Size(28, 24);
            BtnSelectLeftFilePath.TabIndex = 1;
            BtnSelectLeftFilePath.Text = "...";
            BtnSelectLeftFilePath.UseVisualStyleBackColor = true;
            BtnSelectLeftFilePath.Click += BtnSelectLeftFilePath_Click;
            // 
            // BtnSelectRightFilePath
            // 
            BtnSelectRightFilePath.Anchor = AnchorStyles.Right;
            BtnSelectRightFilePath.Location = new Point(1172, 34);
            BtnSelectRightFilePath.Margin = new Padding(4, 3, 4, 3);
            BtnSelectRightFilePath.Name = "BtnSelectRightFilePath";
            BtnSelectRightFilePath.Size = new Size(28, 24);
            BtnSelectRightFilePath.TabIndex = 3;
            BtnSelectRightFilePath.Text = "...";
            BtnSelectRightFilePath.UseVisualStyleBackColor = true;
            BtnSelectRightFilePath.Click += BtnSelectRightFilePath_Click;
            // 
            // TrbMaxDifferentFrames
            // 
            TrbMaxDifferentFrames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TrbMaxDifferentFrames.Location = new Point(220, 96);
            TrbMaxDifferentFrames.Margin = new Padding(4, 3, 4, 3);
            TrbMaxDifferentFrames.Maximum = 100;
            TrbMaxDifferentFrames.Name = "TrbMaxDifferentFrames";
            TrbMaxDifferentFrames.Size = new Size(171, 25);
            TrbMaxDifferentFrames.TabIndex = 32;
            TrbMaxDifferentFrames.TickFrequency = 2;
            // 
            // TrbMaxDifferentPercentage
            // 
            TrbMaxDifferentPercentage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TrbMaxDifferentPercentage.Location = new Point(220, 127);
            TrbMaxDifferentPercentage.Margin = new Padding(4, 3, 4, 3);
            TrbMaxDifferentPercentage.Maximum = 200;
            TrbMaxDifferentPercentage.Name = "TrbMaxDifferentPercentage";
            TrbMaxDifferentPercentage.Size = new Size(171, 27);
            TrbMaxDifferentPercentage.TabIndex = 32;
            TrbMaxDifferentPercentage.TickFrequency = 2;
            TrbMaxDifferentPercentage.Value = 1;
            // 
            // NumMaxFrameComparison
            // 
            NumMaxFrameComparison.Anchor = AnchorStyles.Left;
            NumMaxFrameComparison.Location = new Point(399, 66);
            NumMaxFrameComparison.Margin = new Padding(4, 3, 4, 3);
            NumMaxFrameComparison.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumMaxFrameComparison.Name = "NumMaxFrameComparison";
            NumMaxFrameComparison.Size = new Size(74, 23);
            NumMaxFrameComparison.TabIndex = 4;
            NumMaxFrameComparison.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // NumMaxDifferentFrames
            // 
            NumMaxDifferentFrames.Anchor = AnchorStyles.Left;
            NumMaxDifferentFrames.Location = new Point(399, 97);
            NumMaxDifferentFrames.Margin = new Padding(4, 3, 4, 3);
            NumMaxDifferentFrames.Name = "NumMaxDifferentFrames";
            NumMaxDifferentFrames.Size = new Size(74, 23);
            NumMaxDifferentFrames.TabIndex = 5;
            // 
            // NumMaxDifferentPercentage
            // 
            NumMaxDifferentPercentage.Anchor = AnchorStyles.Left;
            NumMaxDifferentPercentage.Location = new Point(399, 129);
            NumMaxDifferentPercentage.Margin = new Padding(4, 3, 4, 3);
            NumMaxDifferentPercentage.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            NumMaxDifferentPercentage.Name = "NumMaxDifferentPercentage";
            NumMaxDifferentPercentage.Size = new Size(74, 23);
            NumMaxDifferentPercentage.TabIndex = 6;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(660, 70);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(86, 15);
            label6.TabIndex = 21;
            label6.Text = "Sort frames by:";
            // 
            // RdbSortByProcessingOrder
            // 
            RdbSortByProcessingOrder.AutoSize = true;
            RdbSortByProcessingOrder.Checked = true;
            RdbSortByProcessingOrder.Location = new Point(754, 65);
            RdbSortByProcessingOrder.Margin = new Padding(4, 3, 4, 3);
            RdbSortByProcessingOrder.Name = "RdbSortByProcessingOrder";
            RdbSortByProcessingOrder.Size = new Size(113, 19);
            RdbSortByProcessingOrder.TabIndex = 29;
            RdbSortByProcessingOrder.TabStop = true;
            RdbSortByProcessingOrder.Text = "Processing order";
            RdbSortByProcessingOrder.UseVisualStyleBackColor = true;
            RdbSortByProcessingOrder.CheckedChanged += RdbSortByProcessingOrder_CheckedChanged;
            // 
            // RdbSortByTimeline
            // 
            RdbSortByTimeline.AutoSize = true;
            RdbSortByTimeline.Location = new Point(754, 96);
            RdbSortByTimeline.Margin = new Padding(4, 3, 4, 3);
            RdbSortByTimeline.Name = "RdbSortByTimeline";
            RdbSortByTimeline.Size = new Size(71, 19);
            RdbSortByTimeline.TabIndex = 30;
            RdbSortByTimeline.Text = "Timeline";
            RdbSortByTimeline.UseVisualStyleBackColor = true;
            RdbSortByTimeline.CheckedChanged += RdbSortByVideoTimeline_CheckedChanged;
            // 
            // StatusTimer
            // 
            StatusTimer.Tick += HandleStatusTimerTick;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.OK;
            btnClose.Location = new Point(1139, 939);
            btnClose.Margin = new Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 27);
            btnClose.TabIndex = 25;
            btnClose.Text = "&Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Visible = false;
            // 
            // GrbSettings
            // 
            GrbSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GrbSettings.Controls.Add(TlpSettings);
            GrbSettings.Location = new Point(14, 14);
            GrbSettings.Margin = new Padding(4, 3, 4, 3);
            GrbSettings.Name = "GrbSettings";
            GrbSettings.Padding = new Padding(4, 3, 4, 3);
            GrbSettings.Size = new Size(1212, 179);
            GrbSettings.TabIndex = 32;
            GrbSettings.TabStop = false;
            GrbSettings.Text = "Settings";
            // 
            // CustomVideoComparisonDlg
            // 
            AcceptButton = BtnOkay;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = BtnCancel;
            ClientSize = new Size(1240, 980);
            Controls.Add(GrbSettings);
            Controls.Add(btnClose);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOkay);
            Controls.Add(PnlResult);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "CustomVideoComparisonDlg";
            Text = "Custom Video Comparison";
            TlpResult.ResumeLayout(false);
            TlpResult.PerformLayout();
            TlpThirdLoadLevel.ResumeLayout(false);
            TlpThirdLoadLevel.PerformLayout();
            ((ISupportInitialize)PibThirdLoadLevel).EndInit();
            GrbThirdLoadLevel.ResumeLayout(false);
            GrbThirdLoadLevel.PerformLayout();
            TlpSecondLoadLevel.ResumeLayout(false);
            TlpSecondLoadLevel.PerformLayout();
            ((ISupportInitialize)PibSecondLoadLevel).EndInit();
            GrbSecondLoadLevel.ResumeLayout(false);
            GrbSecondLoadLevel.PerformLayout();
            TlpFirstLoadLevel.ResumeLayout(false);
            TlpFirstLoadLevel.PerformLayout();
            ((ISupportInitialize)PibFirstLoadLevel).EndInit();
            GrbFirstLoadLevel.ResumeLayout(false);
            GrbFirstLoadLevel.PerformLayout();
            GrbVideoTimeline.ResumeLayout(false);
            GrbVideoTimeline.PerformLayout();
            TlpProgress.ResumeLayout(false);
            TlpProgress.PerformLayout();
            PnlResult.ResumeLayout(false);
            PnlResult.PerformLayout();
            GrbResult.ResumeLayout(false);
            GrbResult.PerformLayout();
            TlpSettings.ResumeLayout(false);
            TlpSettings.PerformLayout();
            ((ISupportInitialize)TrbMaxFrameComparison).EndInit();
            ((ISupportInitialize)TrbMaxDifferentFrames).EndInit();
            ((ISupportInitialize)TrbMaxDifferentPercentage).EndInit();
            ((ISupportInitialize)NumMaxFrameComparison).EndInit();
            ((ISupportInitialize)NumMaxDifferentFrames).EndInit();
            ((ISupportInitialize)NumMaxDifferentPercentage).EndInit();
            GrbSettings.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel TlpResult;
        private Panel PnlResult;
        private GroupBox GrbFirstLoadLevel;
        private TableLayoutPanel TlpFirstLoadLevelResult;
        private GroupBox GrbSecondLoadLevel;
        private TableLayoutPanel TlpSecondLoadLevelResult;
        private Button BtnCancel;
        private Button BtnOkay;
        private TableLayoutPanel TlpSettings;
        private Label label1;
        private Label label2;
        private TextBox TxtLeftFilePath;
        private TextBox TxtRightFilePath;
        private Button BtnSelectLeftFilePath;
        private Button BtnSelectRightFilePath;
        private Label label3;
        private Label label4;
        private Label label5;
        private NumericUpDown NumMaxFrameComparison;
        private NumericUpDown NumMaxDifferentFrames;
        private NumericUpDown NumMaxDifferentPercentage;
        private GroupBox GrbResult;
        private Label LblResult;
        private GroupBox GrbThirdLoadLevel;
        private TableLayoutPanel TlpThirdLoadLevelResult;
        private Timer StatusTimer;
        private Button BtnStartComparison;
        private Button btnClose;
        private TableLayoutPanel TlpProgress;
        private ProgressBar PgbComparisonProgress;
        private GroupBox GrbVideoTimeline;
        private TableLayoutPanel TlpVideoTimeline;
        private TrackBar TrbMaxFrameComparison;
        private RadioButton RdbSortByProcessingOrder;
        private RadioButton RdbSortByTimeline;
        private TrackBar TrbMaxDifferentFrames;
        private TrackBar TrbMaxDifferentPercentage;
        private Label label6;
        private GroupBox GrbSettings;
        private TableLayoutPanel TlpFirstLoadLevel;
        private PictureBox PibFirstLoadLevel;
        private Label LblFirstLoadLevel;
        private TableLayoutPanel TlpSecondLoadLevel;
        private Label LblSecondLoadLevel;
        private PictureBox PibSecondLoadLevel;
        private TableLayoutPanel TlpThirdLoadLevel;
        private PictureBox PibThirdLoadLevel;
        private Label LblThirdLoadLevel;
        private TextBox TxtLeftFileInfo;
        private TextBox TxtRightFileInfo;
    }
}
