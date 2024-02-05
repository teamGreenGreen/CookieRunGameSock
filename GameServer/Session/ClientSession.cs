using ServerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Session
{
    class ClientSession : PacketSession
    {
        public int SessionID { get; set; }
        public GameRoom Room { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }



        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            // 주문서를 제출할 뿐, 실행을 직접 하지 않는다.
            // Program 전역 Room
            Program.Room.Push(()=> Program.Room.Enter(this));
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if (Room != null)
            {
                GameRoom room = Room;
                room.Push(()=> room.Leave(this));
                Room = null;
            }
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }


        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transfered bytes: {numOfBytes}");
        }
    }
}
