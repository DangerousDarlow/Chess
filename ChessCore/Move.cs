namespace ChessCore;

public record Move
{
    private readonly byte _from;
    private readonly byte _to;

    public Move(MoveType type, byte from, byte to)
    {
        Type = type;
        _from = from;
        _to = to;
    }

    public MoveType Type { get; }

    public string From => _from.PositionAsAlgebraicNotation();

    public string To => _to.PositionAsAlgebraicNotation();

    public override string ToString() => $"{From} > {To} : {Type}";
}

public enum MoveType : byte
{
    Move,
    Capture,
    EnPassant,
    Castle,
    Promotion
}