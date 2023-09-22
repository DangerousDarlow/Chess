namespace ChessCore;

public static class Pieces
{
    public static Piece BlackPawn => new(Colour.Black, PieceType.Pawn);
    public static Piece BlackBishop => new(Colour.Black, PieceType.Bishop);
    public static Piece BlackKnight => new(Colour.Black, PieceType.Knight);
    public static Piece BlackRook => new(Colour.Black, PieceType.Rook);
    public static Piece BlackQueen => new(Colour.Black, PieceType.Queen);
    public static Piece BlackKing => new(Colour.Black, PieceType.King);
    public static Piece WhitePawn => new(Colour.White, PieceType.Pawn);
    public static Piece WhiteBishop => new(Colour.White, PieceType.Bishop);
    public static Piece WhiteKnight => new(Colour.White, PieceType.Knight);
    public static Piece WhiteRook => new(Colour.White, PieceType.Rook);
    public static Piece WhiteQueen => new(Colour.White, PieceType.Queen);
    public static Piece WhiteKing => new(Colour.White, PieceType.King);
}

public record Piece
{
    public Piece(Colour colour, PieceType type)
    {
        ByteValue = (colour, type) switch
        {
            (Colour.Black, PieceType.Pawn) => 1,
            (Colour.Black, PieceType.Bishop) => 2,
            (Colour.Black, PieceType.Knight) => 3,
            (Colour.Black, PieceType.Rook) => 4,
            (Colour.Black, PieceType.Queen) => 5,
            (Colour.Black, PieceType.King) => 6,
            (Colour.White, PieceType.Pawn) => 7,
            (Colour.White, PieceType.Bishop) => 8,
            (Colour.White, PieceType.Knight) => 9,
            (Colour.White, PieceType.Rook) => 10,
            (Colour.White, PieceType.Queen) => 11,
            (Colour.White, PieceType.King) => 12,
            _ => throw new ArgumentException($"Invalid piece colour {colour} and type {type}")
        };
    }

    public byte ByteValue { get; }

    public Colour Colour => ByteValue switch
    {
        1 => Colour.Black,
        2 => Colour.Black,
        3 => Colour.Black,
        4 => Colour.Black,
        5 => Colour.Black,
        6 => Colour.Black,
        _ => Colour.White
    };

    public PieceType Type => ByteValue switch
    {
        2 => PieceType.Bishop,
        3 => PieceType.Knight,
        4 => PieceType.Rook,
        5 => PieceType.Queen,
        6 => PieceType.King,
        8 => PieceType.Bishop,
        9 => PieceType.Knight,
        10 => PieceType.Rook,
        11 => PieceType.Queen,
        12 => PieceType.King,
        _ => PieceType.Pawn
    };

    public char ForsythEdwardsNotation
    {
        get
        {
            var character = Type switch
            {
                PieceType.Bishop => 'b',
                PieceType.Knight => 'n',
                PieceType.Rook => 'r',
                PieceType.Queen => 'q',
                PieceType.King => 'k',
                _ => 'p'
            };

            return Colour == Colour.White ? char.ToUpper(character) : char.ToLower(character);
        }
    }

    public override string ToString() => $"{Colour} {Type}";
}

public enum PieceType
{
    Pawn,
    Bishop,
    Knight,
    Rook,
    Queen,
    King
}