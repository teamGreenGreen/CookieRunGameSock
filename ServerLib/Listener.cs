using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            // backlog : 최대 대기수
            _listenSocket.Listen(backlog);

            // 문지기가 10명
            for (int i  = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        /*
        RegisterAccept는 연결이 바로 되면 OnAcceptCompleted를 호출한다. 만약 안되면?
        위의 args.Completed에 연결한 이벤트헨들러에 의해서 이후에 연결되어도 OnAcceptCompleted가 호출된다.
         */
        void RegisterAccept(SocketAsyncEventArgs args)
        {
            // 이 방식의 문제는 기존의 args값이 초기화되지 않는 상황에서 발생한다.
            // 그렇기 때문에 꼭 초기화를 하고 AcceptAsync를 호출하도록 한다.
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            // 모든 일이 끝났으니까 다음 클라이언트를 위해서 다시 연결을 열어준다.
            // 즉, 서버의 리스너는 RegisterAccept와 OnAcceptCompleted를 계속 왔다갔다 하게 된다.
            RegisterAccept(args);
        }
    }
}
