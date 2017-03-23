namespace yidascan {
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
            this.mtxCode = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "号码";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(349, 30);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 30);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "查找";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lbx
            // 
            this.lbx.FormattingEnabled = true;
            this.lbx.ItemHeight = 19;
            this.lbx.Location = new System.Drawing.Point(28, 90);
            this.lbx.Name = "lbx";
            this.lbx.Size = new System.Drawing.Size(442, 213);
            this.lbx.TabIndex = 3;
            // 
            // mtxCode
            // 
            this.mtxCode.Location = new System.Drawing.Point(79, 30);
            this.mtxCode.Mask = "0000 0000 0000";
            this.mtxCode.Name = "mtxCode";
            this.mtxCode.Size = new System.Drawing.Size(251, 29);
            this.mtxCode.TabIndex = 4;
            // 
            // wfind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 325);
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
        private System.Windows.Forms.MaskedTextBox mtxCode;
    }
}