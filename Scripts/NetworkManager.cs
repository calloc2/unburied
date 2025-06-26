using Godot;
using System;

public partial class NetworkManager : Node
{
    public static NetworkManager Instance;

    [Export] public int Port = 4321;
    private ENetMultiplayerPeer peer;

    public override void _Ready()
    {
        Instance = this;
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        
        CheckLaunchArguments();
    }

    private void CheckLaunchArguments()
    {
        var args = OS.GetCmdlineArgs();
        
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--server" || args[i] == "--host")
            {
                GD.Print("Starting as server from launch arguments");
                HostGame();
                GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, "res://Scenes/Lobby.tscn");
                return;
            }
            else if (args[i] == "--client" && i + 1 < args.Length)
            {
                string ip = args[i + 1];
                GD.Print($"Starting as client connecting to {ip} from launch arguments");
                JoinGame(ip);
                GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, "res://Scenes/Lobby.tscn");
                return;
            }
            else if (args[i] == "--port" && i + 1 < args.Length)
            {
                if (int.TryParse(args[i + 1], out int customPort))
                {
                    Port = customPort;
                    GD.Print($"Using custom port: {Port}");
                }
            }
        }
    }

    public void HostGame()
    {
        peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(Port);
        if (error == Error.Ok)
        {
            Multiplayer.MultiplayerPeer = peer;
            GD.Print($"Server started on port {Port}");
        }
        else
        {
            GD.PrintErr($"Failed to create server: {error}");
        }
    }

    public void JoinGame(string ip)
    {
        peer = new ENetMultiplayerPeer();
        var error = peer.CreateClient(ip, Port);
        if (error == Error.Ok)
        {
            Multiplayer.MultiplayerPeer = peer;
            GD.Print($"Connecting to {ip}:{Port}");
        }
        else
        {
            GD.PrintErr($"Failed to create client: {error}");
        }
    }

    private void OnPeerConnected(long id) { GD.Print($"Peer {id} connected!"); }
    private void OnPeerDisconnected(long id) { GD.Print($"Peer {id} disconnected!"); }
}