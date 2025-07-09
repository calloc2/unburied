using Godot;
using System.Collections.Generic;

public partial class GameManager : Node
{
    [Export] public PackedScene PlayerScene;
    
    private Dictionary<int, PlayerData> playersData = new Dictionary<int, PlayerData>();
    private Dictionary<int, CharacterController> spawnedPlayers = new Dictionary<int, CharacterController>();
    
    public struct PlayerData
    {
        public string Name;
        public int ProfessionIndex;
        public Vector3 SpawnPosition;
        
        public PlayerData(string name, int professionIndex, Vector3 spawnPosition)
        {
            Name = name;
            ProfessionIndex = professionIndex;
            SpawnPosition = spawnPosition;
        }
    }

    public override void _Ready()
    {
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;

        // Get player data from NetworkManager
        if (NetworkManager.Instance != null)
        {
            var lobbyPlayers = NetworkManager.Instance.GetLobbyPlayers();
            foreach (var kvp in lobbyPlayers)
            {
                playersData[kvp.Key] = new PlayerData(kvp.Value.Name, kvp.Value.ProfessionIndex, GetSpawnPosition());
            }
        }

        // Always spawn local player
        int localId = Multiplayer.GetUniqueId();
        if (!playersData.ContainsKey(localId))
        {
            string localName = "Local Player";
            if (NetworkManager.Instance != null)
            {
                var lobbyPlayers = NetworkManager.Instance.GetLobbyPlayers();
                if (lobbyPlayers.ContainsKey(localId))
                {
                    localName = lobbyPlayers[localId].Name;
                }
            }
            playersData[localId] = new PlayerData(localName, 0, GetSpawnPosition());
        }

        // Spawn all players
        CallDeferred(nameof(SpawnAllPlayers));
    }

    private void OnPeerConnected(long id)
    {
        // Handle late joiners
        if (playersData.ContainsKey((int)id))
        {
            SpawnPlayer((int)id, playersData[(int)id]);
        }
    }

    private void OnPeerDisconnected(long id)
    {
        if (spawnedPlayers.ContainsKey((int)id))
        {
            spawnedPlayers[(int)id].QueueFree();
            spawnedPlayers.Remove((int)id);
        }
        playersData.Remove((int)id);
    }

    private void SpawnAllPlayers()
    {
        foreach (var kvp in playersData)
        {
            SpawnPlayer(kvp.Key, kvp.Value);
        }
    }

    private void SpawnPlayer(int playerId, PlayerData playerData)
    {
        if (PlayerScene == null)
        {
            GD.PrintErr("PlayerScene not assigned in GameManager!");
            return;
        }

        var playerInstance = PlayerScene.Instantiate<CharacterController>();
        playerInstance.Name = $"Player_{playerId}";
        playerInstance.Position = playerData.SpawnPosition;
        
        // Set up multiplayer authority
        playerInstance.SetMultiplayerAuthority(playerId);
        
        // Set player info
        playerInstance.SetPlayerInfo(playerData.Name, playerData.ProfessionIndex);
        
        // Add to scene
        GetTree().CurrentScene.AddChild(playerInstance);
        spawnedPlayers[playerId] = playerInstance;
        var audioManager = playerInstance.GetNode<AudioManager>("AudioManager");

        audioManager.SetupAudio(playerId);

        GD.Print($"Spawned player {playerId} ({playerData.Name}) at {playerData.SpawnPosition}");
    }

    private Vector3 GetSpawnPosition()
    {
        // Simple spawn positions in a circle
        int playerCount = playersData.Count;
        float angle = playerCount * (Mathf.Pi * 2 / 8); // Support up to 8 players
        float radius = 5.0f;
        return new Vector3(
            Mathf.Cos(angle) * radius,
            1.0f, // Height above ground
            Mathf.Sin(angle) * radius
        );
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    public void RequestPlayerSpawn(string playerName, int professionIndex)
    {
        int senderId = Multiplayer.GetRemoteSenderId();
        var playerData = new PlayerData(playerName, professionIndex, GetSpawnPosition());
        playersData[senderId] = playerData;
        SpawnPlayer(senderId, playerData);
    }
}
