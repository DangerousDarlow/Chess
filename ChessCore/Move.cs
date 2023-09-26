namespace ChessCore;

public record Move
{
    private readonly byte _from;
    private readonly byte _to;
    private readonly MoveType _type;

    public Move(MoveType type, byte from, byte to)
    {
        _type = type;
        _from = from;
        _to = to;
    }
    
    public MoveType Type => _type;

    public string From => _from.PositionAsAlgebraicNotation();

    public string To => _to.PositionAsAlgebraicNotation();

    public override string ToString() => $"{From} > {To} : {_type}";
}

public enum MoveType : byte
{
    Move,
    Capture,
    EnPassant,
    Castle,
    Promotion
}