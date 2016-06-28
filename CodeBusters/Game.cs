using System.Collections.Generic;

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

        var bustersDirection = new Dictionary<int, Point>();
        var sectors = new Queue<Point>();
        var stuns = new Dictionary<int, int>();

        sectors.Enqueue(new Point(8000, 6600));
        sectors.Enqueue(new Point(8000, 2200));
        sectors.Enqueue(new Point(6000, 6600));
        sectors.Enqueue(new Point(6000, 2200));
        sectors.Enqueue(new Point(4000, 6600));
        sectors.Enqueue(new Point(4000, 2200));
        sectors.Enqueue(new Point(2000, 6600));
        sectors.Enqueue(new Point(2000, 2200));

        while (true)
        {
            var turn = ReadTurn();
            var instructions = turn.Resolve(bustersDirection, sectors, stuns);

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