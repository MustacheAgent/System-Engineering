
namespace TCP_Server__WinForms_
{
    partial class TCPServer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtAddress = new System.Windows.Forms.TextBox();
            this.TxtPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtRichMessage = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BtnConnect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BtnSend = new System.Windows.Forms.Button();
            this.BarServer = new System.Windows.Forms.StatusStrip();
            this.TimerStatus = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TxtRichLog = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP-адрес";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtAddress
            // 
            this.TxtAddress.Location = new System.Drawing.Point(101, 27);
            this.TxtAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtAddress.Name = "TxtAddress";
            this.TxtAddress.Size = new System.Drawing.Size(114, 27);
            this.TxtAddress.TabIndex = 1;
            // 
            // TxtPort
            // 
            this.TxtPort.Location = new System.Drawing.Point(101, 65);
            this.TxtPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtPort.Name = "TxtPort";
            this.TxtPort.Size = new System.Drawing.Size(114, 27);
            this.TxtPort.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 31);
            this.label2.TabIndex = 3;
            this.label2.Text = "Порт";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtRichMessage
            // 
            this.TxtRichMessage.Location = new System.Drawing.Point(7, 60);
            this.TxtRichMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtRichMessage.Name = "TxtRichMessage";
            this.TxtRichMessage.Size = new System.Drawing.Size(216, 127);
            this.TxtRichMessage.TabIndex = 5;
            this.TxtRichMessage.Text = "";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(7, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(208, 31);
            this.label3.TabIndex = 6;
            this.label3.Text = "Отправляемое сообщение";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnConnect
            // 
            this.BtnConnect.Location = new System.Drawing.Point(101, 104);
            this.BtnConnect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(114, 31);
            this.BtnConnect.TabIndex = 7;
            this.BtnConnect.Text = "Запустить";
            this.BtnConnect.UseVisualStyleBackColor = true;
            this.BtnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.BtnConnect);
            this.groupBox1.Controls.Add(this.TxtAddress);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.TxtPort);
            this.groupBox1.Location = new System.Drawing.Point(14, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(229, 147);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Подключение";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BtnSend);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.TxtRichMessage);
            this.groupBox2.Location = new System.Drawing.Point(249, 16);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(229, 243);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Сообщение";
            // 
            // BtnSend
            // 
            this.BtnSend.Location = new System.Drawing.Point(7, 196);
            this.BtnSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnSend.Name = "BtnSend";
            this.BtnSend.Size = new System.Drawing.Size(216, 31);
            this.BtnSend.TabIndex = 8;
            this.BtnSend.Text = "Отправить";
            this.BtnSend.UseVisualStyleBackColor = true;
            // 
            // BarServer
            // 
            this.BarServer.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.BarServer.Location = new System.Drawing.Point(0, 443);
            this.BarServer.Name = "BarServer";
            this.BarServer.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.BarServer.Size = new System.Drawing.Size(492, 22);
            this.BarServer.TabIndex = 10;
            this.BarServer.Text = "statusStrip1";
            // 
            // TimerStatus
            // 
            this.TimerStatus.Tick += new System.EventHandler(this.TimerStatus_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TxtRichLog);
            this.groupBox3.Location = new System.Drawing.Point(21, 267);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(457, 173);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Лог";
            // 
            // TxtRichLog
            // 
            this.TxtRichLog.Location = new System.Drawing.Point(6, 28);
            this.TxtRichLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtRichLog.Name = "TxtRichLog";
            this.TxtRichLog.Size = new System.Drawing.Size(445, 127);
            this.TxtRichLog.TabIndex = 5;
            this.TxtRichLog.Text = "";
            // 
            // TCPServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 465);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.BarServer);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TCPServer";
            this.Text = "TCP сервер";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtAddress;
        private System.Windows.Forms.TextBox TxtPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox TxtRichMessage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BtnConnect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BtnSend;
        private System.Windows.Forms.Timer TimerStatus;
        private System.Windows.Forms.StatusStrip BarServer;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox TxtRichLog;
    }
}

