using Godot;
using System.Collections.Generic;

public partial class Idle : Move
{
    public override string CheckRelevance(InputPackage input)
    {
        if (Input.IsActionPressed("dance"))
            return "dance";

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

    public override void Update(double delta, InputPackage input)
    {
        base.Update(delta, input);
        LookAtDirection();
        Player.MoveAndSlide();
    }

    public override void OnEnterState()
    {
        Player.GetNode<AnimationPlayer>("Rig/AnimationPlayer").Play("Idle");
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }
    
    private void LookAtDirection()
    {
        // Get the mouse position in the viewport
        Vector2 mousePos = Player.GetViewport().GetMousePosition();

        // Get the camera
        Camera3D camera = Player.GetViewport().GetCamera3D();

        // Cast a ray from the camera through the mouse position onto the XZ plane (Y = Player.GlobalPosition.Y)
        Plane groundPlane = new Plane(Vector3.Up, Player.GlobalPosition.Y);
        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePos);
        Vector3 rayDir = camera.ProjectRayNormal(mousePos);

        Vector3? intersection = groundPlane.IntersectsRay(rayOrigin, rayDir);
        if (intersection != null)
        {
            Vector3 target = intersection.Value;
            Vector3 lookDir = (target - Player.GlobalPosition);
            lookDir.Y = 0;
            if (lookDir.Length() > 0.01f)
            {
                Visuals.LookAt(Player.GlobalPosition + -lookDir, Vector3.Up);
            }
        }
    }
}
