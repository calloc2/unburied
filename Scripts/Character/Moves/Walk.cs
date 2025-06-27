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
        Vector2 inputDir = input.InputDirection;

        float angle = Mathf.DegToRad(45);
        Vector2 rotatedInput = new Vector2(
            inputDir.X * Mathf.Cos(angle) - inputDir.Y * Mathf.Sin(angle),
            inputDir.X * Mathf.Sin(angle) + inputDir.Y * Mathf.Cos(angle)
        );

        Vector3 direction = new Vector3(rotatedInput.X, 0, rotatedInput.Y).Normalized();

        if (Player.IsOnFloor())
        {
            if (direction != Vector3.Zero)
            {
                float moveSpeed = IsRunning ? Speed * 2.5f : Speed;
                velocity.X = direction.X * moveSpeed;
                velocity.Z = direction.Z * moveSpeed;
                AnimPlayer.Play("Walk");
            }
            else
            {
                velocity.X = Mathf.MoveToward(Player.Velocity.X, 0, Speed);
                velocity.Z = Mathf.MoveToward(Player.Velocity.Z, 0, Speed);
            }
            velocity.Y = -0.1f;
        }
        else
        {
            velocity.Y -= Gravity * delta;
        }

        Visuals.LookAt(-direction + Player.GlobalTransform.Origin, Vector3.Up);
        return velocity;
    }
}
