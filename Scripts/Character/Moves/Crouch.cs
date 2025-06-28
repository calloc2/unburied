using Godot;
using System.Collections.Generic;

public partial class Crouch : Move
{
    [Export] public float CrouchSpeed = 1.5f;
    public static float Gravity => (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    private Node3D Visuals => Player.GetNode<Node3D>("Rig");
    private AnimationPlayer AnimPlayer => Visuals.GetNode<AnimationPlayer>("AnimationPlayer");

    public override string CheckRelevance(InputPackage input)
    {
        if (Input.IsActionPressed("crouch") && input.InputDirection != Vector2.Zero)
            return "crouch_fwd";
        if (input.InputDirection != Vector2.Zero)
        {
            if (Input.IsActionPressed("sprint"))
                return "sprint";
            else
                return "walk";
        }
        
        return "crouch";
    }

    public override void Update(double delta, InputPackage input)
    {
        Player.Velocity = new Vector3(0, Player.Velocity.Y, 0);
        
        if (!Player.IsOnFloor())
        {
            float gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
            Player.Velocity = new Vector3(0, Player.Velocity.Y - gravity * (float)delta, 0);
        }
        else
        {
            Player.Velocity = new Vector3(0, -0.1f, 0);
        }
        
        Player.MoveAndSlide();
    }

    public override void OnEnterState()
    {
        GD.Print("Entering Crouch Idle State");
        AnimPlayer.Play("Crouch_Idle");
    }
}
