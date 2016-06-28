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