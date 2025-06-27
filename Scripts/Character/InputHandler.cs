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
        
        if (input.InputDirection != Vector2.Zero)
            input.Actions.Append("walk");

        if (input.Actions.Count == 0)
            input.Actions.Append("idle");

        input.InputDirection = Input.GetVector("move_forward", "move_backward", "move_right", "move_left");

        return input;
    }
}
