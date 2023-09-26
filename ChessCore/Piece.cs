namespace ChessCore;

public record Piece
{
    private Piece(Colour colour, PieceType type)
    {
        ByteValue = (colour, type) switch
        {
            (Colour.Black, PieceType.Pawn) => PieceTypeInternal.BlackPawn,
            (Colour.Black, PieceType.Bishop) => PieceTypeInternal.BlackBishop,
            (Colour.Black, PieceType.Knight) => PieceTypeInternal.BlackKnight,
            (Colour.Black, PieceType.Rook) => PieceTypeInternal.BlackRook,
            (Colour.Black, PieceType.Queen) => PieceTypeInternal.BlackQueen,
            (Colour.Black, PieceType.King) => PieceTypeInternal.BlackKing,
            (Colour.White, PieceType.Pawn) => PieceTypeInternal.WhitePawn,
            (Colour.White, PieceType.Bishop) => PieceTypeInternal.WhiteBishop,
            (Colour.White, PieceType.Knight) => PieceTypeInternal.WhiteKnight,
            (Colour.White, PieceType.Rook) => PieceTypeInternal.WhiteRook,
            (Colour.White, PieceType.Queen) => PieceTypeInternal.WhiteQueen,
            (Colour.White, PieceType.King) => PieceTypeInternal.WhiteKing,
            _ => throw new ArgumentException($"Invalid piece colour {colour} and type {type}")
        };
    }

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

    public PieceTypeInternal ByteValue { get; }

    public Colour Colour => ByteValue switch
    {
        PieceTypeInternal.BlackPawn => Colour.Black,
        PieceTypeInternal.BlackBishop => Colour.Black,
        PieceTypeInternal.BlackKnight => Colour.Black,
        PieceTypeInternal.BlackRook => Colour.Black,
        PieceTypeInternal.BlackQueen => Colour.Black,
        PieceTypeInternal.BlackKing => Colour.Black,
        _ => Colour.White
    };

    public PieceType Type => ByteValue switch
    {
        PieceTypeInternal.BlackBishop => PieceType.Bishop,
        PieceTypeInternal.WhiteBishop => PieceType.Bishop,
        PieceTypeInternal.BlackKnight => PieceType.Knight,
        PieceTypeInternal.WhiteKnight => PieceType.Knight,
        PieceTypeInternal.BlackRook => PieceType.Rook,
        PieceTypeInternal.WhiteRook => PieceType.Rook,
        PieceTypeInternal.BlackQueen => PieceType.Queen,
        PieceTypeInternal.WhiteQueen => PieceType.Queen,
        PieceTypeInternal.BlackKing => PieceType.King,
        PieceTypeInternal.WhiteKing => PieceType.King,
        _ => PieceType.Pawn
    };

    public static char ToForsythEdwardsNotation(Piece piece)
    {
        var character = piece.Type switch
        {
            PieceType.Bishop => 'b',
            PieceType.Knight => 'n',
            PieceType.Rook => 'r',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => 'p'
        };

        return piece.Colour == Colour.White ? char.ToUpper(character) : character;
    }

    public static Piece FromForsythEdwardsNotation(char piece) => piece switch
    {
        'p' => BlackPawn,
        'b' => BlackBishop,
        'n' => BlackKnight,
        'r' => BlackRook,
        'q' => BlackQueen,
        'k' => BlackKing,
        'P' => WhitePawn,
        'B' => WhiteBishop,
        'N' => WhiteKnight,
        'R' => WhiteRook,
        'Q' => WhiteQueen,
        'K' => WhiteKing,
        _ => throw new ArgumentException($"Invalid piece notation '{piece}'")
    };

    public override string ToString() => $"{Colour} {Type}";
}

public enum PieceTypeInternal : byte
{
    None = 0,
    BlackPawn,
    BlackBishop,
    BlackKnight,
    BlackRook,
    BlackQueen,
    BlackKing,
    WhitePawn,
    WhiteBishop,
    WhiteKnight,
    WhiteRook,
    WhiteQueen,
    WhiteKing
}

public enum PieceType : byte
{
    Pawn,
    Bishop,
    Knight,
    Rook,
    Queen,
    King
}