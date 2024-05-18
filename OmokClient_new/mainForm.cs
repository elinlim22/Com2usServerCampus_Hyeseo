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

        string _port = "9000"; // TODO : Config�� ����


        public mainForm()
        {
            InitializeComponent();
            // SetTimer(); >> Login���� ���� �� ����
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
            DevLog.Write("���α׷� ���� !!!", LOG_LEVEL.INFO);
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsNetworkThreadRunning = false;
            IsBackGroundProcessRunning = false;

            Network.Close();
        }

        // ��ư Ŭ�� �̺�Ʈ ó�� �Լ���!
        private void button_Regist_Click(object sender, EventArgs e)
        {
            DevLog.Write($"ȸ������ ��û:  {textBox_ID.Text}, {textBox_PW.Text}");
            // Hive ������ ȸ������ ��û
            HttpClient client = new();
            var hiveResponse = client.PostAsJsonAsync(hiveAddress + "/AuthUser", // TODO : ����� �ּ� Ȯ���ϱ�
                                                      new { Email = textBox_ID.Text, Password = textBox_PW.Text }).Result;
            if (hiveResponse.StatusCode != HttpStatusCode.OK)
            {
                DevLog.Write($"ȸ������ ����: {hiveResponse.StatusCode}");
                return;
            }
            DevLog.Write($"ȸ������ ����: {textBox_ID.Text}");
            textBox_PW.Text = "";
        }

        private void button_Login_Click(object sender, EventArgs e)
        {
            if (textBox_Token.Text.IsEmpty() == false) // ���ϼ����� ��ū���� �α��� Ȯ�� ��û. �ٽ� �α��� �� ���. �α׾ƿ��� �� ���� �������� �ϴ� �����α�.
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

            // Hive ������ �α��� ��û
            HttpClient hive = new();
            var hiveResponse = hive.PostAsJsonAsync(hiveAddress + "/AuthUser", // TODO : ����� �ּ� Ȯ���ϱ�
                                                                     new { Email = textBox_ID.Text, Password = textBox_PW.Text }).Result;
            if (hiveResponse.StatusCode != HttpStatusCode.OK)
            {
                DevLog.Write($"HiveServer �α��� ����: {hiveResponse.StatusCode}");
                return;
            }

            var hiveJsonResponse = hiveResponse.Content.ReadAsStringAsync().Result; 
            var hiveLoginResponse = JsonConvert.DeserializeObject<LoginResponse>(hiveJsonResponse);
            if (hiveLoginResponse == null)
            {
                DevLog.Write($"�α��� ����: {hiveResponse.StatusCode}");
                return;
            }
            var token = hiveLoginResponse.Token;
            textBox_Token.Text = token;

            // Game ������ �α��� ��û
            HttpClient game = new();
            var gameResponse = game.PostAsJsonAsync(gameAddress + "/AuthUser", // TODO : ����� �ּ� Ȯ���ϱ�
                                                                    new { Token = token }).Result;
            if (gameResponse.StatusCode != HttpStatusCode.OK)
            {
                DevLog.Write($"GameServer �α��� ����: {gameResponse.StatusCode}");
                return;
            }

            LoginOK();
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
                //labelStatus.Text = string.Format("{0}. ������ ���� ��", DateTime.Now);
                button_Connect.Enabled = false;
                button_Disconnect.Enabled = true;
                button_Leave.Enabled = true;
                button_Ready.Enabled = true;
                button_Chat.Enabled = true;

                DevLog.Write($"������ ���� ��", LOG_LEVEL.INFO);
            }
            else
            {
                //labelStatus.Text = string.Format("{0}. ������ ���� ����", DateTime.Now);
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

            DevLog.Write($"���� �غ� �Ϸ� ��û");
        }

        private void button_Leave_Click(object sender, EventArgs e)
        {
            button_Match.Enabled = true;
            button_Leave.Enabled = false;
            PostSendPacket(PacketID.ReqRoomLeave, null);
            DevLog.Write($"�� ���� ��û:  {textBox_RoomNumber.Text} ��");

            // ���� ���� �Լ� ȣ��
            button_Disconnect_Click(sender, e);
        }

        private void button_Match_Click(object sender, EventArgs e)
        {
            // ���� ������ ��Ī ��û
            HttpClient client = new();
            var gameResponse = client.GetAsync(gameAddress + "/AuthUser").Result;

            string jsonResponse = gameResponse.Content.ReadAsStringAsync().Result;
            MatchingResponse matchingResponse = JsonConvert.DeserializeObject<MatchingResponse>(jsonResponse);
            if (matchingResponse == null)
            {
                DevLog.Write($"��Ī ����: {gameResponse.StatusCode}");
                return;
            }

            var roomNumber = matchingResponse.RoomNumber;
            var serverIP = matchingResponse.ServerIP;

            DevLog.Write($"��Ī ���: {roomNumber}, {serverIP}");

            textBox_IP.Text = serverIP;
            textBox_Port.Text = _port;
            textBox_RoomNumber.Text = roomNumber.ToString();

            // ���� ������ �� ���� ��û
            var sendPacket = new EnterRoomRequest { RoomNum = roomNumber };
            var sendBytes = MemoryPackSerializer.Serialize(sendPacket);
            PostSendPacket(PacketID.ReqRoomEnter, sendBytes);

            button_Match.Enabled = false;
            button_Connect.Enabled = false;

            button_Disconnect.Enabled = true;
            button_Leave.Enabled = true;
            button_Ready.Enabled = true;
            button_Chat.Enabled = true;
            DevLog.Write($"��Ī ��û");
        }

        private void button_Chat_Click(object sender, EventArgs e)
        {
            if (textBox_Chat.Text.IsEmpty())
            {
                MessageBox.Show("ä�� �޽����� �Է��ϼ���");
                return;
            }

            var requestPkt = new ChatRequest();
            requestPkt.Message = textBox_Chat.Text;

            PostSendPacket(PacketID.ReqRoomChat, MemoryPackSerializer.Serialize(requestPkt));
            DevLog.Write($"�� ä�� ��û");
        }
        private void textBox_Chat_TextChanged(object sender, EventArgs e)
        {
            // ä��â�� ���Ͱ� �ԷµǸ� ä�� ����
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
                    //DevLog.Write($"���� ������: {recvData.Item2}", LOG_LEVEL.INFO);
                }
                else
                {
                    Network.Close();
                    SetDisconnected();
                    DevLog.Write("������ ���� ���� !!!", LOG_LEVEL.INFO);
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
            // �ʹ� �� �۾��� �� �� �����Ƿ� ���� �۾� �̻��� �ϸ� �ϴ� �н��Ѵ�.
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
            // Ÿ�̸� ����
            HeartBeatPingTimer?.Dispose();

            //labelStatus.Text = "���� ������ ������";
        }

        void PostSendPacket(UInt16 packetID, byte[] packetData)
        {
            if (Network.IsConnected() == false)
            {
                DevLog.Write("���� ������ �Ǿ� ���� �ʽ��ϴ�", LOG_LEVEL.ERROR);
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

        void RemoveRoomUserList(string userID) // TODO : �濡�� ���� ������ clear�Ǿ�� �Ѵ�.
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

            DevLog.Write($"put stone ��û : x  [ {x} ], y: [ {y} ] ");
        }
    }
}

