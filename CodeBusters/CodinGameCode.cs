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

        // game loop
        while (true)
        {
            ProcessTurn();
        }
    }

    public void ProcessTurn()
    {
        int entities = int.Parse(_console.ReadLine()); // the number of busters and ghosts visible to you
        for (int i = 0; i < entities; i++)
        {
            string[] inputs = _console.ReadLine().Split(' ');
            int entityId = int.Parse(inputs[0]); // buster id or ghost id
            int x = int.Parse(inputs[1]);
            int y = int.Parse(inputs[2]); // position of this buster / ghost
            int entityType = int.Parse(inputs[3]); // the team id if it is a buster, -1 if it is a ghost.
            int state = int.Parse(inputs[4]); // For busters: 0=idle, 1=carrying a ghost.
            int value = int.Parse(inputs[5]);
            // For busters: Ghost id being carried. For ghosts: number of busters attempting to trap this ghost.
        }
        for (int i = 0; i < _bustersPerPlayer; i++)
        {
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            _console.WriteLine("MOVE 8000 4500");
        }
    }

    private void ReadGameParameters()
    {
        _bustersPerPlayer = int.Parse(_console.ReadLine());
        _ghostCount = int.Parse(_console.ReadLine());
        _myTeamId = int.Parse(_console.ReadLine());
        // if this is 0, your base is on the top left of the map, if it is one, on the bottom right
    }
}

class Player
{
    static void Main(string[] args)
    {
        new Game(new SystemConsole()).RunGame();
    }
}