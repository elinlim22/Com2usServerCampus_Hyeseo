namespace myForm
{
    partial class mainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            label_ID = new Label();
            label_PW = new Label();
            label_Token = new Label();
            textBox_ID = new TextBox();
            textBox_PW = new TextBox();
            textBox_Token = new TextBox();
            button_Regist = new Button();
            button_Login = new Button();
            label_IP = new Label();
            textBox_IP = new TextBox();
            label_Port = new Label();
            textBox_Port = new TextBox();
            checkBox_useLocalHost = new CheckBox();
            groupBox1 = new GroupBox();
            button_Match = new Button();
            groupBox2 = new GroupBox();
            button_Disconnect = new Button();
            button_Connect = new Button();
            groupBox3 = new GroupBox();
            button_Leave = new Button();
            button_Ready = new Button();
            textBox_RoomNumber = new TextBox();
            label_RoomNumber = new Label();
            button_Chat = new Button();
            textBox_Chat = new TextBox();
            RoomChatMsg = new ListBox();
            UserList = new ListBox();
            listBox_Log = new ListBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.MediumPurple;
            panel1.Location = new Point(579, 8);
            panel1.Margin = new Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new Size(607, 770);
            panel1.TabIndex = 0;
            panel1.Paint += panel1_Paint;
            // 
            // label_ID
            // 
            label_ID.AutoSize = true;
            label_ID.Location = new Point(13, 41);
            label_ID.Margin = new Padding(2, 0, 2, 0);
            label_ID.Name = "label_ID";
            label_ID.Size = new Size(19, 15);
            label_ID.TabIndex = 1;
            label_ID.Text = "ID";
            // 
            // label_PW
            // 
            label_PW.AutoSize = true;
            label_PW.Location = new Point(13, 76);
            label_PW.Margin = new Padding(2, 0, 2, 0);
            label_PW.Name = "label_PW";
            label_PW.Size = new Size(25, 15);
            label_PW.TabIndex = 2;
            label_PW.Text = "PW";
            // 
            // label_Token
            // 
            label_Token.AutoSize = true;
            label_Token.Location = new Point(13, 111);
            label_Token.Margin = new Padding(2, 0, 2, 0);
            label_Token.Name = "label_Token";
            label_Token.Size = new Size(39, 15);
            label_Token.TabIndex = 3;
            label_Token.Text = "Token";
            // 
            // textBox_ID
            // 
            textBox_ID.Location = new Point(68, 38);
            textBox_ID.Margin = new Padding(2);
            textBox_ID.Name = "textBox_ID";
            textBox_ID.Size = new Size(184, 23);
            textBox_ID.TabIndex = 4;
            // 
            // textBox_PW
            // 
            textBox_PW.Location = new Point(68, 73);
            textBox_PW.Margin = new Padding(2);
            textBox_PW.Name = "textBox_PW";
            textBox_PW.Size = new Size(184, 23);
            textBox_PW.TabIndex = 5;
            // 
            // textBox_Token
            // 
            textBox_Token.BackColor = SystemColors.ButtonFace;
            textBox_Token.Enabled = false;
            textBox_Token.Location = new Point(68, 108);
            textBox_Token.Margin = new Padding(2);
            textBox_Token.Name = "textBox_Token";
            textBox_Token.ReadOnly = true;
            textBox_Token.Size = new Size(184, 23);
            textBox_Token.TabIndex = 6;
            textBox_Token.TextChanged += textBox_Token_TextChanged;
            // 
            // button_Regist
            // 
            button_Regist.Location = new Point(68, 146);
            button_Regist.Margin = new Padding(2);
            button_Regist.Name = "button_Regist";
            button_Regist.Size = new Size(83, 31);
            button_Regist.TabIndex = 7;
            button_Regist.Text = "회원가입";
            button_Regist.UseVisualStyleBackColor = true;
            button_Regist.Click += button_Regist_Click;
            // 
            // button_Login
            // 
            button_Login.Location = new Point(169, 146);
            button_Login.Margin = new Padding(2);
            button_Login.Name = "button_Login";
            button_Login.Size = new Size(83, 31);
            button_Login.TabIndex = 8;
            button_Login.Text = "로그인";
            button_Login.UseVisualStyleBackColor = true;
            button_Login.Click += button_Login_Click;
            // 
            // label_IP
            // 
            label_IP.AutoSize = true;
            label_IP.Location = new Point(15, 53);
            label_IP.Margin = new Padding(2, 0, 2, 0);
            label_IP.Name = "label_IP";
            label_IP.Size = new Size(17, 15);
            label_IP.TabIndex = 9;
            label_IP.Text = "IP";
            // 
            // textBox_IP
            // 
            textBox_IP.BackColor = SystemColors.ButtonFace;
            textBox_IP.Location = new Point(63, 53);
            textBox_IP.Margin = new Padding(2);
            textBox_IP.Name = "textBox_IP";
            textBox_IP.ReadOnly = true;
            textBox_IP.Size = new Size(184, 23);
            textBox_IP.TabIndex = 10;
            // 
            // label_Port
            // 
            label_Port.AutoSize = true;
            label_Port.Location = new Point(15, 89);
            label_Port.Margin = new Padding(2, 0, 2, 0);
            label_Port.Name = "label_Port";
            label_Port.Size = new Size(29, 15);
            label_Port.TabIndex = 11;
            label_Port.Text = "Port";
            // 
            // textBox_Port
            // 
            textBox_Port.BackColor = SystemColors.ButtonFace;
            textBox_Port.Location = new Point(63, 86);
            textBox_Port.Margin = new Padding(2);
            textBox_Port.Name = "textBox_Port";
            textBox_Port.ReadOnly = true;
            textBox_Port.Size = new Size(184, 23);
            textBox_Port.TabIndex = 12;
            // 
            // checkBox_useLocalHost
            // 
            checkBox_useLocalHost.AutoSize = true;
            checkBox_useLocalHost.Location = new Point(145, 20);
            checkBox_useLocalHost.Margin = new Padding(2);
            checkBox_useLocalHost.Name = "checkBox_useLocalHost";
            checkBox_useLocalHost.Size = new Size(102, 19);
            checkBox_useLocalHost.TabIndex = 13;
            checkBox_useLocalHost.Text = "localhost 사용";
            checkBox_useLocalHost.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button_Login);
            groupBox1.Controls.Add(button_Match);
            groupBox1.Controls.Add(textBox_Token);
            groupBox1.Controls.Add(button_Regist);
            groupBox1.Controls.Add(label_Token);
            groupBox1.Controls.Add(label_ID);
            groupBox1.Controls.Add(label_PW);
            groupBox1.Controls.Add(textBox_ID);
            groupBox1.Controls.Add(textBox_PW);
            groupBox1.Location = new Point(6, 11);
            groupBox1.Margin = new Padding(2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(2);
            groupBox1.Size = new Size(287, 276);
            groupBox1.TabIndex = 14;
            groupBox1.TabStop = false;
            groupBox1.Text = "Login";
            // 
            // button_Match
            // 
            button_Match.BackColor = Color.Gold;
            button_Match.Enabled = false;
            button_Match.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            button_Match.Location = new Point(68, 193);
            button_Match.Margin = new Padding(2, 1, 2, 1);
            button_Match.Name = "button_Match";
            button_Match.Size = new Size(184, 57);
            button_Match.TabIndex = 16;
            button_Match.Text = "매칭";
            button_Match.UseVisualStyleBackColor = false;
            button_Match.Click += button_Match_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(button_Disconnect);
            groupBox2.Controls.Add(checkBox_useLocalHost);
            groupBox2.Controls.Add(textBox_Port);
            groupBox2.Controls.Add(button_Connect);
            groupBox2.Controls.Add(label_Port);
            groupBox2.Controls.Add(textBox_IP);
            groupBox2.Controls.Add(label_IP);
            groupBox2.Location = new Point(297, 11);
            groupBox2.Margin = new Padding(2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(2);
            groupBox2.Size = new Size(278, 276);
            groupBox2.TabIndex = 15;
            groupBox2.TabStop = false;
            groupBox2.Text = "Server";
            // 
            // button_Disconnect
            // 
            button_Disconnect.Enabled = false;
            button_Disconnect.Location = new Point(164, 122);
            button_Disconnect.Margin = new Padding(2);
            button_Disconnect.Name = "button_Disconnect";
            button_Disconnect.Size = new Size(83, 30);
            button_Disconnect.TabIndex = 15;
            button_Disconnect.Text = "끊기";
            button_Disconnect.UseVisualStyleBackColor = true;
            button_Disconnect.Click += button_Disconnect_Click;
            // 
            // button_Connect
            // 
            button_Connect.Location = new Point(63, 122);
            button_Connect.Margin = new Padding(2);
            button_Connect.Name = "button_Connect";
            button_Connect.Size = new Size(83, 31);
            button_Connect.TabIndex = 14;
            button_Connect.Text = "접속";
            button_Connect.UseVisualStyleBackColor = true;
            button_Connect.Click += button_Connect_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(button_Leave);
            groupBox3.Controls.Add(button_Ready);
            groupBox3.Controls.Add(textBox_RoomNumber);
            groupBox3.Controls.Add(label_RoomNumber);
            groupBox3.Controls.Add(button_Chat);
            groupBox3.Controls.Add(textBox_Chat);
            groupBox3.Controls.Add(RoomChatMsg);
            groupBox3.Controls.Add(UserList);
            groupBox3.Location = new Point(6, 291);
            groupBox3.Margin = new Padding(2);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(2);
            groupBox3.Size = new Size(569, 254);
            groupBox3.TabIndex = 16;
            groupBox3.TabStop = false;
            groupBox3.Text = "Room";
            // 
            // button_Leave
            // 
            button_Leave.Enabled = false;
            button_Leave.Location = new Point(452, 26);
            button_Leave.Margin = new Padding(2, 1, 2, 1);
            button_Leave.Name = "button_Leave";
            button_Leave.Size = new Size(107, 30);
            button_Leave.TabIndex = 7;
            button_Leave.Text = "LEAVE";
            button_Leave.UseVisualStyleBackColor = true;
            button_Leave.Click += button_Leave_Click;
            // 
            // button_Ready
            // 
            button_Ready.BackColor = Color.Gold;
            button_Ready.Enabled = false;
            button_Ready.Location = new Point(330, 26);
            button_Ready.Margin = new Padding(2, 1, 2, 1);
            button_Ready.Name = "button_Ready";
            button_Ready.Size = new Size(107, 30);
            button_Ready.TabIndex = 6;
            button_Ready.Text = "READY";
            button_Ready.UseVisualStyleBackColor = false;
            button_Ready.Click += button_Ready_Click;
            // 
            // textBox_RoomNumber
            // 
            textBox_RoomNumber.BackColor = SystemColors.ButtonFace;
            textBox_RoomNumber.Enabled = false;
            textBox_RoomNumber.Location = new Point(60, 31);
            textBox_RoomNumber.Margin = new Padding(2, 1, 2, 1);
            textBox_RoomNumber.Name = "textBox_RoomNumber";
            textBox_RoomNumber.ReadOnly = true;
            textBox_RoomNumber.Size = new Size(56, 23);
            textBox_RoomNumber.TabIndex = 5;
            // 
            // label_RoomNumber
            // 
            label_RoomNumber.AutoSize = true;
            label_RoomNumber.Location = new Point(13, 34);
            label_RoomNumber.Margin = new Padding(2, 0, 2, 0);
            label_RoomNumber.Name = "label_RoomNumber";
            label_RoomNumber.Size = new Size(43, 15);
            label_RoomNumber.TabIndex = 4;
            label_RoomNumber.Text = "방번호";
            // 
            // button_Chat
            // 
            button_Chat.Enabled = false;
            button_Chat.Location = new Point(497, 210);
            button_Chat.Margin = new Padding(2);
            button_Chat.Name = "button_Chat";
            button_Chat.Size = new Size(62, 26);
            button_Chat.TabIndex = 3;
            button_Chat.Text = "채팅";
            button_Chat.UseVisualStyleBackColor = true;
            button_Chat.Click += button_Chat_Click;
            // 
            // textBox_Chat
            // 
            textBox_Chat.Location = new Point(13, 213);
            textBox_Chat.Margin = new Padding(2);
            textBox_Chat.Name = "textBox_Chat";
            textBox_Chat.Size = new Size(480, 23);
            textBox_Chat.TabIndex = 2;
            textBox_Chat.TextChanged += textBox_Chat_TextChanged;
            // 
            // RoomChatMsg
            // 
            RoomChatMsg.FormattingEnabled = true;
            RoomChatMsg.ItemHeight = 15;
            RoomChatMsg.Location = new Point(125, 67);
            RoomChatMsg.Margin = new Padding(2);
            RoomChatMsg.Name = "RoomChatMsg";
            RoomChatMsg.Size = new Size(434, 139);
            RoomChatMsg.TabIndex = 1;
            // 
            // UserList
            // 
            UserList.FormattingEnabled = true;
            UserList.ItemHeight = 15;
            UserList.Location = new Point(13, 67);
            UserList.Margin = new Padding(2);
            UserList.Name = "UserList";
            UserList.Size = new Size(103, 139);
            UserList.TabIndex = 0;
            // 
            // listBox_Log
            // 
            listBox_Log.FormattingEnabled = true;
            listBox_Log.HorizontalScrollbar = true;
            listBox_Log.ItemHeight = 15;
            listBox_Log.Location = new Point(6, 549);
            listBox_Log.Margin = new Padding(2);
            listBox_Log.Name = "listBox_Log";
            listBox_Log.Size = new Size(569, 229);
            listBox_Log.TabIndex = 17;
            // 
            // mainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1195, 787);
            Controls.Add(groupBox2);
            Controls.Add(listBox_Log);
            Controls.Add(groupBox3);
            Controls.Add(panel1);
            Controls.Add(groupBox1);
            Margin = new Padding(2);
            Name = "mainForm";
            Text = "Purple Omok :)";
            FormClosing += mainForm_FormClosing;
            Load += mainForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_ID;
        private System.Windows.Forms.Label label_PW;
        private System.Windows.Forms.Label label_Token;
        private System.Windows.Forms.TextBox textBox_ID;
        private System.Windows.Forms.TextBox textBox_PW;
        private System.Windows.Forms.TextBox textBox_Token;
        private System.Windows.Forms.Button button_Regist;
        private System.Windows.Forms.Button button_Login;
        private System.Windows.Forms.Label label_IP;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.Label label_Port;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.CheckBox checkBox_useLocalHost;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_Chat;
        private System.Windows.Forms.TextBox textBox_Chat;
        private System.Windows.Forms.ListBox RoomChatMsg;
        private System.Windows.Forms.ListBox UserList;
        private System.Windows.Forms.ListBox listBox_Log;
        private System.Windows.Forms.Button button_Disconnect;
        private TextBox textBox_RoomNumber;
        private Label label_RoomNumber;
        private Button button_Ready;
        private Button button_Leave;
        private Button button_Match;
    }
}

