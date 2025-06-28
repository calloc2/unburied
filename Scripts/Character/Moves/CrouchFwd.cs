using Godot;
using System.Collections.Generic;

public partial class CrouchFwd : Move
{
    [Export] public float CrouchSpeed = 1.5f;
    public static float Gravity => (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    private Node3D Visuals => Player.GetNode<Node3D>("Rig");
    private AnimationPlayer AnimPlayer => Visuals.GetNode<AnimationPlayer>("AnimationPlayer");
    private bool _isCrouchAnimationPlaying = false;
    
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

                // Only play walk animation if not already playing
                if (!_isCrouchAnimationPlaying)
                {
                    AnimPlayer.Play("Crouch_Fwd");
                    _isCrouchAnimationPlaying = true;
                }
            }
            else
            {
                velocity.X = Mathf.MoveToward(Player.Velocity.X, 0, CrouchSpeed);
                velocity.Z = Mathf.MoveToward(Player.Velocity.Z, 0, CrouchSpeed);
                _isCrouchAnimationPlaying = false;
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

    public override void OnEnterState()
    {
        GD.Print("Entering Crouch Forward State");
        _isCrouchAnimationPlaying = false;
    }
}
