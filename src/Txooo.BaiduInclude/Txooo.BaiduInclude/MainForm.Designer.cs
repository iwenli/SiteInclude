namespace Txooo.BaiduInclude
{
    partial class MainForm
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.st = new System.Windows.Forms.StatusStrip();
            this.stProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsTxtUrl = new System.Windows.Forms.ToolStripTextBox();
            this.tsBtnGoWithUrl = new System.Windows.Forms.ToolStripButton();
            this.tsBtnGo = new System.Windows.Forms.ToolStripButton();
            this.ts = new System.Windows.Forms.ToolStrip();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMaxDataCount = new System.Windows.Forms.TextBox();
            this.chkBoxIsDesc = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMaxThreadCount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rchTxtSearchWhere = new System.Windows.Forms.RichTextBox();
            this.st.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.ts.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Control;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.txtLog.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtLog.Location = new System.Drawing.Point(0, 125);
            this.txtLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(676, 240);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "";
            // 
            // st
            // 
            this.st.AutoSize = false;
            this.st.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.st.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stStatus,
            this.stProgress,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.st.Location = new System.Drawing.Point(0, 365);
            this.st.Name = "st";
            this.st.Size = new System.Drawing.Size(676, 30);
            this.st.TabIndex = 3;
            this.st.Text = "statusStrip1";
            // 
            // stProgress
            // 
            this.stProgress.Name = "stProgress";
            this.stProgress.Size = new System.Drawing.Size(250, 24);
            this.stProgress.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(345, 25);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.rchTxtSearchWhere);
            this.groupBox1.Controls.Add(this.chkBoxIsDesc);
            this.groupBox1.Controls.Add(this.txtMaxThreadCount);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtMaxDataCount);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(676, 100);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设置";
            // 
            // stStatus
            // 
            this.stStatus.AutoSize = false;
            this.stStatus.Image = global::Txooo.BaiduInclude.Properties.Resources.info_16;
            this.stStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stStatus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.stStatus.Name = "stStatus";
            this.stStatus.Size = new System.Drawing.Size(239, 25);
            this.stStatus.Text = "就绪";
            this.stStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Image = global::Txooo.BaiduInclude.Properties.Resources.counseling_style_51;
            this.toolStripStatusLabel2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusLabel2.IsLink = true;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(77, 25);
            this.toolStripStatusLabel2.Tag = "http://wpa.qq.com/msgrd?v=3&uin=234486036&site=qq&menu=yes";
            this.toolStripStatusLabel2.Text = "反馈问题";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(71, 22);
            this.toolStripLabel1.Text = "待收录网址:";
            // 
            // tsTxtUrl
            // 
            this.tsTxtUrl.BackColor = System.Drawing.Color.White;
            this.tsTxtUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tsTxtUrl.Name = "tsTxtUrl";
            this.tsTxtUrl.Size = new System.Drawing.Size(300, 25);
            // 
            // tsBtnGoWithUrl
            // 
            this.tsBtnGoWithUrl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnGoWithUrl.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnGoWithUrl.Image")));
            this.tsBtnGoWithUrl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnGoWithUrl.Name = "tsBtnGoWithUrl";
            this.tsBtnGoWithUrl.Size = new System.Drawing.Size(89, 22);
            this.tsBtnGoWithUrl.Text = "检查本连接(&G)";
            this.tsBtnGoWithUrl.ToolTipText = "检查单个连接(输入的)";
            // 
            // tsBtnGo
            // 
            this.tsBtnGo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsBtnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnGo.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnGo.Image")));
            this.tsBtnGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnGo.Name = "tsBtnGo";
            this.tsBtnGo.Size = new System.Drawing.Size(76, 22);
            this.tsBtnGo.Text = "全部检查(&A)";
            // 
            // ts
            // 
            this.ts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tsTxtUrl,
            this.tsBtnGoWithUrl,
            this.tsBtnGo});
            this.ts.Location = new System.Drawing.Point(0, 0);
            this.ts.Name = "ts";
            this.ts.Size = new System.Drawing.Size(676, 25);
            this.ts.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "获取数据数目：";
            // 
            // txtMaxDataCount
            // 
            this.txtMaxDataCount.Location = new System.Drawing.Point(103, 19);
            this.txtMaxDataCount.Name = "txtMaxDataCount";
            this.txtMaxDataCount.Size = new System.Drawing.Size(100, 23);
            this.txtMaxDataCount.TabIndex = 1;
            // 
            // chkBoxIsDesc
            // 
            this.chkBoxIsDesc.AutoSize = true;
            this.chkBoxIsDesc.Location = new System.Drawing.Point(232, 21);
            this.chkBoxIsDesc.Name = "chkBoxIsDesc";
            this.chkBoxIsDesc.Size = new System.Drawing.Size(99, 21);
            this.chkBoxIsDesc.TabIndex = 2;
            this.chkBoxIsDesc.Text = "是否降序获取";
            this.chkBoxIsDesc.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "执行线程数目：";
            // 
            // txtMaxThreadCount
            // 
            this.txtMaxThreadCount.Location = new System.Drawing.Point(103, 57);
            this.txtMaxThreadCount.Name = "txtMaxThreadCount";
            this.txtMaxThreadCount.Size = new System.Drawing.Size(100, 23);
            this.txtMaxThreadCount.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(358, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "附加查询条件：";
            // 
            // rchTxtSearchWhere
            // 
            this.rchTxtSearchWhere.Location = new System.Drawing.Point(448, 19);
            this.rchTxtSearchWhere.Name = "rchTxtSearchWhere";
            this.rchTxtSearchWhere.Size = new System.Drawing.Size(216, 75);
            this.rchTxtSearchWhere.TabIndex = 3;
            this.rchTxtSearchWhere.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 395);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.st);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ts);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.st.ResumeLayout(false);
            this.st.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ts.ResumeLayout(false);
            this.ts.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.StatusStrip st;
        private System.Windows.Forms.ToolStripStatusLabel stStatus;
        private System.Windows.Forms.ToolStripProgressBar stProgress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox tsTxtUrl;
        private System.Windows.Forms.ToolStripButton tsBtnGoWithUrl;
        private System.Windows.Forms.ToolStripButton tsBtnGo;
        private System.Windows.Forms.ToolStrip ts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMaxDataCount;
        private System.Windows.Forms.CheckBox chkBoxIsDesc;
        private System.Windows.Forms.TextBox txtMaxThreadCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rchTxtSearchWhere;
    }
}

