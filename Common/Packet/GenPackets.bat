START ../../PacketGenerator/bin/Debug/net8.0/PacketGenerator.exe ../../PacketGenerator/PDL.xml
XCOPY /Y GenPackets.cs "../../GameServer/Packet"
XCOPY /Y ServerPacketManager.cs "../../GameServer/Packet"