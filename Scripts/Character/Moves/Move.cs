using Godot;
using System.Collections.Generic;

public abstract partial class Move : Node
{
    public CharacterController Player;
    public Node3D Visuals => Player.GetNode<Node3D>("Rig");

    public static Dictionary<string, int> movesPriorities = new Dictionary<string, int>
    {
        { "idle", 1 },
        { "walk", 2 },
        { "sprint", 3 },
        { "crouch", 4 },
        { "crouch_fwd", 5},
        { "dance", 8},
        { "jump", 10 }
    };

    public static bool MovesPrioritySort(string a, string b)
    {
        return movesPriorities[a] > movesPriorities[b];
    }

    public virtual string CheckRelevance(InputPackage input)
    {
        return "error";
    }

    public virtual void Update(double delta, InputPackage input)
    {
        // Default implementation does nothing
        // Derived classes should override this method to implement specific behavior
    }

    public virtual void OnEnterState()
    {
        //
    }

    public virtual void OnExitState()
    {
        //
    }
}
