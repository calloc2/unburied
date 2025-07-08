using Godot;

public partial class MultiplayerSetup : Node
{
    [Export] public PackedScene PlayerScene;
    [Export] public NodePath SpawnPath = "./";
    
    private MultiplayerSpawner spawner;
    
    public override void _Ready()
    {
        // Create and configure MultiplayerSpawner
        spawner = new MultiplayerSpawner();
        spawner.SpawnPath = SpawnPath;
        
        // Add PlayerScene to spawnable scenes using the correct method
        if (PlayerScene != null)
        {
            spawner.AddSpawnableScene(PlayerScene.ResourcePath);
        }
        
        AddChild(spawner);
        
        // Connect to multiplayer events
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
    }
    
    private void OnPeerConnected(long id)
    {
        GD.Print($"Player {id} connected to game");
    }
    
    private void OnPeerDisconnected(long id)
    {
        GD.Print($"Player {id} disconnected from game");
    }
}
