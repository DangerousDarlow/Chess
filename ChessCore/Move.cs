namespace ChessCore;

public record Move(Position From, Position To)
{
    public override string ToString() => $"{From} > {To}";
}