internal struct Point
{
    public static Point Default => new Point(-1,-1);

    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X*397) ^ Y;
        }
    }

    public static bool operator ==(Point left, Point right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point left, Point right)
    {
        return !left.Equals(right);
    }
}