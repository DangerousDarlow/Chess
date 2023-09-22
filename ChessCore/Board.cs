using System.Text;

namespace ChessCore;

public class Board
{
    public const int Size = 8;

    private readonly byte[] _board = new byte[Size * Size];

    private Board(string fen)
    {
        FromForsythEdwardsNotation(fen);
    }

    private Board()
    {
        this["a1"] = Piece.WhiteRook;
        this["b1"] = Piece.WhiteKnight;
        this["c1"] = Piece.WhiteBishop;
        this["d1"] = Piece.WhiteQueen;
        this["e1"] = Piece.WhiteKing;
        this["f1"] = Piece.WhiteBishop;
        this["g1"] = Piece.WhiteKnight;
        this["h1"] = Piece.WhiteRook;
        this["a2"] = Piece.WhitePawn;
        this["b2"] = Piece.WhitePawn;
        this["c2"] = Piece.WhitePawn;
        this["d2"] = Piece.WhitePawn;
        this["e2"] = Piece.WhitePawn;
        this["f2"] = Piece.WhitePawn;
        this["g2"] = Piece.WhitePawn;
        this["h2"] = Piece.WhitePawn;
        this["a7"] = Piece.BlackPawn;
        this["b7"] = Piece.BlackPawn;
        this["c7"] = Piece.BlackPawn;
        this["d7"] = Piece.BlackPawn;
        this["e7"] = Piece.BlackPawn;
        this["f7"] = Piece.BlackPawn;
        this["g7"] = Piece.BlackPawn;
        this["h7"] = Piece.BlackPawn;
        this["a8"] = Piece.BlackRook;
        this["b8"] = Piece.BlackKnight;
        this["c8"] = Piece.BlackBishop;
        this["d8"] = Piece.BlackQueen;
        this["e8"] = Piece.BlackKing;
        this["f8"] = Piece.BlackBishop;
        this["g8"] = Piece.BlackKnight;
        this["h8"] = Piece.BlackRook;
    }

    public Colour ActiveColour { get; private set; } = Colour.White;

    public int FullMoveNumber { get; private set; } = 1;

    public int HalfMoveClock { get; private set; }

    public Piece? this[string position]
    {
        get => _board[position.PositionAsByte()].PieceFromByte();
        set => _board[position.PositionAsByte()] = value.ByteFromPiece();
    }

    private Piece? this[byte position] => _board[position].PieceFromByte();

    public static Board CreateWithNewGameSetup() => new();

    public static Board CreateFromForsythEdwardsNotation(string fen) => new(fen);

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

                stringBuilder.Append(Piece.ToForsythEdwardsNotation(piece));
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

    public void FromForsythEdwardsNotation(string fen)
    {
        Array.Clear(_board);

        var parts = fen.Split(' ');

        var board = parts[0];
        byte rank = Size;
        byte file = 1;
        foreach (var positionCharacter in board)
        {
            if (positionCharacter == '/')
            {
                --rank;
                file = 1;
                continue;
            }

            if (char.IsDigit(positionCharacter))
            {
                file += (byte) char.GetNumericValue(positionCharacter);
                continue;
            }

            var index = BoardExtensions.IndexFromRankAndFile(rank, file);
            _board[index] = Piece.FromForsythEdwardsNotation(positionCharacter).ByteValue;
            ++file;
        }

        var colour = parts[1];
        ActiveColour = colour switch
        {
            "w" => Colour.White,
            "b" => Colour.Black,
            _ => throw new ArgumentException($"Invalid colour {colour}")
        };

        var halfMoveClock = parts[4];
        HalfMoveClock = int.Parse(halfMoveClock);

        var fullMoveNumber = parts[5];
        FullMoveNumber = int.Parse(fullMoveNumber);
    }

    public override string ToString() => ToForsythEdwardsNotation();

    public List<Move> GetValidMovesForColour(Colour colour)
    {
        var moves = new List<Move>();

        for (byte index = 0; index < _board.Length; ++index)
        {
            var piece = _board[index].PieceFromByte();
            if (piece == null || piece.Colour != colour)
                continue;

            var (rank, file) = BoardExtensions.RankAndFileFromIndex(index);
            var pieceMoves = piece.Type switch
            {
                PieceType.Pawn => GetValidPawnMoves(colour, index)
            };
            
            moves.AddRange(pieceMoves);
        }

        return moves;
    }
    
    private IEnumerable<Move> GetValidPawnMoves(Colour colour, byte position)
    {
        var (rank, file) = BoardExtensions.RankAndFileFromIndex(position);

        var rankAdvance = (byte) (colour == Colour.White ? rank + 1 : rank - 1);
        if (IsRankOrFileInBounds(rankAdvance) is false)
            return Enumerable.Empty<Move>();

        var moves = new List<Move>();

        var indexAdvance = BoardExtensions.IndexFromRankAndFile(rankAdvance, file);
        if (this[indexAdvance] is null)
            moves.Add(new Move(position, indexAdvance));

        return moves;
    }

    private static bool IsRankOrFileInBounds(byte rankOrFile) => rankOrFile is >= 1 and <= Size;
}

public static class BoardExtensions
{
    public static Piece? PieceFromByte(this byte piece) => piece switch
    {
        0 => null,
        1 => Piece.BlackPawn,
        2 => Piece.BlackBishop,
        3 => Piece.BlackKnight,
        4 => Piece.BlackRook,
        5 => Piece.BlackQueen,
        6 => Piece.BlackKing,
        7 => Piece.WhitePawn,
        8 => Piece.WhiteBishop,
        9 => Piece.WhiteKnight,
        10 => Piece.WhiteRook,
        11 => Piece.WhiteQueen,
        12 => Piece.WhiteKing,
        _ => throw new ArgumentException($"Invalid piece byte {piece}")
    };

    public static byte ByteFromPiece(this Piece? piece) => piece?.ByteValue ?? 0;

    public static byte IndexFromRankAndFile(byte rank, byte file) => (byte) ((rank - 1) * Board.Size + (file - 1));

    public static (byte rank, byte file) RankAndFileFromIndex(byte index) => ((byte) (index / Board.Size + 1), (byte) (index % Board.Size + 1));
}