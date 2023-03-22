﻿namespace Sandbox2;

public class CPacketPlayerJoin : APacketClient
{
    public string Username { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write(Username);
    }

    public override void Read(PacketReader reader)
    {
        Username = reader.ReadString();
    }

    public override void Handle(Peer peer)
    {
        // A new player has joined the server
        Net.Server.Log($"Player {Username} (ID = {peer.ID}) joined");

        // Add this player to the server
        Net.Server.Players.Add(peer.ID, new PlayerData
        {
            Username = Username
        });

        // Tell the joining player about peer id
        Net.Server.Send(new SPacketPeerId
        {
            Id = peer.ID
        }, peer);

        // Tell the joining player about all the other players in the server
        var otherPlayers = Net.Server.GetOtherPlayers(peer.ID);
        foreach (var player in otherPlayers)
        {
            Net.Server.Send(new SPacketPlayerJoinLeave
            {
                Id = player.Key,
                Joining = true,
                Position = new Vector2(500, 500)
            }, peer);
        }

        // Broadcast to everyone (except the joining player) about the joining player
        Net.Server.Broadcast(new SPacketPlayerJoinLeave
        {
            Id = peer.ID,
            Joining = true,
            Position = new Vector2(500, 500)
        }, Net.Server.Peers[peer.ID]);
    }
}
