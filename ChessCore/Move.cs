namespace ChessCore;

public record Move
{
    private readonly byte _from;
    private readonly byte _to;

    public Move(byte from, byte to)
    {
        _from = from;
        _to = to;
    }

    public Move(string from, string to)
    {
        _from = from.PositionAsByte();
        _to = to.PositionAsByte();
    }

    public string From => _from.PositionAsAlgebraicNotation();

    public string To => _to.PositionAsAlgebraicNotation();

    public override string ToString() => $"{From} to {To}";
}