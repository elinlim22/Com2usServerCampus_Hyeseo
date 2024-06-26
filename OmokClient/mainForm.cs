﻿using CSCommon;
using MemoryPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

#pragma warning disable CA1416

namespace csharp_test_client
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

            btnDisconnect.Enabled = false;

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

        private void btnConnect_Click(object sender, EventArgs e) // 서버 접속
        {
            string address = textBoxIP.Text;

            if (checkBoxLocalHostIP.Checked)
            {
                address = "127.0.0.1";
            }

            int port = Convert.ToInt32(textBoxPort.Text);

            if (Network.Connect(address, port))
            {
                labelStatus.Text = string.Format("{0}. 서버에 접속 중", DateTime.Now);
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                DevLog.Write($"서버에 접속 중", LOG_LEVEL.INFO);
            }
            else
            {
                labelStatus.Text = string.Format("{0}. 서버에 접속 실패", DateTime.Now);
            }

            PacketBuffer.Clear();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            SetDisconnected();
            Network.Close();
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

                if(RecvPacketQueue.TryDequeue(out packet))
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

                    if (listBoxLog.Items.Count > 512)
                    {
                        listBoxLog.Items.Clear();
                    }

                    listBoxLog.Items.Add(msg);
                    listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1;
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
            if (btnConnect.Enabled == false)
            {
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }

            while (true)
            {
                if (SendPacketQueue.TryDequeue(out var temp) == false)
                {
                    break;
                }
            }

            listBoxRoomChatMsg.Items.Clear();
            listBoxRoomUserList.Items.Clear();

            EndGame();
            // 타이머 종료
            HeartBeatPingTimer?.Dispose();

            labelStatus.Text = "서버 접속이 끊어짐";
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
            listBoxRoomUserList.Items.Add(userID);
        }

        void RemoveRoomUserList(string userID) // TODO : 방에서 나간 유저는 clear되어야 한다.
        {
            object removeItem = null;

            foreach( var user in listBoxRoomUserList.Items)
            {
                if((string)user == userID)
                {
                    removeItem = user;
                    break;
                }
            }

            if (removeItem != null)
            {
                listBoxRoomUserList.Items.Remove(removeItem);
            }
        }

        string GetOtherPlayer(string myName)
        {
            if(listBoxRoomUserList.Items.Count != 2)
            {
                return null;
            }

            var firstName = (string)listBoxRoomUserList.Items[0];
            if (firstName == myName)
            {
                return firstName;
            }
            else
            {
                return (string)listBoxRoomUserList.Items[1];
            }
        }
        
        void ClearRoom()
        {
            OmokLogic.StartGame();
            panel1.Invalidate();
        }

        void LeaveRoom()
        {             
            listBoxRoomUserList.Items.Clear();
            listBoxRoomChatMsg.Items.Clear();
            textBoxRoomNumber.Text = "";
            textBoxRoomSendMsg.Text = "";
            ClearRoom();
        }

        // 로그인 요청
        private void button2_Click(object sender, EventArgs e) // 로그인 요청
        {
            var loginReq = new LoginRequest();
            loginReq.UserId = textBoxUserId.Text;
            loginReq.Token = textBoxUserPW.Text;
            var packet = MemoryPackSerializer.Serialize(loginReq);

            PostSendPacket(PacketID.ReqLogin, packet);
            DevLog.Write($"로그인 요청:  {textBoxUserId.Text}, {textBoxUserPW.Text}");
        }

        private void btn_RoomEnter_Click(object sender, EventArgs e)
        {
            PostSendPacket(PacketID.ReqRoomEnter, null);
            DevLog.Write($"방 입장 요청:  {textBoxRoomNumber.Text} 번");
        }

        private void btn_RoomLeave_Click(object sender, EventArgs e)
        {
            PostSendPacket(PacketID.ReqRoomLeave, null);
            DevLog.Write($"방 퇴장 요청:  {textBoxRoomNumber.Text} 번");
        }

        private void btnRoomChat_Click(object sender, EventArgs e)
        {
            if(textBoxRoomSendMsg.Text.IsEmpty())
            {
                MessageBox.Show("채팅 메시지를 입력하세요");
                return;
            }

            var requestPkt = new ChatRequest();
            requestPkt.Message = textBoxRoomSendMsg.Text;

            PostSendPacket(PacketID.ReqRoomChat, MemoryPackSerializer.Serialize(requestPkt));
            DevLog.Write($"방 채팅 요청");
        }

        private void btnMatching_Click(object sender, EventArgs e)
        {
            // PostSendPacket(PacketID.ReqMatching, null);
            DevLog.Write($"매칭 요청");
        }


        private void listBoxRoomChatMsg_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxRelay_TextChanged(object sender, EventArgs e)
        {

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

        private void btn_GameStartClick(object sender, EventArgs e)
        {
            DevLog.Write($"게임 시작 요청");
            PostSendPacket(PacketID.ReqReadyOmok, null);
            StartGame(true, "My", "Other");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddUser("test1");
            AddUser("test2");
        }

        void AddUser(string userID)
        {
            var value = new PvPMatchingResult
            {
                IP = "127.0.0.1",
                Port = 32451,
                RoomNumber = 0,
                Index = 1,
                Token = "123qwe"
            };
            var saveValue = MemoryPackSerializer.Serialize(value);

            var key = "ret_matching_" + userID;

            var redisConfig = new CloudStructures.RedisConfig("omok", "127.0.0.1");
            var RedisConnection = new CloudStructures.RedisConnection(redisConfig);

            var v = new CloudStructures.Structures.RedisString<byte[]>(RedisConnection, key, null);
            // var ret = v.SetAsync(saveValue).Result;
        }

        // 게임 시작 요청
        private void button3_Click(object sender, EventArgs e)
        {
            PostSendPacket(PacketID.ReqReadyOmok, null);

            DevLog.Write($"게임 준비 완료 요청");
        }
    }
}
