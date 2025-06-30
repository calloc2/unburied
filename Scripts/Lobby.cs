using Godot;
using System.Collections.Generic;

public partial class Lobby : Control
{
    [Export] public LineEdit NameEdit;
    [Export] public OptionButton ProfessionSelect;
    [Export] public Button ReadyButton;
    [Export] public Label TimerLabel;
    [Export] public VBoxContainer PlayerListContainer;

    private bool isReady = false;
    private float startTimer = 30.0f;
    private float timer = 0.0f;
    private bool countdownStarted = false;

    private Dictionary<int, PlayerInfo> connectedPlayers = new Dictionary<int, PlayerInfo>();

    public struct PlayerInfo
    {
        public string Name;
        public int ProfessionIndex;
        public bool IsReady;

        public PlayerInfo(string name, int professionIndex, bool isReady)
        {
            Name = name;
            ProfessionIndex = professionIndex;
            IsReady = isReady;
        }
    }

    public override void _Ready()
    {
        ReadyButton.Pressed += OnReadyPressed;
        NameEdit.TextChanged += OnNameEditChanged;
        ProfessionSelect.AddItem("Inquisidor");
        ProfessionSelect.AddItem("Mercante");
        ProfessionSelect.AddItem("Barão");

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;

        UpdatePlayerList();
    }

    public override void _Process(double delta)
    {
        if (countdownStarted)
        {
            timer -= (float)delta;
            TimerLabel.Text = $"Partida começa em: {Mathf.CeilToInt(timer)}s";
            if (timer <= 0)
            {
                GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
            }
        }
    }

    private void OnPeerConnected(long id)
    {
        GD.Print($"Player {id} connected");
        UpdatePlayerList();
    }

    private void OnPeerDisconnected(long id)
    {
        GD.Print($"Player {id} disconnected");
        connectedPlayers.Remove((int)id);
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        if (PlayerListContainer == null)
            return;

        foreach (Node child in PlayerListContainer.GetChildren())
        {
            child.QueueFree();
        }

        int localPeerId = Multiplayer.GetUniqueId();
        string localPlayerName = GetLocalPlayerName();

        foreach (var kvp in connectedPlayers)
        {
            var playerId = kvp.Key;
            var playerInfo = kvp.Value;

            var playerLabel = new Label();
            string professionName = GetProfessionName(playerInfo.ProfessionIndex);
            string readyStatus = playerInfo.IsReady ? "✓" : "⏳";
            string youTag = (playerId == localPeerId) ? " [Você]" : "";
            playerLabel.Text = $"{readyStatus} {playerInfo.Name} ({professionName}){youTag}";

            PlayerListContainer.AddChild(playerLabel);
        }

        // Add local player if not in connectedPlayers (e.g., single player or host)
        if (!connectedPlayers.ContainsKey(localPeerId))
        {
            var localLabel = new Label();
            string localProfession = GetProfessionName(ProfessionSelect.Selected);
            string localReadyStatus = isReady ? "✓" : "⏳";
            localLabel.Text = $"{localReadyStatus} {localPlayerName} ({localProfession}) [Você]";
            PlayerListContainer.AddChild(localLabel);
        }
    }

    private string GetProfessionName(int index)
    {
        return index switch
        {
            0 => "Inquisidor",
            1 => "Mercante",
            2 => "Barão",
            _ => "Desconhecida"
        };
    }

    private void OnReadyPressed()
    {
        isReady = true;
        ReadyButton.Disabled = true;
        UpdatePlayerList();
        Rpc(nameof(RemoteSetPlayerReady), GetLocalPlayerName(), ProfessionSelect.Selected);
    }

    private void OnNameEditChanged(string newText)
    {
        UpdatePlayerList();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    public void RemoteSetPlayerReady(string playerName, int professionIdx)
    {
        int senderId = Multiplayer.GetRemoteSenderId();

        connectedPlayers[senderId] = new PlayerInfo(playerName, professionIdx, true);
        UpdatePlayerList();

        if (!countdownStarted)
        {
            countdownStarted = true;
            timer = startTimer;
        }
    }

    public string GetLocalPlayerName()
    {
        return string.IsNullOrEmpty(NameEdit.Text) ? "Jogador Local" : NameEdit.Text;
    }
}