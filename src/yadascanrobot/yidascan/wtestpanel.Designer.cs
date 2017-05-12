namespace yidascan {
    partial class wtestpanel {
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
            this.components = new System.ComponentModel.Container();
            this.btnStartAllSignals = new System.Windows.Forms.Button();
            this.btnSignalWeigh = new System.Windows.Forms.Button();
            this.btnSignalCache = new System.Windows.Forms.Button();
            this.btnSignalLableUp = new System.Windows.Forms.Button();
            this.btnSignalCatchA = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lbWeigth = new System.Windows.Forms.Label();
            this.lbCache = new System.Windows.Forms.Label();
            this.lbLableUp = new System.Windows.Forms.Label();
            this.lbCatchA = new System.Windows.Forms.Label();
            this.lbCatchB = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnStopAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartAllSignals
            // 
            this.btnStartAllSignals.Location = new System.Drawing.Point(38, 295);
            this.btnStartAllSignals.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnStartAllSignals.Name = "btnStartAllSignals";
            this.btnStartAllSignals.Size = new System.Drawing.Size(154, 36);
            this.btnStartAllSignals.TabIndex = 0;
            this.btnStartAllSignals.Text = "启动全部信号";
            this.btnStartAllSignals.UseVisualStyleBackColor = true;
            this.btnStartAllSignals.Click += new System.EventHandler(this.btnStartAllSignals_Click);
            // 
            // btnSignalWeigh
            // 
            this.btnSignalWeigh.Location = new System.Drawing.Point(38, 19);
            this.btnSignalWeigh.Name = "btnSignalWeigh";
            this.btnSignalWeigh.Size = new System.Drawing.Size(125, 36);
            this.btnSignalWeigh.TabIndex = 1;
            this.btnSignalWeigh.Text = "称重信号";
            this.btnSignalWeigh.UseVisualStyleBackColor = true;
            this.btnSignalWeigh.Click += new System.EventHandler(this.btnSignalWeigh_Click);
            // 
            // btnSignalCache
            // 
            this.btnSignalCache.Location = new System.Drawing.Point(38, 71);
            this.btnSignalCache.Name = "btnSignalCache";
            this.btnSignalCache.Size = new System.Drawing.Size(125, 36);
            this.btnSignalCache.TabIndex = 2;
            this.btnSignalCache.Text = "缓存信号";
            this.btnSignalCache.UseVisualStyleBackColor = true;
            this.btnSignalCache.Click += new System.EventHandler(this.btnSignalCache_Click);
            // 
            // btnSignalLableUp
            // 
            this.btnSignalLableUp.Location = new System.Drawing.Point(38, 124);
            this.btnSignalLableUp.Name = "btnSignalLableUp";
            this.btnSignalLableUp.Size = new System.Drawing.Size(125, 36);
            this.btnSignalLableUp.TabIndex = 3;
            this.btnSignalLableUp.Text = "标签朝上";
            this.btnSignalLableUp.UseVisualStyleBackColor = true;
            this.btnSignalLableUp.Click += new System.EventHandler(this.btnSignalLableUp_Click);
            // 
            // btnSignalCatchA
            // 
            this.btnSignalCatchA.Location = new System.Drawing.Point(38, 179);
            this.btnSignalCatchA.Name = "btnSignalCatchA";
            this.btnSignalCatchA.Size = new System.Drawing.Size(125, 36);
            this.btnSignalCatchA.TabIndex = 4;
            this.btnSignalCatchA.Text = "抓料处A";
            this.btnSignalCatchA.UseVisualStyleBackColor = true;
            this.btnSignalCatchA.Click += new System.EventHandler(this.btnSignalCatchA_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(38, 235);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 36);
            this.button1.TabIndex = 5;
            this.button1.Text = "抓料处B";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbWeigth
            // 
            this.lbWeigth.AutoSize = true;
            this.lbWeigth.Location = new System.Drawing.Point(219, 29);
            this.lbWeigth.Name = "lbWeigth";
            this.lbWeigth.Size = new System.Drawing.Size(39, 19);
            this.lbWeigth.TabIndex = 6;
            this.lbWeigth.Text = "off";
            // 
            // lbCache
            // 
            this.lbCache.AutoSize = true;
            this.lbCache.Location = new System.Drawing.Point(219, 80);
            this.lbCache.Name = "lbCache";
            this.lbCache.Size = new System.Drawing.Size(39, 19);
            this.lbCache.TabIndex = 7;
            this.lbCache.Text = "off";
            // 
            // lbLableUp
            // 
            this.lbLableUp.AutoSize = true;
            this.lbLableUp.Location = new System.Drawing.Point(219, 133);
            this.lbLableUp.Name = "lbLableUp";
            this.lbLableUp.Size = new System.Drawing.Size(39, 19);
            this.lbLableUp.TabIndex = 8;
            this.lbLableUp.Text = "off";
            // 
            // lbCatchA
            // 
            this.lbCatchA.AutoSize = true;
            this.lbCatchA.Location = new System.Drawing.Point(219, 188);
            this.lbCatchA.Name = "lbCatchA";
            this.lbCatchA.Size = new System.Drawing.Size(39, 19);
            this.lbCatchA.TabIndex = 9;
            this.lbCatchA.Text = "off";
            // 
            // lbCatchB
            // 
            this.lbCatchB.AutoSize = true;
            this.lbCatchB.Location = new System.Drawing.Point(219, 244);
            this.lbCatchB.Name = "lbCatchB";
            this.lbCatchB.Size = new System.Drawing.Size(39, 19);
            this.lbCatchB.TabIndex = 10;
            this.lbCatchB.Text = "off";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnStopAll
            // 
            this.btnStopAll.Location = new System.Drawing.Point(232, 295);
            this.btnStopAll.Margin = new System.Windows.Forms.Padding(5);
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(154, 36);
            this.btnStopAll.TabIndex = 11;
            this.btnStopAll.Text = "关闭全部信号";
            this.btnStopAll.UseVisualStyleBackColor = true;
            this.btnStopAll.Click += new System.EventHandler(this.btnStopAll_Click);
            // 
            // wtestpanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 345);
            this.Controls.Add(this.btnStopAll);
            this.Controls.Add(this.lbCatchB);
            this.Controls.Add(this.lbCatchA);
            this.Controls.Add(this.lbLableUp);
            this.Controls.Add(this.lbCache);
            this.Controls.Add(this.lbWeigth);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSignalCatchA);
            this.Controls.Add(this.btnSignalLableUp);
            this.Controls.Add(this.btnSignalCache);
            this.Controls.Add(this.btnSignalWeigh);
            this.Controls.Add(this.btnStartAllSignals);
            this.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wtestpanel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "wtestpanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartAllSignals;
        private System.Windows.Forms.Button btnSignalWeigh;
        private System.Windows.Forms.Button btnSignalCache;
        private System.Windows.Forms.Button btnSignalLableUp;
        private System.Windows.Forms.Button btnSignalCatchA;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbWeigth;
        private System.Windows.Forms.Label lbCache;
        private System.Windows.Forms.Label lbLableUp;
        private System.Windows.Forms.Label lbCatchA;
        private System.Windows.Forms.Label lbCatchB;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnStopAll;
    }
}