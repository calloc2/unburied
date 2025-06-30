using Godot;
using System.Collections.Generic;

public partial class CrouchFwd : Move
{
    [Export] public float CrouchSpeed = 1.5f;

    private AnimationPlayer AnimPlayer => Visuals.GetNode<AnimationPlayer>("AnimationPlayer");

    public override string CheckRelevance(InputPackage input)
    {
        if (Input.IsActionPressed("crouch") && input.InputDirection == Vector2.Zero)
            return "crouch";

        if (input.InputDirection == Vector2.Zero)
            return "idle";

        return "crouch_fwd";
    }

    public override void Update(double delta, InputPackage input)
    {
        base.Update(delta, input);
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
                velocity.X = direction.X * CrouchSpeed;
                velocity.Z = direction.Z * CrouchSpeed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Player.Velocity.X, 0, CrouchSpeed);
                velocity.Z = Mathf.MoveToward(Player.Velocity.Z, 0, CrouchSpeed);
            }
            velocity.Y = -0.1f;
        }

        Visuals.LookAt(-direction + Player.GlobalTransform.Origin, Vector3.Up);
        return velocity;
    }

    public override void OnEnterState()
    {
        AnimPlayer.Play("Crouch_Fwd");
    }
    
    public override void OnExitState()
    {
        // Stop sprint animation when exiting
        base.OnExitState();
    }
}
