using Godot;
using System.Linq;

public partial class InputHandler : Node
{
    public InputPackage GatherInput()
    {
        InputPackage input = new InputPackage();

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
