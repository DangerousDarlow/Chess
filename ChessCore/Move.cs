namespace ChessCore;

public record Move(Position From, Position To, MoveType Type = MoveType.Move)
{
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