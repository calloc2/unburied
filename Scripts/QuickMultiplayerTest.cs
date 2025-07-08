using Godot;

public partial class QuickMultiplayerTest : Control
{
    [Export] public Button HostButton;
    [Export] public Button JoinButton;
    [Export] public LineEdit IpInput;
    [Export] public Label StatusLabel;
    
    public override void _Ready()
    {
        // Create UI if not assigned
        if (HostButton == null || JoinButton == null)
        {
            CreateUI();
        }
        
        HostButton.Pressed += OnHostPressed;
        JoinButton.Pressed += OnJoinPressed;
        
        // Connect to network events
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        
        UpdateStatus();
    }
    
    private void CreateUI()
    {
        var vbox = new VBoxContainer();
        AddChild(vbox);
        
        StatusLabel = new Label();
        StatusLabel.Text = "Not Connected";
        vbox.AddChild(StatusLabel);
        
        HostButton = new Button();
        HostButton.Text = "Host Game";
        vbox.AddChild(HostButton);
        
        var hbox = new HBoxContainer();
        vbox.AddChild(hbox);
        
        IpInput = new LineEdit();
        IpInput.Text = "127.0.0.1";
        IpInput.PlaceholderText = "IP Address";
        hbox.AddChild(IpInput);
        
        JoinButton = new Button();
        JoinButton.Text = "Join Game";
        hbox.AddChild(JoinButton);
    }
    
    private void OnHostPressed()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.HostGame();
            UpdateStatus();
            
            // Add local player info
            NetworkManager.Instance.SetLobbyPlayerInfo(Multiplayer.GetUniqueId(), "Host Player", 0, true);
            
            // Go to game scene after short delay
            GetTree().CreateTimer(1.0).Timeout += () => {
                GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
            };
        }
    }
    
    private void OnJoinPressed()
    {
        if (NetworkManager.Instance != null && !string.IsNullOrEmpty(IpInput.Text))
        {
            NetworkManager.Instance.JoinGame(IpInput.Text);
            UpdateStatus();
        }
    }
    
    private void OnPeerConnected(long id)
    {
        GD.Print($"Player {id} connected");
        UpdateStatus();
    }
    
    private void OnPeerDisconnected(long id)
    {
        GD.Print($"Player {id} disconnected");
        UpdateStatus();
    }
    
    private void OnConnectedToServer()
    {
        GD.Print("Connected to server!");
        UpdateStatus();
        
        // Add local player info
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.SetLobbyPlayerInfo(Multiplayer.GetUniqueId(), "Client Player", 1, true);
        }
        
        // Go to game scene after short delay
        GetTree().CreateTimer(1.0).Timeout += () => {
            GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
        };
    }
    
    private void OnConnectionFailed()
    {
        GD.PrintErr("Connection failed!");
        UpdateStatus();
    }
    
    private void UpdateStatus()
    {
        if (StatusLabel == null) return;
        
        if (Multiplayer.HasMultiplayerPeer())
        {
            if (Multiplayer.IsServer())
            {
                var peers = Multiplayer.GetPeers();
                StatusLabel.Text = $"Hosting - {peers.Length} players connected";
            }
            else
            {
                StatusLabel.Text = "Connected to server";
            }
        }
        else
        {
            StatusLabel.Text = "Not Connected";
        }
    }
}
