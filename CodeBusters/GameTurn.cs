using System;
using System.Collections.Generic;
using System.Linq;

internal class GameTurn
{
    private int _bustersPerPlayer;
    private int _ghostCount;
    private readonly int _teamId;

    private readonly List<Ghost> _ghosts = new List<Ghost>();
    private readonly List<Buster> _teamBusters = new List<Buster>();
    private readonly List<Buster> _enemyBusters = new List<Buster>();

    public GameTurn(int teamId, int bustersPerPlayer, int ghostCount)
    {
        _teamId = teamId;
        _bustersPerPlayer = bustersPerPlayer;
        _ghostCount = ghostCount;
    }

    public IEnumerable<Buster> PlayerBusters => _teamBusters;

    public IDictionary<int, IBusterInstruction> Resolve(Dictionary<int, Point> bustersDirection, Queue<Point> sectors, Dictionary<int, int> stuns)
    {
        var instructions = new Dictionary<int, IBusterInstruction>();

        foreach (var buster in _teamBusters)
        {
            if(!bustersDirection.ContainsKey(buster.Id))
                bustersDirection.Add(buster.Id, Point.Default);

            if (!stuns.ContainsKey(buster.Id))
                stuns.Add(buster.Id, 0);
            else
            {
                stuns[buster.Id] = Math.Max(stuns[buster.Id] - 1, 0);
            }

            var nextInstruction = FindNextInstruction(buster, bustersDirection, sectors, stuns);
            instructions.Add(buster.Id, nextInstruction);
        }

        return instructions;
    }

    private IBusterInstruction FindNextInstruction(Buster buster, Dictionary<int, Point> bustersDirection, Queue<Point> sectors, Dictionary<int, int> stuns)
    {
        if (buster.IsCarryingGhost)
        {
            var homeDist = ComputeDistanceFromHome(buster);
            if (homeDist < 1600)
                return new ReleaseInstruction();

            return new GoHomeInstruction(_teamId);
        }

        var enemyBuster = FindEnemyStunTarget(buster);
        if (enemyBuster != null && stuns[buster.Id] == 0)
        {
            stuns[buster.Id] = 20;
            return new StunInstruction(enemyBuster.Id);
        }

        var ghostInRange = FindGhostInBustRange(buster);
        if (ghostInRange != null)
        {
            return new BustIntruction(ghostInRange.Id);
        }

        var anyGhost = _ghosts.FirstOrDefault();
        if (anyGhost != null)
            return new MoveInstruction(anyGhost.X, anyGhost.Y);

        var direction = bustersDirection[buster.Id];
        var distDirection = ComputeDistanceFromBuster(buster, direction.X, direction.Y);
        if (distDirection < 100 || direction == Point.Default)
        {
            var nextDirection = sectors.Dequeue();
            sectors.Enqueue(direction);
            bustersDirection[buster.Id] = nextDirection;
            direction = nextDirection;
        }

        return new MoveInstruction(direction.X, direction.Y);
    }

    private Buster FindEnemyStunTarget(Buster buster)
    {
        return _enemyBusters.FirstOrDefault(
            enemy => enemy.IsCarryingGhost && ComputeDistanceFromBuster(buster, enemy.X, enemy.Y) < 1760);
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
        else
            _enemyBusters.Add(new Buster(entityId, x, y, state, value));
    }
}