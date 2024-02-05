using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Session;
using ServerLib;

namespace Server
{
    internal class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession s in _sessions)
                s.Send(_pendingList);

            //Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        // jobQueue가 Broadcast, Enter, Leave를 단일로 실행시키기 때문에
        // lock을 해당 함수 내부에서 사용할 이유가 없다.
        public void Broadcast(ArraySegment<byte> segment)
        {
            // 클라이언트 단에서 패킷 모아 보내기
            _pendingList.Add(segment);
        }
        public void Enter(ClientSession session)
        {
            // 플레이어 추가하고
            _sessions.Add(session);
            session.Room = this;

            // 들어온 유저에게 모든 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
            foreach (ClientSession s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerId = s.SessionID,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                });
            }

            session.Send(players.Write());

            // 새로운 유저 입장을 모두에게 알린다
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.SessionID;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
            Broadcast(enter.Write());
        }
        public void Leave(ClientSession session)
        {
            // player 제거
            _sessions.Remove(session);

            // 다른 유저들에게 알린다
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionID;
            Broadcast(leave.Write());
        }
        public void Move(ClientSession session, C_Move pakcet)
        {
            // 좌표 바꿔주고
            session.PosX = pakcet.posX;
            session.PosY = pakcet.posY;
            session.PosZ = pakcet.posZ;

            // 모두에게 알린다.
            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.SessionID;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;
            Broadcast(move.Write());
        }
    }
}
