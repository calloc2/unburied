using Godot;
using System.Collections.Generic;

public abstract partial class Move : Node
{
    public CharacterController Player;

    public static Dictionary<string, int> movesPriorities = new Dictionary<string, int>
    {
        { "idle", 1 },
        { "walk", 2 },
        { "sprint", 3 },
        { "crouch", 4 },
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
        //
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
