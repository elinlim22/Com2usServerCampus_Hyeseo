using CSCommon;
using MemoryPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Net;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Newtonsoft.Json;

#pragma warning disable CA1416

namespace myForm
{
    [SupportedOSPlatform("windows10.0.177630")]
    public partial class mainForm : Form
    {
        ClientSimpleTcp Network = new ClientSimpleTcp();

        bool IsNetworkThreadRunning = false;
        bool IsBackGroundProcessRunning = false;

        System.Threading.Thread NetworkReadThread = null;
        System.Threading.Thread NetworkSendThread = null;

        PacketBufferManager PacketBuffer = new PacketBufferManager();
        ConcurrentQueue<byte[]> RecvPacketQueue = new ConcurrentQueue<byte[]>();
        ConcurrentQueue<byte[]> SendPacketQueue = new ConcurrentQueue<byte[]>();

        System.Windows.Forms.Timer dispatcherUITimer = new();
        System.Threading.Timer HeartBeatPingTimer;
        TimerCallback TimerCallback;


        public mainForm()
        {
            InitializeComponent();
            // SetTimer(); >> Login응답 받은 뒤 실행
        }

        void SetTimer()
        {
            TimerCallback = new TimerCallback(HeartBeatPingRequest);
            HeartBeatPingTimer = new System.Threading.Timer(TimerCallback, null, 0, 1000);
        }

        void HeartBeatPingRequest(object state)
        {
            var requestPkt = new HeartBeatPing();
            var packet = MemoryPackSerializer.Serialize(requestPkt);
            PostSendPacket(PacketID.ReqHeartBeat, packet);
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            PacketBuffer.Init((8096 * 10), PacketHeadererInfo.HeadSize, 2048);

            IsNetworkThreadRunning = true;
            NetworkReadThread = new System.Threading.Thread(this.NetworkReadProcess);
            NetworkReadThread.Start();
            NetworkSendThread = new System.Threading.Thread(this.NetworkSendProcess);
            NetworkSendThread.Start();

            IsBackGroundProcessRunning = true;
            dispatcherUITimer.Tick += new EventHandler(BackGroundProcess);
            dispatcherUITimer.Interval = 100;
            dispatcherUITimer.Start();

            SetPacketHandler();


            Omok_Init();
            DevLog.Write("프로그램 시작 !!!", LOG_LEVEL.INFO);
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsNetworkThreadRunning = false;
            IsBackGroundProcessRunning = false;

            Network.Close();
        }

        // 버튼 클릭 이벤트 처리 함수들!
        private void button_Regist_Click(object sender, EventArgs e)
        {
            DevLog.Write($"회원가입 요청:  {textBox_ID.Text}, {textBox_PW.Text}");
            // Hive 서버에 회원가입 요청
            HttpClient client = new();
            var hiveAddr = $"{serverAddr}:{hivePort}/createuser";
            var hiveResponse = client.PostAsJsonAsync(hiveAddr, new { Email = textBox_ID.Text, Password = textBox_PW.Text }).Result;
            var hiveJsonResponse = hiveResponse.Content.ReadAsStringAsync().Result;
            var hiveLoginResponse = JsonConvert.DeserializeObject<CreateUserResponse>(hiveJsonResponse);
            if (hiveLoginResponse.StatusCode != (short)ErrorCode.None)
            {
                DevLog.Write($"회원가입 실패: {hiveLoginResponse.StatusCode}");
                return;
            }
            DevLog.Write($"회원가입 성공: {textBox_ID.Text}");
            textBox_PW.Text = "";
        }

        private void button_Login_Click(object sender, EventArgs e)
        {
            DevLog.Write($"로그인 요청:  {textBox_ID.Text}, {textBox_PW.Text}");
            if (textBox_Token.Text.IsEmpty() == false) // 소켓서버에 토큰으로 로그인 확인 요청. 다시 로그인 할 경우. 로그아웃이 될 일은 없겠지만 일단 만들어두기.
            {
                var validateRequest = new ValidateUserTokenRequest
                {
                    UserId = textBox_ID.Text,
                    Token = textBox_Token.Text
                };
                var validateBytes = MemoryPackSerializer.Serialize(validateRequest);
                PostSendPacket(PacketID.ValidateUserTokenRequest, validateBytes);
                return;
            }

            // Hive 서버에 로그인 요청
            HttpClient hive = new();
            var hiveAddr = $"{serverAddr}:{hivePort}/login";
            var hiveResponse = hive.PostAsJsonAsync(hiveAddr, new { Email = textBox_ID.Text, Password = textBox_PW.Text }).Result;

            var hiveJsonResponse = hiveResponse.Content.ReadAsStringAsync().Result;
            var hiveLoginResponse = JsonConvert.DeserializeObject<HiveLoginResponse>(hiveJsonResponse);
            if (hiveLoginResponse.StatusCode != (short)ErrorCode.None)
            {
                DevLog.Write($"로그인 실패: {hiveLoginResponse.StatusCode}");
                return;
            }
            var token = hiveLoginResponse.Token;
            textBox_Token.Text = token;

            // Game 서버에 로그인 요청
            HttpClient game = new();
            var gameAddr = $"{serverAddr}:{gamePort}/login";
            var gameResponse = game.PostAsJsonAsync(gameAddr, new { Email = textBox_ID.Text, Token = token }).Result;

            var gameJsonResponse = gameResponse.Content.ReadAsStringAsync().Result;
            var gameLoginResponse = JsonConvert.DeserializeObject<GameLoginResponse>(gameJsonResponse);
            if (gameLoginResponse.StatusCode != (short)ErrorCode.None)
            {
                DevLog.Write($"GameServer 로그인 실패: {gameLoginResponse.StatusCode}");
                return;
            }

            LoginOK();
            DevLog.Write($"로그인 성공: {textBox_ID.Text}");
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            string address = textBox_IP.Text;

            if (checkBox_useLocalHost.Checked)
            {
                address = "127.0.0.1";
            }

            int port = Convert.ToInt32(textBox_Port.Text);

            if (Network.Connect(address, port))
            {
                //labelStatus.Text = string.Format("{0}. 서버에 접속 중", DateTime.Now);
                button_Connect.Enabled = false;
                button_Disconnect.Enabled = true;
                button_Leave.Enabled = true;
                button_Ready.Enabled = true;
                button_Chat.Enabled = true;

                DevLog.Write($"서버에 접속 중", LOG_LEVEL.INFO);
            }
            else
            {
                //labelStatus.Text = string.Format("{0}. 서버에 접속 실패", DateTime.Now);
            }

            PacketBuffer.Clear();
        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            SetDisconnected();
            Network.Close();

            button_Disconnect.Enabled = false;
            button_Leave.Enabled = false;
            button_Ready.Enabled = false;
            button_Chat.Enabled = false;

            button_Connect.Enabled = true;
        }


        private void button_Ready_Click(object sender, EventArgs e)
        {
            PostSendPacket(PacketID.ReqReadyOmok, null);

            DevLog.Write($"게임 준비 완료 요청");
        }

        private void button_Leave_Click(object sender, EventArgs e)
        {
            button_Match.Enabled = true;
            button_Leave.Enabled = false;
            PostSendPacket(PacketID.ReqRoomLeave, null);
            DevLog.Write($"방 퇴장 요청:  {textBox_RoomNumber.Text} 번");

            // 접속 종료 함수 호출
            button_Disconnect_Click(sender, e);
        }

        private void button_Match_Click(object sender, EventArgs e)
        {
            DevLog.Write($"매칭 요청");
            // 게임 서버에 매칭 요청
            HttpClient client = new();
            var gameAddr = $"{serverAddr}:{gamePort}/matching";
            var gameResponse = client.GetAsync(gameAddr).Result;

            string jsonResponse = gameResponse.Content.ReadAsStringAsync().Result;
            MatchingResponse matchingResponse = JsonConvert.DeserializeObject<MatchingResponse>(jsonResponse);
            if (matchingResponse.StatusCode != (short)ErrorCode.None)
            {
                DevLog.Write($"매칭 실패: {matchingResponse.StatusCode}");
                return;
            }

            var roomNumber = matchingResponse.RoomNumber;
            var serverIP = matchingResponse.ServerIP;

            DevLog.Write($"매칭 결과: {roomNumber}, {serverIP}");

            textBox_IP.Text = serverIP;
            textBox_Port.Text = port;
            textBox_RoomNumber.Text = roomNumber.ToString();


            // 소켓서버에 연결
            if (Network.Connect(textBox_IP.Text, Convert.ToInt32(textBox_Port.Text)) == false)
            {
                DevLog.Write("소켓 서버 연결 실패", LOG_LEVEL.ERROR);
                return;
            }
            SetTimer();


            // 소켓 서버에 방 입장 요청
            var sendPacket = new EnterRoomRequest { RoomNum = roomNumber };
            var sendBytes = MemoryPackSerializer.Serialize(sendPacket);
            PostSendPacket(PacketID.ReqRoomEnter, sendBytes);
        }

        private void button_Chat_Click(object sender, EventArgs e)
        {
            if (textBox_Chat.Text.IsEmpty())
            {
                MessageBox.Show("채팅 메시지를 입력하세요");
                return;
            }

            var requestPkt = new ChatRequest();
            requestPkt.Message = textBox_Chat.Text;

            PostSendPacket(PacketID.ReqRoomChat, MemoryPackSerializer.Serialize(requestPkt));
            DevLog.Write($"방 채팅 요청");
        }
        private void textBox_Chat_TextChanged(object sender, EventArgs e)
        {
            // 채팅창에 엔터가 입력되면 채팅 전송
            if (textBox_Chat.Text.Contains("\r\n"))
            {
                button_Chat_Click(sender, e);
            }
        }

        void NetworkReadProcess()
        {
            while (IsNetworkThreadRunning)
            {
                if (Network.IsConnected() == false)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                var recvData = Network.Receive();

                if (recvData != null)
                {
                    PacketBuffer.Write(recvData.Item2, 0, recvData.Item1);

                    while (true)
                    {
                        var data = PacketBuffer.Read();
                        if (data == null)
                        {
                            break;
                        }

                        RecvPacketQueue.Enqueue(data);
                    }
                    //DevLog.Write($"받은 데이터: {recvData.Item2}", LOG_LEVEL.INFO);
                }
                else
                {
                    Network.Close();
                    SetDisconnected();
                    DevLog.Write("서버와 접속 종료 !!!", LOG_LEVEL.INFO);
                }
            }
        }

        void NetworkSendProcess()
        {
            while (IsNetworkThreadRunning)
            {
                System.Threading.Thread.Sleep(1);

                if (Network.IsConnected() == false)
                {
                    continue;
                }


                if (SendPacketQueue.TryDequeue(out var packet))
                {
                    Network.Send(packet);
                }
            }
        }


        void BackGroundProcess(object sender, EventArgs e)
        {
            ProcessLog();

            try
            {
                byte[] packet = null;

                if (RecvPacketQueue.TryDequeue(out packet))
                {
                    PacketProcess(packet);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("BackGroundProcess. error:{0}", ex.Message));
            }
        }

        private void ProcessLog()
        {
            // 너무 이 작업만 할 수 없으므로 일정 작업 이상을 하면 일단 패스한다.
            int logWorkCount = 0;

            while (IsBackGroundProcessRunning)
            {
                System.Threading.Thread.Sleep(1);

                string msg;

                if (DevLog.GetLog(out msg))
                {
                    ++logWorkCount;

                    if (listBox_Log.Items.Count > 512)
                    {
                        listBox_Log.Items.Clear();
                    }

                    listBox_Log.Items.Add(msg);
                    listBox_Log.SelectedIndex = listBox_Log.Items.Count - 1;
                }
                else
                {
                    break;
                }

                if (logWorkCount > 8)
                {
                    break;
                }
            }
        }


        public void SetDisconnected()
        {
            if (button_Connect.Enabled == false)
            {
                button_Connect.Enabled = true;
                button_Disconnect.Enabled = false;
            }

            while (true)
            {
                if (SendPacketQueue.TryDequeue(out var temp) == false)
                {
                    break;
                }
            }

            RoomChatMsg.Items.Clear();
            UserList.Items.Clear();

            EndGame();
            // 타이머 종료
            HeartBeatPingTimer?.Dispose();

            //labelStatus.Text = "서버 접속이 끊어짐";
        }

        void PostSendPacket(UInt16 packetID, byte[] packetData)
        {
            if (Network.IsConnected() == false)
            {
                DevLog.Write("서버 연결이 되어 있지 않습니다", LOG_LEVEL.ERROR);
                return;
            }

            var header = new PacketHeadererInfo();
            header.ID = packetID;
            header.Type = 0;

            if (packetData != null)
            {
                header.TotalSize = (UInt16)packetData.Length;

                header.Write(packetData);
            }
            else
            {
                packetData = header.Write();
            }

            SendPacketQueue.Enqueue(packetData);
        }


        void AddRoomUserList(string userID)
        {
            UserList.Items.Add(userID);
        }

        void RemoveRoomUserList(string userID) // TODO : 방에서 나간 유저는 clear되어야 한다.
        {
            object removeItem = null;

            foreach (var user in UserList.Items)
            {
                if ((string)user == userID)
                {
                    removeItem = user;
                    break;
                }
            }

            if (removeItem != null)
            {
                UserList.Items.Remove(removeItem);
            }
        }

        string GetOtherPlayer(string myName)
        {
            if (UserList.Items.Count != 2)
            {
                return null;
            }

            var firstName = (string)UserList.Items[0];
            if (firstName == myName)
            {
                return firstName;
            }
            else
            {
                return (string)UserList.Items[1];
            }
        }

        void ClearRoom()
        {
            OmokLogic.StartGame();
            panel1.Invalidate();
        }

        void LeaveRoom()
        {
            UserList.Items.Clear();
            RoomChatMsg.Items.Clear();
            textBox_RoomNumber.Text = "";
            textBox_Chat.Text = "";
            ClearRoom();
        }

        void SendPacketOmokPut(int x, int y)
        {
            var requestPkt = new PutStoneRequest
            {
                X = x,
                Y = y
            };

            var packet = MemoryPackSerializer.Serialize(requestPkt);
            PostSendPacket(PacketID.ReqPutMok, packet);

            DevLog.Write($"put stone 요청 : x  [ {x} ], y: [ {y} ] ");
        }

        private void textBox_Token_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

