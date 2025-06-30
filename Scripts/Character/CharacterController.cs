using Godot;
using System;

public partial class CharacterController : CharacterBody3D
{
	private InputHandler inputHandler => GetNode<InputHandler>("Input");
	private PlayerModel model => GetNode<PlayerModel>("Model");

	public override void _PhysicsProcess(double delta)
	{
		var input = inputHandler.GatherInput();
		model.Update(delta, input);
	}
}
