using Godot;
using System.Linq;

public partial class InputHandler : Node
{
    private CharacterController player;
    
    public override void _Ready()
    {
        player = GetParent<CharacterController>();
    }

    public InputPackage GatherInput()
    {
        InputPackage input = new InputPackage();
        
        // Only gather input if this is the local player
        if (player == null || !player.IsMultiplayerAuthority())
        {
            // Return empty input for non-authority players
            input.Actions.Append("idle");
            return input;
        }

        /*
        if (Input.IsActionJustPressed("ui_accept"))
            input.Actions.Add("jump");
        */
        
        input.InputDirection = Input.GetVector("move_forward", "move_backward", "move_right", "move_left");

        if (Input.IsActionPressed("crouch"))
        {
            input.Actions.Append("crouch");
        }
        else if (Input.IsActionPressed("crouch") && input.InputDirection != Vector2.Zero)
        {
            input.Actions.Append("crouch_fwd");
        }
        else if (Input.IsActionPressed("sprint") && input.InputDirection != Vector2.Zero)
        {
            input.Actions.Append("sprint");
        }
        else if (input.InputDirection != Vector2.Zero)
        {
            input.Actions.Append("walk");
        }
        else if (Input.IsActionPressed("dance"))
        {
            input.Actions.Append("dance");
        }

        if (input.Actions.Count == 0)
                input.Actions.Append("idle");

        return input;
    }
}
