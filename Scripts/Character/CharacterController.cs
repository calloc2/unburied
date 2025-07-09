using Godot;
using System;

public partial class CharacterController : CharacterBody3D
{
	private InputHandler inputHandler => GetNode<InputHandler>("Input");
	private PlayerModel model => GetNode<PlayerModel>("Model");
	private AudioManager audioManager => GetNode<AudioManager>("AudioManager");
	
	// Multiplayer synchronized properties
	[Export] public string PlayerName { get; set; } = "";
	[Export] public int ProfessionIndex { get; set; } = 0;
	
	// Network sync variables
	private Vector3 networkPosition;
	private Vector3 networkVelocity;
	private Vector3 networkRotation;
	private string networkCurrentMove = "idle";

	public override void _Ready()
	{
		// Set up networking variables
		networkPosition = Position;
		networkVelocity = Velocity;
		networkRotation = Rotation;

		// Update visuals if player info is already set
		UpdatePlayerVisuals();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsMultiplayerAuthority())
		{
			// Handle input and movement for the local player
			var input = inputHandler.GatherInput();
			model.Update(delta, input);
			
			// Sync position and movement to other players
			Rpc(nameof(UpdateNetworkTransform), Position, Velocity, Rotation, model.GetCurrentMoveName());
		}
		else
		{
			// Interpolate networked players to their network positions
			InterpolateNetworkTransform(delta);
		}
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
	private void UpdateNetworkTransform(Vector3 pos, Vector3 vel, Vector3 rot, string currentMove)
	{
		networkPosition = pos;
		networkVelocity = vel;
		networkRotation = rot;
		networkCurrentMove = currentMove;
		
		// Update non-authority player's visual state
		if (!IsMultiplayerAuthority())
		{
			model.SetCurrentMove(currentMove);
		}
	}
	
	private void InterpolateNetworkTransform(double delta)
	{
		// Smooth interpolation for networked players
		float lerpSpeed = 10.0f;
		Position = Position.Lerp(networkPosition, (float)(lerpSpeed * delta));
		Velocity = Velocity.Lerp(networkVelocity, (float)(lerpSpeed * delta));
		Rotation = Rotation.Lerp(networkRotation, (float)(lerpSpeed * delta));
	}
	
	public void SetPlayerInfo(string name, int profession)
	{
		PlayerName = name;
		ProfessionIndex = profession;
		
		// Update visual representation
		CallDeferred(nameof(UpdatePlayerVisuals));
		
		// Sync to other players
		if (IsMultiplayerAuthority())
		{
			Rpc(nameof(SyncPlayerInfo), name, profession);
		}
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
	private void SyncPlayerInfo(string name, int profession)
	{
		PlayerName = name;
		ProfessionIndex = profession;
		UpdatePlayerVisuals();
	}
	
	private void UpdatePlayerVisuals()
	{
		// Create or update name label
		var nameLabel = GetNodeOrNull<Label3D>("NameLabel");
		if (nameLabel == null && !string.IsNullOrEmpty(PlayerName))
		{
			nameLabel = new Label3D();
			nameLabel.Name = "NameLabel";
			nameLabel.Position = new Vector3(0, 2.5f, 0); // Above player
			nameLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
			AddChild(nameLabel);
		}
		
		if (nameLabel != null && !string.IsNullOrEmpty(PlayerName))
		{
			nameLabel.Text = PlayerName;
			
			// Color based on if this is the local player
			if (IsMultiplayerAuthority())
			{
				nameLabel.Modulate = Colors.Yellow; // Local player
			}
			else
			{
				nameLabel.Modulate = Colors.White; // Other players
			}
		}
	}
}
