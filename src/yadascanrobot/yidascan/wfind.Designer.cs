﻿namespace yidascan {
    partial class wfind {
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lbx = new System.Windows.Forms.ListBox();
            this.mtxCode = new System.Windows.Forms.TextBox();
            this.ckHistory = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "号码";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(304, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(104, 30);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "查找";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lbx
            // 
            this.lbx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbx.FormattingEnabled = true;
            this.lbx.ItemHeight = 19;
            this.lbx.Location = new System.Drawing.Point(12, 106);
            this.lbx.Name = "lbx";
            this.lbx.Size = new System.Drawing.Size(617, 403);
            this.lbx.TabIndex = 3;
            // 
            // mtxCode
            // 
            this.mtxCode.Location = new System.Drawing.Point(66, 21);
            this.mtxCode.Name = "mtxCode";
            this.mtxCode.Size = new System.Drawing.Size(232, 29);
            this.mtxCode.TabIndex = 0;
            this.mtxCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mtxCode_KeyPress);
            // 
            // ckHistory
            // 
            this.ckHistory.AutoSize = true;
            this.ckHistory.Location = new System.Drawing.Point(15, 67);
            this.ckHistory.Name = "ckHistory";
            this.ckHistory.Size = new System.Drawing.Size(161, 23);
            this.ckHistory.TabIndex = 4;
            this.ckHistory.Text = "从历史数据中查";
            this.ckHistory.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(525, 67);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(104, 30);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "清除显示";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // wfind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 517);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.ckHistory);
            this.Controls.Add(this.mtxCode);
            this.Controls.Add(this.lbx);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wfind";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查找号码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ListBox lbx;
        private System.Windows.Forms.TextBox mtxCode;
        private System.Windows.Forms.CheckBox ckHistory;
        private System.Windows.Forms.Button btnClear;
    }
}