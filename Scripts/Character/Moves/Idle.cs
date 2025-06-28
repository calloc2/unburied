using Godot;
using System.Collections.Generic;

public partial class Idle : Move
{
    public override string CheckRelevance(InputPackage input)
    {
        if (Input.IsActionPressed("crouch"))
            return "crouch";
        
        if (input.InputDirection != Vector2.Zero)
        {
            if (Input.IsActionPressed("sprint"))
                return "sprint";
            else
                return "walk";
        }
        
        return "idle";
    }

    public override void OnEnterState()
    {
        Player.Velocity = Vector3.Zero;
        Player.GetNode<AnimationPlayer>("Rig/AnimationPlayer").Play("Idle");
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }
}
