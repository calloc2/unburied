using Godot;
using System.Collections.Generic;

public partial class Walk : Move
{
    [Export] public float Speed = 3.3f;
    public static float Gravity => (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    private Node3D Visuals => Player.GetNode<Node3D>("Rig");
    private bool IsRunning => Input.IsActionPressed("run");
    private AnimationPlayer AnimPlayer => Visuals.GetNode<AnimationPlayer>("AnimationPlayer");

    public override string CheckRelevance(InputPackage input)
    {
        if (input.InputDirection == Vector2.Zero)
            return "idle";
        return "walk";
    }

	public override void Update(double delta, InputPackage input)
	{
		Player.Velocity = CalculateVelocity(input, (float)delta);
		Player.MoveAndSlide();

    }
    private Vector3 CalculateVelocity(InputPackage input, float delta)
    {
        Vector3 velocity = Player.Velocity;

        // Movimento isométrico (45°)
        float angle = Mathf.Pi / 4;
        Vector2 isoInput = new Vector2(
            input.InputDirection.X * Mathf.Cos(angle) - input.InputDirection.Y * Mathf.Sin(angle),
            input.InputDirection.X * Mathf.Sin(angle) + input.InputDirection.Y * Mathf.Cos(angle)
        );

        Vector3 direction = (Visuals.Transform.Basis * new Vector3(isoInput.X, 0, isoInput.Y)).Normalized();

        if (Player.IsOnFloor())
        {
            if (direction != Vector3.Zero)
            {
                float moveSpeed = IsRunning ? Speed * 2.5f : Speed;
                velocity.X = direction.X * moveSpeed;
                velocity.Z = direction.Z * moveSpeed;
                if (IsRunning) AnimPlayer.Play("Sprint");
                AnimPlayer.Play("Walk");
            }
            else
            {
                velocity.X = Mathf.MoveToward(Player.Velocity.X, 0, Speed);
                velocity.Z = Mathf.MoveToward(Player.Velocity.Z, 0, Speed);
            }
            velocity.Y = -0.1f; // Mantém o personagem no chão
        }
        else
        {
            // Aplica gravidade se estiver no ar
            velocity.Y -= Gravity * delta;
        }

        return velocity;
    }
}
