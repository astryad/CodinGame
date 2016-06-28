class GoHomeInstruction : MoveInstruction
{
    public GoHomeInstruction(int teamId)
        : base(teamId == 0 ? 0 : 16000, teamId == 0 ? 0 : 9000)
    {

    }
}