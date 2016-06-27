﻿using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Send your busters out into the fog to trap ghosts and bring them home!
 **/

internal interface IConsole
{
    void WriteLine(string line);
    string ReadLine();
}

class SystemConsole : IConsole
{
    public void WriteLine(string line)
    {
        Console.WriteLine(line);
    }

    public string ReadLine()
    {
        return Console.ReadLine();
    }
}

class Game
{
    private readonly IConsole _console;

    private int _bustersPerPlayer;
    private int _ghostCount;
    private int _myTeamId;

    public Game(IConsole console)
    {
        _console = console;
    }

    public int BustersPerPlayer => _bustersPerPlayer;

    public int GhostCount => _ghostCount;

    public void RunGame()
    {
        ReadGameParameters();

        while (true)
        {
            var turn = ReadTurn();
            var instructions = turn.Resolve();

            foreach (var playerBuster in turn.PlayerBusters)
            {
                _console.WriteLine(instructions[playerBuster.Id].ToString());
            }
        }
    }

    public GameTurn ReadTurn()
    {
        var gameTurn = new GameTurn(_myTeamId, _bustersPerPlayer, _ghostCount);
        gameTurn.ReadEntities(_console);
        return gameTurn;
    }

    private void ReadGameParameters()
    {
        _bustersPerPlayer = int.Parse(_console.ReadLine());
        _ghostCount = int.Parse(_console.ReadLine());
        _myTeamId = int.Parse(_console.ReadLine());
        // if this is 0, your base is on the top left of the map, if it is one, on the bottom right
    }
}

internal class GameTurn
{
    private int _bustersPerPlayer;
    private int _ghostCount;
    private int _teamId;

    private readonly List<Ghost> _ghosts = new List<Ghost>();
    private readonly List<Buster> _teamBusters = new List<Buster>();

    public GameTurn(int teamId, int bustersPerPlayer, int ghostCount)
    {
        _teamId = teamId;
        _bustersPerPlayer = bustersPerPlayer;
        _ghostCount = ghostCount;
    }

    public IEnumerable<Buster> PlayerBusters => _teamBusters;

    public IDictionary<int, IBusterInstruction> Resolve()
    {
        var instructions = new Dictionary<int, IBusterInstruction>();

        foreach (var buster in _teamBusters)
        {
            var nextInstruction = FindNextInstruction(buster);
            instructions.Add(buster.Id, nextInstruction);
        }

        return instructions;
    }

    private IBusterInstruction FindNextInstruction(Buster buster)
    {
        if (buster.IsCarryingGhost)
        {
            var homeDist = ComputeDistanceFromHome(buster);
            if (homeDist < 1600)
                return new ReleaseInstruction();

            return new GoHomeInstruction(_teamId);
        }

        var ghostInRange = FindGhostInBustRange(buster);
        if (ghostInRange != null)
        {
            return new BustIntruction(ghostInRange.Id);
        }

        var anyGhost = _ghosts.FirstOrDefault();
        if (anyGhost != null)
            return new MoveInstruction(anyGhost.X, anyGhost.Y);

        return new MoveInstruction(Rand.Next(16000), Rand.Next(9000));
    }

    public static Random Rand { get; } = new Random();

    private double ComputeDistanceFromHome(Buster buster)
    {
        int homeX = _teamId == 0 ? 0 : 16000;
        int homeY = _teamId == 0 ? 0 : 9000;

        return ComputeDistanceFromBuster(buster, homeX, homeY);
    }

    private double ComputeDistanceFromBuster(Buster buster, int x, int y)
    {
        var distX = x - buster.X;
        var distY = y - buster.Y;

        return Math.Sqrt(distX*distX + distY*distY);
    }

    private Ghost FindGhostInBustRange(Buster buster)
    {
        foreach (var ghost in _ghosts)
        {
            var dist = ComputeDistanceFromBuster(buster, ghost.X, ghost.Y);

            if (dist > 900 && dist < 1760)
                return ghost;
        }

        return null;
    }

    public void ReadEntities(IConsole console)
    {
        var entities = int.Parse(console.ReadLine()); // the number of busters and ghosts visible to you
        for (var i = 0; i < entities; i++)
        {
            ReadEntity(console.ReadLine());
        }
    }

    private void ReadEntity(string text)
    {
        string[] inputs = text.Split(' ');
        int entityId = int.Parse(inputs[0]); // buster id or ghost id
        int x = int.Parse(inputs[1]);
        int y = int.Parse(inputs[2]); // position of this buster / ghost
        int entityType = int.Parse(inputs[3]); // the team id if it is a buster, -1 if it is a ghost.
        int state = int.Parse(inputs[4]); // For busters: 0=idle, 1=carrying a ghost.
        int value = int.Parse(inputs[5]);
        // For busters: Ghost id being carried. For ghosts: number of busters attempting to trap this ghost.

        if (entityType == -1)
            _ghosts.Add(new Ghost(entityId, x, y, value));
        else if (entityType == _teamId)
            _teamBusters.Add(new Buster(entityId, x, y, state, value));
    }
}

internal class Buster
{
    public int Id { get; }
    public int X { get; }
    public int Y { get; }

    public Buster(int id, int x, int y, int state, int ghostId)
    {
        Id = id;
        X = x;
        Y = y;
        IsCarryingGhost = (state == 1);
        CarriedGhostId = ghostId;
    }

    public int CarriedGhostId { get; }

    public bool IsCarryingGhost { get; }
}

internal class Ghost
{
    public int Id { get; }
    public int X { get; }
    public int Y { get; }
    public int BustersTrappingGhost { get; }

    public Ghost(int id, int x, int y, int bustersTrappingGhost)
    {
        Id = id;
        X = x;
        Y = y;
        BustersTrappingGhost = bustersTrappingGhost;
    }
}

internal interface IBusterInstruction
{
    string Type { get; }
}

class GoHomeInstruction : MoveInstruction
{
    public GoHomeInstruction(int teamId)
        : base(teamId == 0 ? 0 : 16000, teamId == 0 ? 0 : 9000)
    {

    }
}

class ReleaseInstruction : IBusterInstruction
{
    public string Type => "RELEASE";
    public override string ToString()
    {
        return Type;
    }
}

class BustIntruction : IBusterInstruction
{
    public int GhostId { get; }

    public BustIntruction(int ghostId)
    {
        GhostId = ghostId;
    }

    public string Type => "BUST";

    public override string ToString()
    {
        return $"{Type} {GhostId}";
    }
}

class MoveInstruction : IBusterInstruction
{
    public int X { get; }
    public int Y { get; }
    public string Type => "MOVE";

    public MoveInstruction(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"{Type} {X} {Y}";
    }
}

class Player
{
    static void Main(string[] args)
    {
        new Game(new SystemConsole()).RunGame();
    }
}