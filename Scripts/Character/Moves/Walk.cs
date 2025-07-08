using Godot;
using System.Collections.Generic;

public partial class Walk : Move
{
    [Export] public float Speed = 3.3f;

    private AnimationPlayer AnimPlayer => Visuals.GetNode<AnimationPlayer>("AnimationPlayer");
    private bool _isWalkAnimationPlaying = false;

    public override string CheckRelevance(InputPackage input)
    {
        if (Input.IsActionPressed("crouch"))
            return "crouch";

        if (Input.IsActionPressed("sprint") && input.InputDirection != Vector2.Zero)
            return "sprint";

        if (input.InputDirection == Vector2.Zero)
            return "idle";

        return "walk";
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
                velocity.X = direction.X * Speed;
                velocity.Z = direction.Z * Speed;

                // Only play walk animation if not already playing
                if (!_isWalkAnimationPlaying)
                {
                    AnimPlayer.Play("Walk");
                    _isWalkAnimationPlaying = true;
                }
            }
            else
            {
                velocity.X = Mathf.MoveToward(Player.Velocity.X, 0, Speed);
                velocity.Z = Mathf.MoveToward(Player.Velocity.Z, 0, Speed);
                _isWalkAnimationPlaying = false;
            }
            velocity.Y = -0.1f;
        }
        
        Visuals.LookAt(-direction + Player.GlobalTransform.Origin, Vector3.Up);
        return velocity;
    }

    public override void OnEnterState()
    {
        // Reset animation state when entering walk mode
        _isWalkAnimationPlaying = false;
    }

    public override void OnExitState()
    {
        Player.Velocity = Vector3.Zero;
    }
}
