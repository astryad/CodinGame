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