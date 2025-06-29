using Godot;
using System.Collections.Generic;

public partial class Crouch : Move
{
    [Export] public float CrouchSpeed = 1.5f;
    public static float Gravity => (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
    private bool _isCrouching;
    private AnimationPlayer AnimPlayer => Visuals.GetNode<AnimationPlayer>("AnimationPlayer");

    public override string CheckRelevance(InputPackage input)
    {
        if (input.InputDirection != Vector2.Zero)
        {
            if (Input.IsActionPressed("crouch"))
                return "crouch_fwd";
            else
                return "idle";
        }

        return "crouch";
    }

    public override void Update(double delta, InputPackage input)
    {
        LookAtDirection();
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
        AnimPlayer.Play("Crouch_Idle");
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
