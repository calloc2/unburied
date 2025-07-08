using Godot;
using System.Collections.Generic;

public partial class MultiplayerDebugger : Control
{
    private Label connectionStatus;
    private Label playerCount;
    private VBoxContainer playerList;
    
    public override void _Ready()
    {
        // Create debug UI
        var vbox = new VBoxContainer();
        AddChild(vbox);
        
        connectionStatus = new Label();
        connectionStatus.Text = "Connection: Disconnected";
        vbox.AddChild(connectionStatus);
        
        playerCount = new Label();
        playerCount.Text = "Players: 0";
        vbox.AddChild(playerCount);
        
        playerList = new VBoxContainer();
        vbox.AddChild(playerList);
        
        // Connect to multiplayer events
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
        
        UpdateUI();
    }
    
    public override void _Process(double delta)
    {
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        // Update connection status
        if (Multiplayer.HasMultiplayerPeer())
        {
            if (Multiplayer.IsServer())
            {
                connectionStatus.Text = "Connection: Server";
            }
            else
            {
                connectionStatus.Text = "Connection: Client";
            }
        }
        else
        {
            connectionStatus.Text = "Connection: Disconnected";
        }
        
        // Update player count
        var peers = Multiplayer.GetPeers();
        playerCount.Text = $"Players: {peers.Length + 1}"; // +1 for local player
        
        // Update player list
        foreach (Node child in playerList.GetChildren())
        {
            child.QueueFree();
        }
        
        // Add local player
        var localLabel = new Label();
        localLabel.Text = $"Local Player (ID: {Multiplayer.GetUniqueId()})";
        playerList.AddChild(localLabel);
        
        // Add remote players
        foreach (int peer in peers)
        {
            var peerLabel = new Label();
            peerLabel.Text = $"Remote Player (ID: {peer})";
            playerList.AddChild(peerLabel);
        }
    }
    
    private void OnPeerConnected(long id)
    {
        GD.Print($"[MultiplayerDebugger] Peer {id} connected");
    }
    
    private void OnPeerDisconnected(long id)
    {
        GD.Print($"[MultiplayerDebugger] Peer {id} disconnected");
    }
    
    private void OnConnectionFailed()
    {
        GD.Print("[MultiplayerDebugger] Connection failed");
    }
    
    private void OnConnectedToServer()
    {
        GD.Print("[MultiplayerDebugger] Connected to server");
    }
    
    private void OnServerDisconnected()
    {
        GD.Print("[MultiplayerDebugger] Server disconnected");
    }
}
