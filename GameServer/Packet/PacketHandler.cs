using Server;
using Server.Session;
using ServerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    internal class PacketHandler
    {
        public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
        {
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null)
            {
                return;
            }

            // 해야하는 행동 자체를 push
            GameRoom room = clientSession.Room;
            room.Push(() => room.Leave(clientSession));
        }
        public static void C_MoveHandler(PacketSession session, IPacket packet)
        {
            C_Move movePacket = packet as C_Move;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.Room == null)
            {
                return;
            }

            // 해야하는 행동 자체를 push
            GameRoom room = clientSession.Room;
            room.Push(() => room.Move(clientSession, movePacket));
        }
    }

