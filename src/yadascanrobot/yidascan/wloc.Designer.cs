namespace yidascan {
    partial class wloc {
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnReset = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txVirtual = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOnFull = new System.Windows.Forms.Button();
            this.txOnFull = new System.Windows.Forms.TextBox();
            this.btnOnReady = new System.Windows.Forms.Button();
            this.btnGet = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(427, 246);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "名义交地";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "实际交地";
            this.columnHeader2.Width = 120;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(463, 22);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "复位";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(463, 61);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "关闭窗口";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // txVirtual
            // 
            this.txVirtual.Location = new System.Drawing.Point(463, 121);
            this.txVirtual.Name = "txVirtual";
            this.txVirtual.ReadOnly = true;
            this.txVirtual.Size = new System.Drawing.Size(100, 21);
            this.txVirtual.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(463, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "名义交地";
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader3});
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(12, 264);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(427, 205);
            this.listView2.TabIndex = 9;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView2_SelectedIndexChanged);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "交地";
            this.columnHeader5.Width = 120;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "交地状态";
            this.columnHeader6.Width = 120;
            // 
            // btnOnFull
            // 
            this.btnOnFull.Location = new System.Drawing.Point(465, 265);
            this.btnOnFull.Name = "btnOnFull";
            this.btnOnFull.Size = new System.Drawing.Size(75, 23);
            this.btnOnFull.TabIndex = 10;
            this.btnOnFull.Text = "onfull";
            this.btnOnFull.UseVisualStyleBackColor = true;
            this.btnOnFull.Click += new System.EventHandler(this.btnOnFull_Click);
            // 
            // txOnFull
            // 
            this.txOnFull.Location = new System.Drawing.Point(465, 237);
            this.txOnFull.Name = "txOnFull";
            this.txOnFull.Size = new System.Drawing.Size(100, 21);
            this.txOnFull.TabIndex = 11;
            // 
            // btnOnReady
            // 
            this.btnOnReady.Location = new System.Drawing.Point(465, 294);
            this.btnOnReady.Name = "btnOnReady";
            this.btnOnReady.Size = new System.Drawing.Size(75, 23);
            this.btnOnReady.TabIndex = 13;
            this.btnOnReady.Text = "onready";
            this.btnOnReady.UseVisualStyleBackColor = true;
            this.btnOnReady.Click += new System.EventHandler(this.btnOnReady_Click);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(463, 148);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(75, 23);
            this.btnGet.TabIndex = 14;
            this.btnGet.Text = "get";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(465, 219);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 15;
            this.label2.Text = "实际交地";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "优先级";
            this.columnHeader3.Width = 120;
            // 
            // wloc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 481);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnGet);
            this.Controls.Add(this.btnOnReady);
            this.Controls.Add(this.txOnFull);
            this.Controls.Add(this.btnOnFull);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txVirtual);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.listView1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wloc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "板号状态";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox txVirtual;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button btnOnFull;
        private System.Windows.Forms.TextBox txOnFull;
        private System.Windows.Forms.Button btnOnReady;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}