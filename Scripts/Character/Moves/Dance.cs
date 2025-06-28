using Godot;

public partial class Dance : Move
{
    public override string CheckRelevance(InputPackage input)
    {
        if (input.InputDirection != Vector2.Zero)
            {
                if (Input.IsActionPressed("sprint"))
                    return "sprint";
                else
                    return "walk";
            }
        
        return "dance";
    }

    public override void OnEnterState()
    {
        Player.Velocity = Vector3.Zero;
        Player.GetNode<AnimationPlayer>("Rig/AnimationPlayer").Play("Dance");
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }
}
