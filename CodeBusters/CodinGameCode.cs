using System;
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
            instructions.Add(buster.Id, new MoveInstruction(8000, 4500));
        }

        return instructions;
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
    public string Type => "BUST";

    public override string ToString()
    {
        return Type;
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