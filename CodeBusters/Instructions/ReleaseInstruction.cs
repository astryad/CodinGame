class ReleaseInstruction : IBusterInstruction
{
    public string Type => "RELEASE";
    public override string ToString()
    {
        return Type;
    }
}