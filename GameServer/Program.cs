using System.Net;
using System.Net.Sockets;
using System.Text;
using Server.Session;
using ServerLib;

namespace Server
{
    internal class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening ...");

            //FlushRoom();
            JobTimer.Instance.Push(FlushRoom);

            while (true)
            {
                JobTimer.Instance.Flush();

                // tickCount 안쓰는 이유? 관리하는 개체가 3000개면 tickCount 3000번 확인
                // 중앙(JobTimer)에서 관리하면, Priority Queue가 대소관계를 빠르게(log n) 비교하여
                // 현재 실행 가능한 작업들만 실행시킨다.
            }

        }
    }
}