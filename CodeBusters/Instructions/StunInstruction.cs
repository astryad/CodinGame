internal class StunInstruction : IBusterInstruction
{
    public int Id { get; }

    public StunInstruction(int id)
    {
        Id = id;
    }

    public string Type => "STUN";

    public override string ToString()
    {
        return $"{Type} {Id}";
    }
}