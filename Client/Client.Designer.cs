namespace Client
{
    partial class Client
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
            this.connectBtn = new System.Windows.Forms.Button();
            this.browseBtn = new System.Windows.Forms.Button();
            this.logText = new System.Windows.Forms.RichTextBox();
            this.filePathTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.sendBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(139, 69);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(97, 34);
            this.connectBtn.TabIndex = 0;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // browseBtn
            // 
            this.browseBtn.Enabled = false;
            this.browseBtn.Location = new System.Drawing.Point(139, 158);
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Size = new System.Drawing.Size(97, 34);
            this.browseBtn.TabIndex = 1;
            this.browseBtn.Text = "Browse Files";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // logText
            // 
            this.logText.Location = new System.Drawing.Point(370, 69);
            this.logText.Name = "logText";
            this.logText.Size = new System.Drawing.Size(351, 304);
            this.logText.TabIndex = 2;
            this.logText.Text = "";
            // 
            // filePathTxt
            // 
            this.filePathTxt.Location = new System.Drawing.Point(159, 260);
            this.filePathTxt.Name = "filePathTxt";
            this.filePathTxt.Size = new System.Drawing.Size(162, 22);
            this.filePathTxt.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 260);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Selected file:";
            // 
            // sendBtn
            // 
            this.sendBtn.Enabled = false;
            this.sendBtn.Location = new System.Drawing.Point(139, 339);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(97, 34);
            this.sendBtn.TabIndex = 5;
            this.sendBtn.Text = "Send File";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.filePathTxt);
            this.Controls.Add(this.logText);
            this.Controls.Add(this.browseBtn);
            this.Controls.Add(this.connectBtn);
            this.Name = "Client";
            this.Text = "Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.RichTextBox logText;
        private System.Windows.Forms.TextBox filePathTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sendBtn;
    }
}

