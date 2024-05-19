using CSCommon;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

#pragma warning disable CA1416

namespace myForm
{
    public partial class mainForm
    {
        Dictionary<UInt16, Action<byte[]>> PacketFuncDic = new Dictionary<UInt16, Action<byte[]>>();
        //string serverAddr = "http://" + Environment.GetEnvironmentVariable("SERVER_ADDR") + ":";
        //string hivePort = Environment.GetEnvironmentVariable("HIVE_PORT");
        //string gamePort = Environment.GetEnvironmentVariable("GAME_PORT");
        //string _port = Environment.GetEnvironmentVariable("SOCKET_PORT");
        string serverAddr = "http://34.64.46.227";
        string hivePort = "11502";
        string gamePort = "11500";
        string port = "9000";

        void SetPacketHandler()
        {
            //PacketFuncDic.Add(PACKET_ID.PACKET_ID_ERROR_NTF, PacketProcess_ErrorNotify);
            //PacketFuncDic.Add(PacketID.ResLogin, PacketProcess_Loginin);
            PacketFuncDic.Add(PacketID.ResLogin, PacketProcess_LoginResponseSocketServer);

            PacketFuncDic.Add(PacketID.ResRoomEnter, PacketProcess_RoomEnterResponse);
            PacketFuncDic.Add(PacketID.NtfRoomUserList, PacketProcess_RoomUserListNotify);
            PacketFuncDic.Add(PacketID.NtfRoomNewUser, PacketProcess_RoomNewUserNotify);
            PacketFuncDic.Add(PacketID.ResRoomLeave, PacketProcess_RoomLeaveResponse);
            PacketFuncDic.Add(PacketID.NotifyRoomUserLeft, PacketProcess_RoomLeaveUserNotify);
            PacketFuncDic.Add(PacketID.ResRoomChat, PacketProcess_RoomChatResponse);
            PacketFuncDic.Add(PacketID.NtfRoomChat, PacketProcess_RoomChatNotify);
            PacketFuncDic.Add(PacketID.ResReadyOmok, PacketProcess_ReadyOmokResponse);
            PacketFuncDic.Add(PacketID.NtfReadyOmok, PacketProcess_ReadyOmokNotify);
            PacketFuncDic.Add(PacketID.NtfStartOmok, PacketProcess_StartOmokNotify);
            PacketFuncDic.Add(PacketID.ResPutMok, PacketProcess_PutMokResponse);
            PacketFuncDic.Add(PacketID.NTFPutMok, PacketProcess_PutMokNotify);
            PacketFuncDic.Add(PacketID.NTFEndOmok, PacketProcess_EndOmokNotify);
            PacketFuncDic.Add(PacketID.ResHeartBeat, PacketProcess_HeartBeatPong);
            PacketFuncDic.Add(PacketID.NtfMustClose, PacketProcess_MustCloseNotify);
            PacketFuncDic.Add(PacketID.NotifyUserMustLeave, PacketProcess_NotifyUserMustLeave);
        }

        void PacketProcess(byte[] packet)
        {
            var header = new PacketHeadererInfo();
            header.Read(packet);

            var packetID = header.ID;

            if (PacketFuncDic.ContainsKey(packetID))
            {
                PacketFuncDic[packetID](packet);
            }
            else
            {
                DevLog.Write("Unknown Packet Id: " + packetID);
            }
        }

        void PacketProcess_PutStoneInfoNotifyResponse(byte[] bodyData)
        {
            /*var responsePkt = new PutStoneNtfPacket();
            responsePkt.FromBytes(bodyData);

            DevLog.Write($"'{responsePkt.userID}' Put Stone  : [{responsePkt.xPos}] , [{responsePkt.yPos}] ");*/

        }

        void PacketProcess_GameStartResultResponse(byte[] bodyData)
        {
            /*var responsePkt = new GameStartResPacket();
            responsePkt.FromBytes(bodyData);

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NOT_READY_EXIST)
            {
                DevLog.Write($"모두 레디상태여야 시작합니다.");
            }
            else
            {
                DevLog.Write($"게임시작 !!!! '{responsePkt.UserId}' turn  ");
            }*/
        }

        void PacketProcess_GameEndResultResponse(byte[] bodyData)
        {
            /*var responsePkt = new GameResultResPacket();
            responsePkt.FromBytes(bodyData);

            DevLog.Write($"'{responsePkt.UserId}' WIN , END GAME ");*/

        }

        void PacketProcess_PutStoneResponse(byte[] bodyData)
        {
            /*var responsePkt = new MatchUserResPacket();
            responsePkt.FromBytes(bodyData);

            if((ERROR_CODE)responsePkt.Result != ERROR_CODE.ERROR_NONE)
            {
                DevLog.Write($"Put Stone Error : {(ERROR_CODE)responsePkt.Result}");
            }

            DevLog.Write($"다음 턴 :  {(ERROR_CODE)responsePkt.Result}");*/

        }




        void PacketProcess_ErrorNotify(byte[] packetData)
        {
            /*var notifyPkt = new ErrorNtfPacket();
            notifyPkt.FromBytes(bodyData);

            DevLog.Write($"에러 통보 받음:  {notifyPkt.Error}");*/
        }

        void LoginOK()
        {
            textBox_PW.Text = "";
            // 로그인 성공 시 버튼 비활성화
            button_Login.Enabled = false;
            button_Regist.Enabled = false;
            button_Match.Enabled = true;
        }

        void PacketProcess_LoginResponseSocketServer(byte[] packetData)
        {
            var responsePkt = MemoryPackSerializer.Deserialize<LoginResponseSocketServer>(packetData);
            DevLog.Write($"SocketServer 로그인 결과: {(ErrorCode)responsePkt.Result}");

            if (responsePkt.Result == (int)ErrorCode.None)
            {
                var roomNumber = int.Parse(textBox_RoomNumber.Text);

                // 소켓 서버 로그인에 성공하면, 소켓 서버에 방 입장 요청
                var sendPacket = new EnterRoomRequest { RoomNum = roomNumber };
                var sendBytes = MemoryPackSerializer.Serialize(sendPacket);
                PostSendPacket(PacketID.ReqRoomEnter, sendBytes);
            }
        }

        void PacketProcess_RoomEnterResponse(byte[] packetData)
        {
            var responsePkt = MemoryPackSerializer.Deserialize<EnterRoomResult>(packetData);
            DevLog.Write($"방 입장 결과:  {(ErrorCode)responsePkt.Result}");

            if (responsePkt.Result == (int)ErrorCode.None)
            {
                button_Match.Enabled = false;
                button_Connect.Enabled = false;

                button_Disconnect.Enabled = true;
                button_Leave.Enabled = true;
                button_Ready.Enabled = true;
                button_Chat.Enabled = true;
            }
        }

        void PacketProcess_RoomUserListNotify(byte[] packetData)
        {
            var notifyPkt = MemoryPackSerializer.Deserialize<NotifyRoomUserList>(packetData);

            for (int i = 0; i < notifyPkt.UserIdList.Count; ++i)
            {
                AddRoomUserList(notifyPkt.UserIdList[i]);
            }

            DevLog.Write($"방의 기존 유저 리스트 받음");
        }

        void PacketProcess_RoomNewUserNotify(byte[] packetData)
        {
            var notifyPkt = MemoryPackSerializer.Deserialize<NotifyRoomNewUser>(packetData);

            AddRoomUserList(notifyPkt.UserId);

            DevLog.Write($"방에 새로 들어온 유저 받음");
        }


        void PacketProcess_RoomLeaveResponse(byte[] packetData)
        {
            var responsePkt = MemoryPackSerializer.Deserialize<LeaveRoomResponse>(packetData);

            DevLog.Write($"방 나가기 결과:  {(ErrorCode)responsePkt.Result}");

            // TODO : 방 나가기 성공 시 ListBox 초기화
            if (responsePkt.Result == (int)ErrorCode.None)
            {
                LeaveRoom();
            }
        }

        void PacketProcess_RoomLeaveUserNotify(byte[] packetData)
        {
            var notifyPkt = MemoryPackSerializer.Deserialize<NotifyRoomUserLeft>(packetData);

            RemoveRoomUserList(notifyPkt.UserId);

            DevLog.Write($"방에서 나간 유저 받음");
        }


        void PacketProcess_RoomChatResponse(byte[] packetData)
        {
            var responsePkt = MemoryPackSerializer.Deserialize<ChatResponse>(packetData);

            DevLog.Write($"방 채팅 결과:  {(ErrorCode)responsePkt.Result}");
        }


        void PacketProcess_RoomChatNotify(byte[] packetData)
        {
            var notifyPkt = MemoryPackSerializer.Deserialize<NotifyRoomChat>(packetData);

            AddRoomMessageList(notifyPkt.UserId, notifyPkt.Message);
        }

        void AddRoomMessageList(string userID, string message)
        {
            if (RoomChatMsg.Items.Count > 512)
            {
                RoomChatMsg.Items.Clear();
            }

            RoomChatMsg.Items.Add($"[{userID}]: {message}");
            RoomChatMsg.SelectedIndex = RoomChatMsg.Items.Count - 1;
        }

        void PacketProcess_ReadyOmokResponse(byte[] packetData)
        {
            var responsePkt = MemoryPackSerializer.Deserialize<PKTResReadyOmok>(packetData);

            DevLog.Write($"게임 준비 완료 요청 결과:  {(ErrorCode)responsePkt.Result}");
        }

        void PacketProcess_ReadyOmokNotify(byte[] packetData)
        {
            var notifyPkt = MemoryPackSerializer.Deserialize<PKTNtfReadyOmok>(packetData);

            if (notifyPkt.IsReady)
            {
                DevLog.Write($"[{notifyPkt.UserId}]님은 게임 준비 완료");
            }
            else
            {
                DevLog.Write($"[{notifyPkt.UserId}]님이 게임 준비 완료 취소");
            }

        }

        void PacketProcess_StartOmokNotify(byte[] packetData)
        {
            var isMyTurn = false;

            var notifyPkt = MemoryPackSerializer.Deserialize<PKTNtfStartOmok>(packetData);

            if(notifyPkt.FirstUserId == textBox_ID.Text)
            {
                isMyTurn = true;
            }

            StartGame(isMyTurn, textBox_ID.Text, GetOtherPlayer(textBox_ID.Text));

            DevLog.Write($"게임 시작. 흑돌 플레이어: {notifyPkt.FirstUserId}");
        }


        void PacketProcess_PutMokResponse(byte[] packetData)
        {
            var responsePkt = MemoryPackSerializer.Deserialize<PutStoneResponse>(packetData);

            DevLog.Write($"오목 놓기 결과: {(ErrorCode)responsePkt.Result}");

            //TODO : 요청 실패할 경우 방금 놓은 오목 정보를 취소 시켜야 한다
        }


        void PacketProcess_PutMokNotify(byte[] packetData)
        {
            var notifyPkt = MemoryPackSerializer.Deserialize<NotifyPutStone>(packetData);

            플레이어_돌두기(true, notifyPkt.X, notifyPkt.Y);

            DevLog.Write($"오목 정보: X: {notifyPkt.X},  Y: {notifyPkt.Y}");
        }


        void PacketProcess_EndOmokNotify(byte[] packetData)
        {
            var notifyPkt = MemoryPackSerializer.Deserialize<PKTNtfEndOmok>(packetData);

            EndGame();
            ClearRoom(); // 바둑판 초기화

            DevLog.Write($"오목 GameOver: Win: {notifyPkt.WinUserId}");
        }

        void PacketProcess_HeartBeatPong(byte[] packetData)
        {
            // 받은 Pong 패킷에 대한 처리
            // Result가 ErrorCode.None이 아닐 경우, 접속 해제
            var responsePkt = MemoryPackSerializer.Deserialize<HeartBeatPong>(packetData);
            if (responsePkt.Result != (int)ErrorCode.None)
            {
                DevLog.Write($"HeartBeat Pong Error: {(ErrorCode)responsePkt.Result}");
                SetDisconnected();
            }/*
            else
            {
                DevLog.Write("HeartBeat Pong..oO");
            }*/
        }

        void PacketProcess_MustCloseNotify(byte[] packetData)
        {
            var notifyPkt = MemoryPackSerializer.Deserialize<NotifyUserMustClose>(packetData);

            DevLog.Write($"강제 종료 통보 받음: {notifyPkt.ErrorCode}");

            SetDisconnected();
        }

        void PacketProcess_NotifyUserMustLeave(byte[] packetData)
        {
            var notifyPkt = MemoryPackSerializer.Deserialize<NotifyUserMustLeave>(packetData);

            DevLog.Write($"활동 없으므로 방에서 나가짐: {notifyPkt.RoomNumber}번방 {notifyPkt.UserId} 유저");
            // 모든 유저 내보내기
            RemoveRoomUserList(notifyPkt.UserId);
        }
    }
}
