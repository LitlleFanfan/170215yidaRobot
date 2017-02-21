namespace yidascan
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRun = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnReset = new System.Windows.Forms.ToolStripButton();
            this.btnSet = new System.Windows.Forms.ToolStripButton();
            this.btnLog = new System.Windows.Forms.ToolStripButton();
            this.btnOther = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnStartRobot = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStopRobot = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnWeighReset = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnBrowsePanels = new System.Windows.Forms.ToolStripMenuItem();
            this.btnTestPlc = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSignalGen = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSignalWeigh = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSignalCache = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSignalLabelUp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSignalItemCatchA = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSignalItemCatchB = new System.Windows.Forms.ToolStripMenuItem();
            this.btnQuit = new System.Windows.Forms.ToolStripButton();
            this.grbHandwork = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtLableCode1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lsvLog = new System.Windows.Forms.ListBox();
            this.lsvBufferLog = new System.Windows.Forms.ListBox();
            this.lsvRobotStackLog = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblScanner = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblScanner2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblOpcIp = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRobot = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbRobotState = new System.Windows.Forms.Label();
            this.chkUseRobot = new System.Windows.Forms.CheckBox();
            this.lbTaskState = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.cmbShiftNo = new System.Windows.Forms.ComboBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.label10 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer_message = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbcLogs = new System.Windows.Forms.TabControl();
            this.tbpScanLog = new System.Windows.Forms.TabPage();
            this.tbpCacheLog = new System.Windows.Forms.TabPage();
            this.tbpRobotLog = new System.Windows.Forms.TabPage();
            this.tbpALarmLog = new System.Windows.Forms.TabPage();
            this.lsvAlarmLog = new System.Windows.Forms.ListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            lsvRobotA = new System.Windows.Forms.ListView();
            this.label6 = new System.Windows.Forms.Label();
            this.lsvCatch2 = new System.Windows.Forms.ListView();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lsvCatch1 = new System.Windows.Forms.ListView();
            this.lsvLableUp = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.lsvCacheBefor = new System.Windows.Forms.ListView();
            this.lsvLableCode = new System.Windows.Forms.ListView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.listView2 = new System.Windows.Forms.ListView();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.listView3 = new System.Windows.Forms.ListView();
            this.listView4 = new System.Windows.Forms.ListView();
            this.label8 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.panelDataZone = new System.Windows.Forms.Panel();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.label9 = new System.Windows.Forms.Label();
            lsvRobotB = new System.Windows.Forms.ListView();
            this.toolStrip1.SuspendLayout();
            this.grbHandwork.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tbcLogs.SuspendLayout();
            this.tbpScanLog.SuspendLayout();
            this.tbpCacheLog.SuspendLayout();
            this.tbpRobotLog.SuspendLayout();
            this.tbpALarmLog.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panelDataZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).BeginInit();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRun,
            this.btnStop,
            this.btnReset,
            this.btnSet,
            this.btnLog,
            this.btnOther,
            this.btnQuit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1276, 67);
            this.toolStrip1.TabIndex = 48;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnRun
            // 
            this.btnRun.AutoSize = false;
            this.btnRun.BackColor = System.Drawing.Color.LimeGreen;
            this.btnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRun.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnRun.ForeColor = System.Drawing.Color.Honeydew;
            this.btnRun.Image = ((System.Drawing.Image)(resources.GetObject("btnRun.Image")));
            this.btnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(160, 64);
            this.btnRun.Text = "启动(&R)";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStop
            // 
            this.btnStop.AutoSize = false;
            this.btnStop.BackColor = System.Drawing.Color.Red;
            this.btnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("微软雅黑", 14.25F);
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(160, 64);
            this.btnStop.Text = "停止(&S)";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnReset
            // 
            this.btnReset.AutoSize = false;
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnReset.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnReset.Image = ((System.Drawing.Image)(resources.GetObject("btnReset.Image")));
            this.btnReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(160, 64);
            this.btnReset.Text = "传送复位";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSet
            // 
            this.btnSet.AutoSize = false;
            this.btnSet.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSet.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnSet.Image = ((System.Drawing.Image)(resources.GetObject("btnSet.Image")));
            this.btnSet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(160, 64);
            this.btnSet.Text = "设置(&M)";
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // btnLog
            // 
            this.btnLog.AutoSize = false;
            this.btnLog.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnLog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLog.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnLog.Image = ((System.Drawing.Image)(resources.GetObject("btnLog.Image")));
            this.btnLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(160, 64);
            this.btnLog.Text = "日志(&L)";
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // btnOther
            // 
            this.btnOther.AutoSize = false;
            this.btnOther.BackColor = System.Drawing.Color.LightYellow;
            this.btnOther.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOther.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStartRobot,
            this.btnStopRobot,
            this.toolStripMenuItem3,
            this.btnWeighReset,
            this.toolStripMenuItem2,
            this.btnDelete,
            this.toolStripMenuItem1,
            this.btnHelp,
            this.toolStripMenuItem4,
            this.btnBrowsePanels,
            this.btnTestPlc,
            this.btnSignalGen});
            this.btnOther.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnOther.Image = ((System.Drawing.Image)(resources.GetObject("btnOther.Image")));
            this.btnOther.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOther.Name = "btnOther";
            this.btnOther.Size = new System.Drawing.Size(160, 64);
            this.btnOther.Text = "其他";
            // 
            // btnStartRobot
            // 
            this.btnStartRobot.Name = "btnStartRobot";
            this.btnStartRobot.Size = new System.Drawing.Size(217, 30);
            this.btnStartRobot.Text = "启动机器人任务";
            this.btnStartRobot.Click += new System.EventHandler(this.btnStartRobot_Click);
            // 
            // btnStopRobot
            // 
            this.btnStopRobot.Name = "btnStopRobot";
            this.btnStopRobot.Size = new System.Drawing.Size(217, 30);
            this.btnStopRobot.Text = "停止机器人任务";
            this.btnStopRobot.Click += new System.EventHandler(this.btnStopRobot_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(214, 6);
            // 
            // btnWeighReset
            // 
            this.btnWeighReset.Name = "btnWeighReset";
            this.btnWeighReset.Size = new System.Drawing.Size(217, 30);
            this.btnWeighReset.Text = "称重复位";
            this.btnWeighReset.Click += new System.EventHandler(this.btnWeighReset_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(214, 6);
            // 
            // btnDelete
            // 
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(217, 30);
            this.btnDelete.Text = "删除标签(&D)";
            this.btnDelete.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(214, 6);
            // 
            // btnHelp
            // 
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(217, 30);
            this.btnHelp.Text = "帮助(&H)";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(214, 6);
            // 
            // btnBrowsePanels
            // 
            this.btnBrowsePanels.Name = "btnBrowsePanels";
            this.btnBrowsePanels.Size = new System.Drawing.Size(217, 30);
            this.btnBrowsePanels.Text = "板状态浏览";
            this.btnBrowsePanels.Click += new System.EventHandler(this.btnBrowsePanels_Click);
            // 
            // btnTestPlc
            // 
            this.btnTestPlc.Name = "btnTestPlc";
            this.btnTestPlc.Size = new System.Drawing.Size(217, 30);
            this.btnTestPlc.Text = "测试";
            this.btnTestPlc.Click += new System.EventHandler(this.btnTestPlc_Click);
            // 
            // btnSignalGen
            // 
            this.btnSignalGen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSignalWeigh,
            this.btnSignalCache,
            this.btnSignalLabelUp,
            this.btnSignalItemCatchA,
            this.btnSignalItemCatchB});
            this.btnSignalGen.Name = "btnSignalGen";
            this.btnSignalGen.Size = new System.Drawing.Size(217, 30);
            this.btnSignalGen.Text = "模拟信号发生";
            // 
            // btnSignalWeigh
            // 
            this.btnSignalWeigh.Name = "btnSignalWeigh";
            this.btnSignalWeigh.Size = new System.Drawing.Size(255, 30);
            this.btnSignalWeigh.Text = "称重信号启动";
            this.btnSignalWeigh.Click += new System.EventHandler(this.btnSignalWeigh_Click);
            // 
            // btnSignalCache
            // 
            this.btnSignalCache.Name = "btnSignalCache";
            this.btnSignalCache.Size = new System.Drawing.Size(255, 30);
            this.btnSignalCache.Text = "缓存信号启动";
            this.btnSignalCache.Click += new System.EventHandler(this.btnSignalCache_Click);
            // 
            // btnSignalLabelUp
            // 
            this.btnSignalLabelUp.Name = "btnSignalLabelUp";
            this.btnSignalLabelUp.Size = new System.Drawing.Size(255, 30);
            this.btnSignalLabelUp.Text = "标签向上处信号启动";
            this.btnSignalLabelUp.Click += new System.EventHandler(this.btnSignalLabelUp_Click);
            // 
            // btnSignalItemCatchA
            // 
            this.btnSignalItemCatchA.Name = "btnSignalItemCatchA";
            this.btnSignalItemCatchA.Size = new System.Drawing.Size(255, 30);
            this.btnSignalItemCatchA.Text = "抓料处A信号启动";
            this.btnSignalItemCatchA.Click += new System.EventHandler(this.btnSignalItemCatchA_Click);
            // 
            // btnSignalItemCatchB
            // 
            this.btnSignalItemCatchB.Name = "btnSignalItemCatchB";
            this.btnSignalItemCatchB.Size = new System.Drawing.Size(255, 30);
            this.btnSignalItemCatchB.Text = "抓料处B信号启动";
            this.btnSignalItemCatchB.Click += new System.EventHandler(this.btnSignalItemCatchB_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.AutoSize = false;
            this.btnQuit.BackColor = System.Drawing.Color.Orange;
            this.btnQuit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnQuit.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnQuit.Image = ((System.Drawing.Image)(resources.GetObject("btnQuit.Image")));
            this.btnQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(160, 64);
            this.btnQuit.Text = "退出(&Q)";
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // grbHandwork
            // 
            this.grbHandwork.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbHandwork.Controls.Add(this.panel1);
            this.grbHandwork.Location = new System.Drawing.Point(611, 70);
            this.grbHandwork.Margin = new System.Windows.Forms.Padding(5);
            this.grbHandwork.Name = "grbHandwork";
            this.grbHandwork.Padding = new System.Windows.Forms.Padding(5);
            this.grbHandwork.Size = new System.Drawing.Size(653, 131);
            this.grbHandwork.TabIndex = 51;
            this.grbHandwork.TabStop = false;
            this.grbHandwork.Text = "手动操作";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtLableCode1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(16, 28);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(628, 95);
            this.panel1.TabIndex = 8;
            // 
            // txtLableCode1
            // 
            this.txtLableCode1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLableCode1.Font = new System.Drawing.Font("宋体", 24F);
            this.txtLableCode1.ForeColor = System.Drawing.Color.DarkRed;
            this.txtLableCode1.Location = new System.Drawing.Point(183, 18);
            this.txtLableCode1.Margin = new System.Windows.Forms.Padding(4);
            this.txtLableCode1.Multiline = true;
            this.txtLableCode1.Name = "txtLableCode1";
            this.txtLableCode1.Size = new System.Drawing.Size(431, 62);
            this.txtLableCode1.TabIndex = 1;
            this.txtLableCode1.Text = "请将光标放置到这里扫描";
            this.txtLableCode1.Enter += new System.EventHandler(this.txtLableCode1_Enter);
            this.txtLableCode1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLableCode1_KeyPress);
            this.txtLableCode1.Leave += new System.EventHandler(this.txtLableCode1_Leave);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Moccasin;
            this.label5.Font = new System.Drawing.Font("宋体", 24F);
            this.label5.Location = new System.Drawing.Point(14, 18);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 62);
            this.label5.TabIndex = 5;
            this.label5.Text = "扫码";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lsvLog
            // 
            this.lsvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvLog.Font = new System.Drawing.Font("宋体", 14F);
            this.lsvLog.FormattingEnabled = true;
            this.lsvLog.ItemHeight = 19;
            this.lsvLog.Location = new System.Drawing.Point(3, 3);
            this.lsvLog.Name = "lsvLog";
            this.lsvLog.Size = new System.Drawing.Size(125, 198);
            this.lsvLog.TabIndex = 2;
            // 
            // lsvBufferLog
            // 
            this.lsvBufferLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvBufferLog.ItemHeight = 19;
            this.lsvBufferLog.Location = new System.Drawing.Point(3, 3);
            this.lsvBufferLog.MinimumSize = new System.Drawing.Size(250, 4);
            this.lsvBufferLog.Name = "lsvBufferLog";
            this.lsvBufferLog.Size = new System.Drawing.Size(405, 205);
            this.lsvBufferLog.TabIndex = 4;
            // 
            // lsvRobotStackLog
            // 
            this.lsvRobotStackLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvRobotStackLog.ItemHeight = 19;
            this.lsvRobotStackLog.Location = new System.Drawing.Point(0, 0);
            this.lsvRobotStackLog.MinimumSize = new System.Drawing.Size(250, 4);
            this.lsvRobotStackLog.Name = "lsvRobotStackLog";
            this.lsvRobotStackLog.Size = new System.Drawing.Size(411, 211);
            this.lsvRobotStackLog.TabIndex = 4;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblScanner,
            this.lblScanner2,
            this.lblOpcIp,
            this.lblRobot,
            this.toolStripStatusLabel2,
            this.lblTimer});
            this.statusStrip1.Location = new System.Drawing.Point(0, 667);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 18, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1276, 26);
            this.statusStrip1.TabIndex = 53;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Gray;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(306, 21);
            this.toolStripStatusLabel1.Text = "guangzhou golden beaver software.";
            // 
            // lblScanner
            // 
            this.lblScanner.BackColor = System.Drawing.Color.LightGray;
            this.lblScanner.Name = "lblScanner";
            this.lblScanner.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lblScanner.Size = new System.Drawing.Size(20, 21);
            // 
            // lblScanner2
            // 
            this.lblScanner2.BackColor = System.Drawing.Color.LightGray;
            this.lblScanner2.Name = "lblScanner2";
            this.lblScanner2.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lblScanner2.Size = new System.Drawing.Size(20, 21);
            // 
            // lblOpcIp
            // 
            this.lblOpcIp.BackColor = System.Drawing.Color.LightGray;
            this.lblOpcIp.Name = "lblOpcIp";
            this.lblOpcIp.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lblOpcIp.Size = new System.Drawing.Size(20, 21);
            // 
            // lblRobot
            // 
            this.lblRobot.BackColor = System.Drawing.Color.LightGray;
            this.lblRobot.Name = "lblRobot";
            this.lblRobot.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lblRobot.Size = new System.Drawing.Size(20, 21);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(851, 21);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // lblTimer
            // 
            this.lblTimer.BackColor = System.Drawing.Color.LightPink;
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lblTimer.Size = new System.Drawing.Size(20, 21);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbRobotState);
            this.groupBox1.Controls.Add(this.chkUseRobot);
            this.groupBox1.Controls.Add(this.lbTaskState);
            this.groupBox1.Controls.Add(this.lblCount);
            this.groupBox1.Controls.Add(this.cmbShiftNo);
            this.groupBox1.Controls.Add(this.dtpDate);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(4, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(599, 131);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "信息";
            // 
            // lbRobotState
            // 
            this.lbRobotState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lbRobotState.Font = new System.Drawing.Font("黑体", 20F);
            this.lbRobotState.Location = new System.Drawing.Point(260, 89);
            this.lbRobotState.Name = "lbRobotState";
            this.lbRobotState.Size = new System.Drawing.Size(333, 37);
            this.lbRobotState.TabIndex = 18;
            this.lbRobotState.Text = "机器人状态";
            this.lbRobotState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkUseRobot
            // 
            this.chkUseRobot.BackColor = System.Drawing.Color.White;
            this.chkUseRobot.Font = new System.Drawing.Font("宋体", 20F);
            this.chkUseRobot.Location = new System.Drawing.Point(7, 89);
            this.chkUseRobot.Name = "chkUseRobot";
            this.chkUseRobot.Size = new System.Drawing.Size(253, 37);
            this.chkUseRobot.TabIndex = 17;
            this.chkUseRobot.Text = "启动机器人";
            this.chkUseRobot.UseVisualStyleBackColor = false;
            // 
            // lbTaskState
            // 
            this.lbTaskState.BackColor = System.Drawing.Color.Green;
            this.lbTaskState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbTaskState.Font = new System.Drawing.Font("黑体", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTaskState.ForeColor = System.Drawing.Color.White;
            this.lbTaskState.Location = new System.Drawing.Point(260, 26);
            this.lbTaskState.Name = "lbTaskState";
            this.lbTaskState.Size = new System.Drawing.Size(333, 59);
            this.lbTaskState.TabIndex = 16;
            this.lbTaskState.Text = "空闲";
            this.lbTaskState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCount
            // 
            this.lblCount.BackColor = System.Drawing.Color.White;
            this.lblCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCount.Font = new System.Drawing.Font("微软雅黑", 35F, System.Drawing.FontStyle.Bold);
            this.lblCount.ForeColor = System.Drawing.Color.Black;
            this.lblCount.Location = new System.Drawing.Point(3, 26);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(257, 59);
            this.lblCount.TabIndex = 15;
            this.lblCount.Text = "0";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbShiftNo
            // 
            this.cmbShiftNo.FormattingEnabled = true;
            this.cmbShiftNo.Items.AddRange(new object[] {
            "白班",
            "中班",
            "夜班"});
            this.cmbShiftNo.Location = new System.Drawing.Point(272, 31);
            this.cmbShiftNo.Name = "cmbShiftNo";
            this.cmbShiftNo.Size = new System.Drawing.Size(321, 27);
            this.cmbShiftNo.TabIndex = 10;
            this.cmbShiftNo.Text = "白班";
            this.cmbShiftNo.Visible = false;
            // 
            // dtpDate
            // 
            this.dtpDate.CustomFormat = "yyyy-MM-dd";
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(63, 29);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(203, 29);
            this.dtpDate.TabIndex = 9;
            this.dtpDate.Visible = false;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Location = new System.Drawing.Point(5, 25);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 36);
            this.label10.TabIndex = 12;
            this.label10.Text = "班次";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label10.Visible = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer_message
            // 
            this.timer_message.Interval = 200;
            this.timer_message.Tick += new System.EventHandler(this.timer_message_Tick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(1130, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1471, 67);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 50;
            this.pictureBox1.TabStop = false;
            // 
            // tbcLogs
            // 
            this.tbcLogs.Controls.Add(this.tbpScanLog);
            this.tbcLogs.Controls.Add(this.tbpCacheLog);
            this.tbcLogs.Controls.Add(this.tbpRobotLog);
            this.tbcLogs.Controls.Add(this.tbpALarmLog);
            this.tbcLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcLogs.Location = new System.Drawing.Point(0, 0);
            this.tbcLogs.Name = "tbcLogs";
            this.tbcLogs.SelectedIndex = 0;
            this.tbcLogs.Size = new System.Drawing.Size(139, 237);
            this.tbcLogs.TabIndex = 55;
            // 
            // tbpScanLog
            // 
            this.tbpScanLog.Controls.Add(this.lsvLog);
            this.tbpScanLog.Location = new System.Drawing.Point(4, 29);
            this.tbpScanLog.Name = "tbpScanLog";
            this.tbpScanLog.Padding = new System.Windows.Forms.Padding(3);
            this.tbpScanLog.Size = new System.Drawing.Size(131, 204);
            this.tbpScanLog.TabIndex = 0;
            this.tbpScanLog.Text = "采集日志";
            this.tbpScanLog.UseVisualStyleBackColor = true;
            // 
            // tbpCacheLog
            // 
            this.tbpCacheLog.Controls.Add(this.lsvBufferLog);
            this.tbpCacheLog.Location = new System.Drawing.Point(4, 22);
            this.tbpCacheLog.Name = "tbpCacheLog";
            this.tbpCacheLog.Padding = new System.Windows.Forms.Padding(3);
            this.tbpCacheLog.Size = new System.Drawing.Size(411, 211);
            this.tbpCacheLog.TabIndex = 1;
            this.tbpCacheLog.Text = "缓存日志";
            this.tbpCacheLog.UseVisualStyleBackColor = true;
            // 
            // tbpRobotLog
            // 
            this.tbpRobotLog.Controls.Add(this.lsvRobotStackLog);
            this.tbpRobotLog.Location = new System.Drawing.Point(4, 22);
            this.tbpRobotLog.Name = "tbpRobotLog";
            this.tbpRobotLog.Size = new System.Drawing.Size(411, 211);
            this.tbpRobotLog.TabIndex = 2;
            this.tbpRobotLog.Text = "机器人日志";
            this.tbpRobotLog.UseVisualStyleBackColor = true;
            // 
            // tbpALarmLog
            // 
            this.tbpALarmLog.Controls.Add(this.lsvAlarmLog);
            this.tbpALarmLog.Location = new System.Drawing.Point(4, 22);
            this.tbpALarmLog.Name = "tbpALarmLog";
            this.tbpALarmLog.Size = new System.Drawing.Size(411, 211);
            this.tbpALarmLog.TabIndex = 3;
            this.tbpALarmLog.Text = "报警日志";
            this.tbpALarmLog.UseVisualStyleBackColor = true;
            // 
            // lsvAlarmLog
            // 
            this.lsvAlarmLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvAlarmLog.ItemHeight = 19;
            this.lsvAlarmLog.Location = new System.Drawing.Point(0, 0);
            this.lsvAlarmLog.MinimumSize = new System.Drawing.Size(250, 4);
            this.lsvAlarmLog.Name = "lsvAlarmLog";
            this.lsvAlarmLog.Size = new System.Drawing.Size(411, 211);
            this.lsvAlarmLog.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(lsvRobotB);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(lsvRobotA);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.lsvCatch2);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.lsvCatch1);
            this.panel3.Controls.Add(this.lsvLableUp);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.lsvCacheBefor);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1114, 260);
            this.panel3.TabIndex = 56;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.SkyBlue;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(802, 0);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(267, 32);
            this.label7.TabIndex = 10;
            this.label7.Text = "机器人任务A队列";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lsvRobotA
            // 
            lsvRobotA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            lsvRobotA.Location = new System.Drawing.Point(802, 32);
            lsvRobotA.Name = "lsvRobotA";
            lsvRobotA.Size = new System.Drawing.Size(267, 129);
            lsvRobotA.TabIndex = 9;
            lsvRobotA.UseCompatibleStateImageBehavior = false;
            lsvRobotA.View = System.Windows.Forms.View.List;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.SkyBlue;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(535, 161);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(267, 32);
            this.label6.TabIndex = 8;
            this.label6.Text = "抓料B队列";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lsvCatch2
            // 
            this.lsvCatch2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lsvCatch2.Location = new System.Drawing.Point(535, 190);
            this.lsvCatch2.Name = "lsvCatch2";
            this.lsvCatch2.Size = new System.Drawing.Size(267, 68);
            this.lsvCatch2.TabIndex = 7;
            this.lsvCatch2.UseCompatibleStateImageBehavior = false;
            this.lsvCatch2.View = System.Windows.Forms.View.List;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.SkyBlue;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(535, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(267, 32);
            this.label4.TabIndex = 6;
            this.label4.Text = "抓料A队列";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.SkyBlue;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(268, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(267, 32);
            this.label3.TabIndex = 6;
            this.label3.Text = "标签朝上采集队列";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lsvCatch1
            // 
            this.lsvCatch1.Location = new System.Drawing.Point(535, 32);
            this.lsvCatch1.Name = "lsvCatch1";
            this.lsvCatch1.Size = new System.Drawing.Size(267, 129);
            this.lsvCatch1.TabIndex = 5;
            this.lsvCatch1.UseCompatibleStateImageBehavior = false;
            this.lsvCatch1.View = System.Windows.Forms.View.List;
            // 
            // lsvLableUp
            // 
            this.lsvLableUp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lsvLableUp.Location = new System.Drawing.Point(268, 32);
            this.lsvLableUp.Name = "lsvLableUp";
            this.lsvLableUp.Size = new System.Drawing.Size(267, 226);
            this.lsvLableUp.TabIndex = 5;
            this.lsvLableUp.UseCompatibleStateImageBehavior = false;
            this.lsvLableUp.View = System.Windows.Forms.View.List;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.SkyBlue;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(1, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(267, 32);
            this.label2.TabIndex = 4;
            this.label2.Text = "缓存前队列";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lsvCacheBefor
            // 
            this.lsvCacheBefor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lsvCacheBefor.Location = new System.Drawing.Point(1, 32);
            this.lsvCacheBefor.Name = "lsvCacheBefor";
            this.lsvCacheBefor.Size = new System.Drawing.Size(267, 226);
            this.lsvCacheBefor.TabIndex = 0;
            this.lsvCacheBefor.UseCompatibleStateImageBehavior = false;
            this.lsvCacheBefor.View = System.Windows.Forms.View.List;
            // 
            // lsvLableCode
            // 
            this.lsvLableCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvLableCode.Location = new System.Drawing.Point(0, 32);
            this.lsvLableCode.Name = "lsvLableCode";
            this.lsvLableCode.Size = new System.Drawing.Size(139, 232);
            this.lsvLableCode.TabIndex = 4;
            this.lsvLableCode.UseCompatibleStateImageBehavior = false;
            this.lsvLableCode.View = System.Windows.Forms.View.List;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Size = new System.Drawing.Size(1114, 505);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.TabIndex = 58;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer3.Location = new System.Drawing.Point(0, 35);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer5);
            this.splitContainer3.Size = new System.Drawing.Size(1114, 206);
            this.splitContainer3.SplitterDistance = 102;
            this.splitContainer3.TabIndex = 5;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.listView2);
            this.splitContainer4.Size = new System.Drawing.Size(1114, 102);
            this.splitContainer4.SplitterDistance = 539;
            this.splitContainer4.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(539, 102);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // listView2
            // 
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.Location = new System.Drawing.Point(0, 0);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(571, 102);
            this.listView2.TabIndex = 1;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.List;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.listView3);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.listView4);
            this.splitContainer5.Size = new System.Drawing.Size(1114, 100);
            this.splitContainer5.SplitterDistance = 536;
            this.splitContainer5.TabIndex = 0;
            // 
            // listView3
            // 
            this.listView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView3.Location = new System.Drawing.Point(0, 0);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(536, 100);
            this.listView3.TabIndex = 1;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.List;
            // 
            // listView4
            // 
            this.listView4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView4.Location = new System.Drawing.Point(0, 0);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(574, 100);
            this.listView4.TabIndex = 1;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.List;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.BackColor = System.Drawing.Color.SkyBlue;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(0, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(1114, 32);
            this.label8.TabIndex = 4;
            this.label8.Text = "缓存区列表";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lsvLableCode);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tbcLogs);
            this.splitContainer2.Size = new System.Drawing.Size(139, 505);
            this.splitContainer2.SplitterDistance = 264;
            this.splitContainer2.TabIndex = 59;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.SkyBlue;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 32);
            this.label1.TabIndex = 58;
            this.label1.Text = "数据";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelDataZone
            // 
            this.panelDataZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDataZone.Controls.Add(this.splitContainer6);
            this.panelDataZone.Location = new System.Drawing.Point(7, 207);
            this.panelDataZone.Name = "panelDataZone";
            this.panelDataZone.Size = new System.Drawing.Size(1257, 505);
            this.panelDataZone.TabIndex = 60;
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer6.Location = new System.Drawing.Point(0, 0);
            this.splitContainer6.Name = "splitContainer6";
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer6.Size = new System.Drawing.Size(1257, 505);
            this.splitContainer6.SplitterDistance = 139;
            this.splitContainer6.TabIndex = 60;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.SkyBlue;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(802, 161);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(267, 32);
            this.label9.TabIndex = 12;
            this.label9.Text = "机器人任务B队列";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lsvRobotB
            // 
            lsvRobotB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            lsvRobotB.Location = new System.Drawing.Point(802, 193);
            lsvRobotB.Name = "lsvRobotB";
            lsvRobotB.Size = new System.Drawing.Size(267, 65);
            lsvRobotB.TabIndex = 11;
            lsvRobotB.UseCompatibleStateImageBehavior = false;
            lsvRobotB.View = System.Windows.Forms.View.List;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 693);
            this.Controls.Add(this.panelDataZone);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.grbHandwork);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("宋体", 14F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据采集软件";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grbHandwork.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tbcLogs.ResumeLayout(false);
            this.tbpScanLog.ResumeLayout(false);
            this.tbpCacheLog.ResumeLayout(false);
            this.tbpRobotLog.ResumeLayout(false);
            this.tbpALarmLog.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panelDataZone.ResumeLayout(false);
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).EndInit();
            this.splitContainer6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRun;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripButton btnQuit;
        private System.Windows.Forms.GroupBox grbHandwork;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtLableCode1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblScanner;
        private System.Windows.Forms.ToolStripStatusLabel lblScanner2;
        private System.Windows.Forms.ToolStripButton btnSet;
        private System.Windows.Forms.ToolStripStatusLabel lblTimer;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbShiftNo;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolStripButton btnLog;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripStatusLabel lblOpcIp;
        private System.Windows.Forms.Label lbTaskState;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.ToolStripButton btnReset;
        private System.Windows.Forms.Timer timer_message;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripStatusLabel lblRobot;
        private System.Windows.Forms.CheckBox chkUseRobot;
        private System.Windows.Forms.ListBox lsvBufferLog;
        private System.Windows.Forms.ListBox lsvRobotStackLog;
        private System.Windows.Forms.ToolStripDropDownButton btnOther;
        private System.Windows.Forms.ToolStripMenuItem btnStartRobot;
        private System.Windows.Forms.ToolStripMenuItem btnStopRobot;
        private System.Windows.Forms.Label lbRobotState;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem btnDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem btnHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem btnWeighReset;
        private System.Windows.Forms.ListBox lsvLog;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem btnBrowsePanels;
        private System.Windows.Forms.ToolStripMenuItem btnTestPlc;
        private System.Windows.Forms.TabControl tbcLogs;
        private System.Windows.Forms.TabPage tbpScanLog;
        private System.Windows.Forms.TabPage tbpCacheLog;
        private System.Windows.Forms.TabPage tbpRobotLog;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView lsvCacheBefor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView lsvLableUp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView lsvCatch2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView lsvCatch1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListView lsvLableCode;
        private System.Windows.Forms.TabPage tbpALarmLog;
        private System.Windows.Forms.ListBox lsvAlarmLog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ListView listView4;
        private System.Windows.Forms.Panel panelDataZone;
        private System.Windows.Forms.SplitContainer splitContainer6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem btnSignalGen;
        private System.Windows.Forms.ToolStripMenuItem btnSignalWeigh;
        private System.Windows.Forms.ToolStripMenuItem btnSignalCache;
        private System.Windows.Forms.ToolStripMenuItem btnSignalLabelUp;
        private System.Windows.Forms.ToolStripMenuItem btnSignalItemCatchA;
        private System.Windows.Forms.ToolStripMenuItem btnSignalItemCatchB;
        private System.Windows.Forms.Label label9;
        public static System.Windows.Forms.ListView lsvRobotA;
        public static System.Windows.Forms.ListView lsvRobotB;
    }
}

