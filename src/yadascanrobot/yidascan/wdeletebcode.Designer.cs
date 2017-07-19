namespace yidascan {
    partial class wdeletebcode {
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
            this.label2 = new System.Windows.Forms.Label();
            this.lbxLog = new System.Windows.Forms.ListBox();
            this.txtLabelCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pageSingle = new System.Windows.Forms.TabPage();
            this.pageQueues = new System.Windows.Forms.TabPage();
            this.ckWeighQue = new System.Windows.Forms.CheckBox();
            this.ckBeforeCache = new System.Windows.Forms.CheckBox();
            this.ckCache = new System.Windows.Forms.CheckBox();
            this.ckLableUp = new System.Windows.Forms.CheckBox();
            this.ckCatchQue = new System.Windows.Forms.CheckBox();
            this.ckRobotQue = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.pageSingle.SuspendLayout();
            this.pageQueues.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(8, 150);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 19);
            this.label2.TabIndex = 13;
            this.label2.Text = "消息栏";
            // 
            // lbxLog
            // 
            this.lbxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxLog.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbxLog.FormattingEnabled = true;
            this.lbxLog.ItemHeight = 19;
            this.lbxLog.Location = new System.Drawing.Point(12, 191);
            this.lbxLog.Margin = new System.Windows.Forms.Padding(5);
            this.lbxLog.Name = "lbxLog";
            this.lbxLog.Size = new System.Drawing.Size(526, 232);
            this.lbxLog.TabIndex = 11;
            // 
            // txtLabelCode
            // 
            this.txtLabelCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLabelCode.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLabelCode.Location = new System.Drawing.Point(123, 28);
            this.txtLabelCode.Margin = new System.Windows.Forms.Padding(5);
            this.txtLabelCode.Name = "txtLabelCode";
            this.txtLabelCode.Size = new System.Drawing.Size(418, 29);
            this.txtLabelCode.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(15, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 19);
            this.label1.TabIndex = 9;
            this.label1.Text = "标签号码";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(548, 243);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(119, 42);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDelete.ForeColor = System.Drawing.Color.Red;
            this.btnDelete.Location = new System.Drawing.Point(548, 191);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(119, 42);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.pageSingle);
            this.tabControl1.Controls.Add(this.pageQueues);
            this.tabControl1.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(655, 123);
            this.tabControl1.TabIndex = 14;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // pageSingle
            // 
            this.pageSingle.Controls.Add(this.label1);
            this.pageSingle.Controls.Add(this.txtLabelCode);
            this.pageSingle.Location = new System.Drawing.Point(4, 29);
            this.pageSingle.Name = "pageSingle";
            this.pageSingle.Padding = new System.Windows.Forms.Padding(3);
            this.pageSingle.Size = new System.Drawing.Size(647, 90);
            this.pageSingle.TabIndex = 0;
            this.pageSingle.Text = "删除号码";
            this.pageSingle.UseVisualStyleBackColor = true;
            // 
            // pageQueues
            // 
            this.pageQueues.Controls.Add(this.ckRobotQue);
            this.pageQueues.Controls.Add(this.ckCatchQue);
            this.pageQueues.Controls.Add(this.ckLableUp);
            this.pageQueues.Controls.Add(this.ckCache);
            this.pageQueues.Controls.Add(this.ckBeforeCache);
            this.pageQueues.Controls.Add(this.ckWeighQue);
            this.pageQueues.Location = new System.Drawing.Point(4, 29);
            this.pageQueues.Name = "pageQueues";
            this.pageQueues.Padding = new System.Windows.Forms.Padding(3);
            this.pageQueues.Size = new System.Drawing.Size(647, 90);
            this.pageQueues.TabIndex = 1;
            this.pageQueues.Text = "一键清空队列";
            this.pageQueues.UseVisualStyleBackColor = true;
            // 
            // ckWeighQue
            // 
            this.ckWeighQue.AutoSize = true;
            this.ckWeighQue.Checked = true;
            this.ckWeighQue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckWeighQue.Location = new System.Drawing.Point(23, 18);
            this.ckWeighQue.Name = "ckWeighQue";
            this.ckWeighQue.Size = new System.Drawing.Size(104, 23);
            this.ckWeighQue.TabIndex = 0;
            this.ckWeighQue.Text = "称重队列";
            this.ckWeighQue.UseVisualStyleBackColor = true;
            // 
            // ckBeforeCache
            // 
            this.ckBeforeCache.AutoSize = true;
            this.ckBeforeCache.Checked = true;
            this.ckBeforeCache.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckBeforeCache.Location = new System.Drawing.Point(154, 18);
            this.ckBeforeCache.Name = "ckBeforeCache";
            this.ckBeforeCache.Size = new System.Drawing.Size(123, 23);
            this.ckBeforeCache.TabIndex = 1;
            this.ckBeforeCache.Text = "缓存前队列";
            this.ckBeforeCache.UseVisualStyleBackColor = true;
            // 
            // ckCache
            // 
            this.ckCache.AutoSize = true;
            this.ckCache.Checked = true;
            this.ckCache.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckCache.Location = new System.Drawing.Point(304, 18);
            this.ckCache.Name = "ckCache";
            this.ckCache.Size = new System.Drawing.Size(85, 23);
            this.ckCache.TabIndex = 2;
            this.ckCache.Text = "缓存位";
            this.ckCache.UseVisualStyleBackColor = true;
            // 
            // ckLableUp
            // 
            this.ckLableUp.AutoSize = true;
            this.ckLableUp.Checked = true;
            this.ckLableUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckLableUp.Location = new System.Drawing.Point(416, 18);
            this.ckLableUp.Name = "ckLableUp";
            this.ckLableUp.Size = new System.Drawing.Size(142, 23);
            this.ckLableUp.TabIndex = 3;
            this.ckLableUp.Text = "标签朝上队列";
            this.ckLableUp.UseVisualStyleBackColor = true;
            // 
            // ckCatchQue
            // 
            this.ckCatchQue.AutoSize = true;
            this.ckCatchQue.Checked = true;
            this.ckCatchQue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckCatchQue.Location = new System.Drawing.Point(23, 53);
            this.ckCatchQue.Name = "ckCatchQue";
            this.ckCatchQue.Size = new System.Drawing.Size(104, 23);
            this.ckCatchQue.TabIndex = 4;
            this.ckCatchQue.Text = "抓料队列";
            this.ckCatchQue.UseVisualStyleBackColor = true;
            // 
            // ckRobotQue
            // 
            this.ckRobotQue.AutoSize = true;
            this.ckRobotQue.Checked = true;
            this.ckRobotQue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckRobotQue.Location = new System.Drawing.Point(154, 53);
            this.ckRobotQue.Name = "ckRobotQue";
            this.ckRobotQue.Size = new System.Drawing.Size(123, 23);
            this.ckRobotQue.TabIndex = 5;
            this.ckRobotQue.Text = "机器人队列";
            this.ckRobotQue.UseVisualStyleBackColor = true;
            // 
            // wdeletebcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 444);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbxLog);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDelete);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wdeletebcode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "删除B区号码";
            this.tabControl1.ResumeLayout(false);
            this.pageSingle.ResumeLayout(false);
            this.pageSingle.PerformLayout();
            this.pageQueues.ResumeLayout(false);
            this.pageQueues.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbxLog;
        private System.Windows.Forms.TextBox txtLabelCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pageSingle;
        private System.Windows.Forms.TabPage pageQueues;
        private System.Windows.Forms.CheckBox ckWeighQue;
        private System.Windows.Forms.CheckBox ckCache;
        private System.Windows.Forms.CheckBox ckBeforeCache;
        private System.Windows.Forms.CheckBox ckLableUp;
        private System.Windows.Forms.CheckBox ckRobotQue;
        private System.Windows.Forms.CheckBox ckCatchQue;
    }
}