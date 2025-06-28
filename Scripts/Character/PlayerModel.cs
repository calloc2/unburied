using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerModel : Node
{
    // Private:
    private Move _currentMove;

    // Public:
    public CharacterController Player => GetParent<CharacterController>();
    public Dictionary<string, Move> Moves;

    public override void _Ready()
    {
        Moves = new Dictionary<string, Move>
        {
            { "idle", (Move)GetNode("Idle") },
            { "walk", (Move)GetNode("Walk") },
            { "sprint", (Move)GetNode("Sprint") },
            { "crouch", (Move)GetNode("Crouch") },
            { "crouch_fwd", (Move)GetNode("CrouchFwd") },
        };

        _currentMove = Moves["idle"];

        // Corrigido: atribuir Player corretamente para cada Move
        foreach (var move in Moves.Values)
        {
            move.Player = Player;
        }
    }

    public void Update(double delta, InputPackage input)
    {
        var relevance = _currentMove.CheckRelevance(input);

        if (relevance != "okay" && Moves.ContainsKey(relevance))
        {
            SwitchTo(relevance);
        }

        _currentMove.Update(delta, input);
    }

    public void SwitchTo(string state)
    {
        _currentMove.OnExitState();
        _currentMove = Moves[state];
        _currentMove.OnEnterState();
    }
}
