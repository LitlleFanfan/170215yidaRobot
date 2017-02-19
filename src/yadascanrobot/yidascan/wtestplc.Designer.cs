namespace yidascan {
    partial class wtestplc {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ntxtPos = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ntxtChannel = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txDiameter = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ntxtCachePosGet = new System.Windows.Forms.NumericUpDown();
            this.ntxtCachePosSave = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cbxJob = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnWriteSignal = new System.Windows.Forms.Button();
            this.btnReadCacheSignal = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnPushAside = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.txPlcIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSetPlcIP = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtPos)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtChannel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtCachePosGet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtCachePosSave)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(0, 247);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(686, 160);
            this.listBox1.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.ntxtPos);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.button6);
            this.tabPage3.Controls.Add(this.button5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(678, 221);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "抓料处";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // ntxtPos
            // 
            this.ntxtPos.Location = new System.Drawing.Point(122, 31);
            this.ntxtPos.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.ntxtPos.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ntxtPos.Name = "ntxtPos";
            this.ntxtPos.Size = new System.Drawing.Size(120, 21);
            this.ntxtPos.TabIndex = 3;
            this.ntxtPos.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "抓料处";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(54, 115);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(229, 23);
            this.button6.TabIndex = 1;
            this.button6.Text = "复位抓料处来料信号";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(54, 73);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(229, 23);
            this.button5.TabIndex = 0;
            this.button5.Text = "读抓料处来料信号";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.ntxtChannel);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.txDiameter);
            this.tabPage2.Controls.Add(this.button4);
            this.tabPage2.Controls.Add(this.numericUpDown2);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(678, 221);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "标签采集（标签朝上）";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ntxtChannel
            // 
            this.ntxtChannel.Location = new System.Drawing.Point(486, 79);
            this.ntxtChannel.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.ntxtChannel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ntxtChannel.Name = "ntxtChannel";
            this.ntxtChannel.Size = new System.Drawing.Size(83, 21);
            this.ntxtChannel.TabIndex = 7;
            this.ntxtChannel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(437, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "通道";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(252, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "直径";
            // 
            // txDiameter
            // 
            this.txDiameter.Location = new System.Drawing.Point(307, 79);
            this.txDiameter.Name = "txDiameter";
            this.txDiameter.Size = new System.Drawing.Size(80, 21);
            this.txDiameter.TabIndex = 4;
            this.txDiameter.Text = "180";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(33, 117);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(180, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "写标签采集抓料信号";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(93, 80);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(120, 21);
            this.numericUpDown2.TabIndex = 2;
            this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "抓料处";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(30, 23);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(162, 23);
            this.button3.TabIndex = 0;
            this.button3.Text = "读标签采集来料信号";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ntxtCachePosGet);
            this.tabPage1.Controls.Add(this.ntxtCachePosSave);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.cbxJob);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.btnWriteSignal);
            this.tabPage1.Controls.Add(this.btnReadCacheSignal);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(678, 221);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "缓存来料";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ntxtCachePosGet
            // 
            this.ntxtCachePosGet.Location = new System.Drawing.Point(512, 79);
            this.ntxtCachePosGet.Name = "ntxtCachePosGet";
            this.ntxtCachePosGet.Size = new System.Drawing.Size(100, 21);
            this.ntxtCachePosGet.TabIndex = 14;
            // 
            // ntxtCachePosSave
            // 
            this.ntxtCachePosSave.Location = new System.Drawing.Point(329, 79);
            this.ntxtCachePosSave.Name = "ntxtCachePosSave";
            this.ntxtCachePosSave.Size = new System.Drawing.Size(100, 21);
            this.ntxtCachePosSave.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(454, 84);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 12);
            this.label11.TabIndex = 12;
            this.label11.Text = "取位置";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(266, 84);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 10;
            this.label10.Text = "存位置";
            // 
            // cbxJob
            // 
            this.cbxJob.FormattingEnabled = true;
            this.cbxJob.Location = new System.Drawing.Point(106, 77);
            this.cbxJob.Name = "cbxJob";
            this.cbxJob.Size = new System.Drawing.Size(121, 20);
            this.cbxJob.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "缓存区动作";
            // 
            // btnWriteSignal
            // 
            this.btnWriteSignal.Location = new System.Drawing.Point(26, 124);
            this.btnWriteSignal.Name = "btnWriteSignal";
            this.btnWriteSignal.Size = new System.Drawing.Size(162, 23);
            this.btnWriteSignal.TabIndex = 7;
            this.btnWriteSignal.Text = "写缓存动作";
            this.btnWriteSignal.UseVisualStyleBackColor = true;
            this.btnWriteSignal.Click += new System.EventHandler(this.btnWriteSignal_Click);
            // 
            // btnReadCacheSignal
            // 
            this.btnReadCacheSignal.Location = new System.Drawing.Point(30, 18);
            this.btnReadCacheSignal.Name = "btnReadCacheSignal";
            this.btnReadCacheSignal.Size = new System.Drawing.Size(158, 23);
            this.btnReadCacheSignal.TabIndex = 6;
            this.btnReadCacheSignal.Text = "读缓存来料信号";
            this.btnReadCacheSignal.UseVisualStyleBackColor = true;
            this.btnReadCacheSignal.Click += new System.EventHandler(this.btnReadCacheSignal_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(686, 247);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnPushAside);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(678, 221);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "勾料信号";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnPushAside
            // 
            this.btnPushAside.Location = new System.Drawing.Point(28, 35);
            this.btnPushAside.Name = "btnPushAside";
            this.btnPushAside.Size = new System.Drawing.Size(150, 23);
            this.btnPushAside.TabIndex = 0;
            this.btnPushAside.Text = "测试勾料信号";
            this.btnPushAside.UseVisualStyleBackColor = true;
            this.btnPushAside.Click += new System.EventHandler(this.btnPushAside_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.txPlcIP);
            this.tabPage5.Controls.Add(this.label3);
            this.tabPage5.Controls.Add(this.btnSetPlcIP);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(678, 221);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "plc ip设置";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // txPlcIP
            // 
            this.txPlcIP.Location = new System.Drawing.Point(100, 33);
            this.txPlcIP.Name = "txPlcIP";
            this.txPlcIP.Size = new System.Drawing.Size(181, 21);
            this.txPlcIP.TabIndex = 2;
            this.txPlcIP.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "plc ip";
            // 
            // btnSetPlcIP
            // 
            this.btnSetPlcIP.Location = new System.Drawing.Point(28, 86);
            this.btnSetPlcIP.Name = "btnSetPlcIP";
            this.btnSetPlcIP.Size = new System.Drawing.Size(130, 23);
            this.btnSetPlcIP.TabIndex = 0;
            this.btnSetPlcIP.Text = "设置";
            this.btnSetPlcIP.UseVisualStyleBackColor = true;
            this.btnSetPlcIP.Click += new System.EventHandler(this.btnSetPlcIP_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(393, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "mm";
            // 
            // wtestplc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 407);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.tabControl1);
            this.Name = "wtestplc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "测试plc信号";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtPos)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtChannel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtCachePosGet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtCachePosSave)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnWriteSignal;
        private System.Windows.Forms.Button btnReadCacheSignal;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.NumericUpDown ntxtPos;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxJob;
        private System.Windows.Forms.NumericUpDown ntxtChannel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txDiameter;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown ntxtCachePosGet;
        private System.Windows.Forms.NumericUpDown ntxtCachePosSave;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnPushAside;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TextBox txPlcIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSetPlcIP;
        private System.Windows.Forms.Label label4;
    }
}