using Godot;
using System.Collections.Generic;

public partial class Crouch : Move
{
    [Export] public float CrouchSpeed = 1.5f;
    public static float Gravity => (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    private Node3D Visuals => Player.GetNode<Node3D>("Rig");
    private AnimationPlayer AnimPlayer => Visuals.GetNode<AnimationPlayer>("AnimationPlayer");
    private bool _wasMoving = false;

    public override string CheckRelevance(InputPackage input)
    {
        // If not crouching anymore
        if (!Input.IsActionPressed("crouch"))
        {
            if (input.InputDirection == Vector2.Zero)
                return "idle";
            else if (Input.IsActionPressed("sprint"))
                return "sprint";
            else
                return "walk";
        }
        
        return "crouch";
    }

    public override void Update(double delta, InputPackage input)
    {
        Player.Velocity = CalculateVelocity(input, (float)delta);
        Player.MoveAndSlide();
        
        bool isMoving = input.InputDirection != Vector2.Zero;
        
        if (isMoving != _wasMoving)
        {
            if (isMoving)
            {
                GD.Print("Playing Crouch_Fwd animation");
                if (AnimPlayer.HasAnimation("Crouch_Fwd"))
                {
                    AnimPlayer.Play("Crouch_Fwd");
                }
                else
                {
                    GD.Print("ERROR: Crouch_Fwd animation not found!");
                    if (AnimPlayer.HasAnimation("Walk"))
                    {
                        AnimPlayer.Play("Walk");
                        GD.Print("Fallback: Playing Walk animation");
                    }
                }
            }
            else
            {
                GD.Print("Playing Crouch_Idle animation");
                if (AnimPlayer.HasAnimation("Crouch_Idle"))
                {
                    AnimPlayer.Play("Crouch_Idle");
                }
                else
                {
                    GD.Print("ERROR: Crouch_Idle animation not found!");
                    if (AnimPlayer.HasAnimation("Idle"))
                    {
                        AnimPlayer.Play("Idle");
                        GD.Print("Fallback: Playing Idle animation");
                    }
                }
            }
                
            _wasMoving = isMoving;
        }

        if (AnimPlayer.CurrentAnimation != null)
        {
            GD.Print($"Current animation: {AnimPlayer.CurrentAnimation}, Playing: {AnimPlayer.IsPlaying()}");
        }
    }

    private Vector3 CalculateVelocity(InputPackage input, float delta)
    {
        Vector3 velocity = Player.Velocity;
        Vector2 inputDir = input.InputDirection;

        if (Player.IsOnFloor())
        {
            if (inputDir != Vector2.Zero)
            {
                float angle = Mathf.DegToRad(45);
                Vector2 rotatedInput = new Vector2(
                    inputDir.X * Mathf.Cos(angle) - inputDir.Y * Mathf.Sin(angle),
                    inputDir.X * Mathf.Sin(angle) + inputDir.Y * Mathf.Cos(angle)
                );

                Vector3 direction = new Vector3(rotatedInput.X, 0, rotatedInput.Y).Normalized();
                
                velocity.X = direction.X * CrouchSpeed;
                velocity.Z = direction.Z * CrouchSpeed;
                
                Visuals.LookAt(-direction + Player.GlobalTransform.Origin, Vector3.Up);
            }
            else
            {
                velocity.X = Mathf.MoveToward(Player.Velocity.X, 0, CrouchSpeed);
                velocity.Z = Mathf.MoveToward(Player.Velocity.Z, 0, CrouchSpeed);
            }
            velocity.Y = -0.1f;
        }
        else
        {
            velocity.Y -= Gravity * delta;
        }

        return velocity;
    }

    public override void OnEnterState()
    {
        GD.Print("Entering Crouch state - Playing Crouch_Idle");
        if (AnimPlayer.HasAnimation("Crouch_Idle"))
        {
            AnimPlayer.Play("Crouch_Idle");
        }
        else
        {
            GD.Print("ERROR: Crouch_Idle animation not found!");
            if (AnimPlayer.HasAnimation("Idle"))
            {
                AnimPlayer.Play("Idle");
                GD.Print("Fallback: Playing Idle animation");
            }
        }
        
        _wasMoving = false;
    }
}
