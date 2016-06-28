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