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