using System.Text;

namespace ChessCore;

public class Board
{
    public const int Size = 8;

    private readonly byte[] _board = new byte[Size * Size];

    public Colour ActiveColour => Colour.White;

    public int FullMoveNumber => 1;

    public int HalfMoveClock => 0;

    public Board()
    {
        this["a1"] = Pieces.WhiteRook;
        this["b1"] = Pieces.WhiteKnight;
        this["c1"] = Pieces.WhiteBishop;
        this["d1"] = Pieces.WhiteQueen;
        this["e1"] = Pieces.WhiteKing;
        this["f1"] = Pieces.WhiteBishop;
        this["g1"] = Pieces.WhiteKnight;
        this["h1"] = Pieces.WhiteRook;
        this["a2"] = Pieces.WhitePawn;
        this["b2"] = Pieces.WhitePawn;
        this["c2"] = Pieces.WhitePawn;
        this["d2"] = Pieces.WhitePawn;
        this["e2"] = Pieces.WhitePawn;
        this["f2"] = Pieces.WhitePawn;
        this["g2"] = Pieces.WhitePawn;
        this["h2"] = Pieces.WhitePawn;
        this["a7"] = Pieces.BlackPawn;
        this["b7"] = Pieces.BlackPawn;
        this["c7"] = Pieces.BlackPawn;
        this["d7"] = Pieces.BlackPawn;
        this["e7"] = Pieces.BlackPawn;
        this["f7"] = Pieces.BlackPawn;
        this["g7"] = Pieces.BlackPawn;
        this["h7"] = Pieces.BlackPawn;
        this["a8"] = Pieces.BlackRook;
        this["b8"] = Pieces.BlackKnight;
        this["c8"] = Pieces.BlackBishop;
        this["d8"] = Pieces.BlackQueen;
        this["e8"] = Pieces.BlackKing;
        this["f8"] = Pieces.BlackBishop;
        this["g8"] = Pieces.BlackKnight;
        this["h8"] = Pieces.BlackRook;
    }

    public Piece? this[string position]
    {
        get => _board[position.PositionAsByte()].PieceFromByte();
        set => _board[position.PositionAsByte()] = value.ByteFromPiece();
    }

    public string ToForsythEdwardsNotation()
    {
        var stringBuilder = new StringBuilder();

        for (byte rank = Size; rank >= 1; --rank)
        {
            var emptyCount = 0;

            for (byte file = 1; file <= Size; ++file)
            {
                var piece = _board[BoardExtensions.IndexFromRankAndFile(rank, file)].PieceFromByte();
                if (piece == null)
                {
                    ++emptyCount;
                    continue;
                }

                if (emptyCount > 0)
                {
                    stringBuilder.Append(emptyCount);
                    emptyCount = 0;
                }

                stringBuilder.Append(piece.ForsythEdwardsNotation);
            }

            if (emptyCount > 0)
                stringBuilder.Append(emptyCount);

            if (rank > 1)
                stringBuilder.Append('/');
        }

        var colourChar = ActiveColour.ToString().ToLower().First();
        stringBuilder.Append($" {colourChar} KQkq - {HalfMoveClock} {FullMoveNumber}");
        return stringBuilder.ToString();
    }
}

public static class BoardExtensions
{
    public static Piece? PieceFromByte(this byte piece) => piece switch
    {
        0 => null,
        1 => Pieces.BlackPawn,
        2 => Pieces.BlackBishop,
        3 => Pieces.BlackKnight,
        4 => Pieces.BlackRook,
        5 => Pieces.BlackQueen,
        6 => Pieces.BlackKing,
        7 => Pieces.WhitePawn,
        8 => Pieces.WhiteBishop,
        9 => Pieces.WhiteKnight,
        10 => Pieces.WhiteRook,
        11 => Pieces.WhiteQueen,
        12 => Pieces.WhiteKing,
        _ => throw new ArgumentException($"Invalid piece byte {piece}")
    };

    public static byte ByteFromPiece(this Piece? piece) => piece?.ByteValue ?? 0;

    public static byte IndexFromRankAndFile(byte rank, byte file) => (byte) ((rank - 1) * Board.Size + (file - 1));
}