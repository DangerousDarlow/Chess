using System.Text;
using static ChessCore.Position;

namespace ChessCore;

public class Board
{
    private const int Size = 8;

    private const int IndexSize = Size * Size;

    /// <summary>
    ///     There are 32 chess pieces so half this array will be empty. An array of pieces was considered as an
    ///     alternative however position and piece bytes would need to be stored therefore the array would be
    ///     the same size. In addition querying the piece at a location would require iteration.
    /// </summary>
    private readonly PieceTypeInternal[] _board = new PieceTypeInternal[IndexSize];

    private Board(string fen)
    {
        FromForsythEdwardsNotation(fen);
    }

    private Board()
    {
        this[a1] = Piece.WhiteRook;
        this[b1] = Piece.WhiteKnight;
        this[c1] = Piece.WhiteBishop;
        this[d1] = Piece.WhiteQueen;
        this[e1] = Piece.WhiteKing;
        this[f1] = Piece.WhiteBishop;
        this[g1] = Piece.WhiteKnight;
        this[h1] = Piece.WhiteRook;
        this[a2] = Piece.WhitePawn;
        this[b2] = Piece.WhitePawn;
        this[c2] = Piece.WhitePawn;
        this[d2] = Piece.WhitePawn;
        this[e2] = Piece.WhitePawn;
        this[f2] = Piece.WhitePawn;
        this[g2] = Piece.WhitePawn;
        this[h2] = Piece.WhitePawn;
        this[a7] = Piece.BlackPawn;
        this[b7] = Piece.BlackPawn;
        this[c7] = Piece.BlackPawn;
        this[d7] = Piece.BlackPawn;
        this[e7] = Piece.BlackPawn;
        this[f7] = Piece.BlackPawn;
        this[g7] = Piece.BlackPawn;
        this[h7] = Piece.BlackPawn;
        this[a8] = Piece.BlackRook;
        this[b8] = Piece.BlackKnight;
        this[c8] = Piece.BlackBishop;
        this[d8] = Piece.BlackQueen;
        this[e8] = Piece.BlackKing;
        this[f8] = Piece.BlackBishop;
        this[g8] = Piece.BlackKnight;
        this[h8] = Piece.BlackRook;
    }

    public bool WhiteTurn { get; private set; } = true;

    public ushort FullMoveNumber { get; private set; } = 1;

    public ushort HalfMoveClock { get; private set; }

    public Piece? this[Position position]
    {
        get => _board[(byte) position].ToPiece();
        private init => _board[(byte) position] = value.ToPieceTypeInternal();
    }

    private Piece? this[byte position] => _board[position].ToPiece();

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
                var piece = _board[IndexFromRankAndFile(rank, file)].ToPiece();
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

        var activeColour = WhiteTurn ? Colour.White : Colour.Black;
        var colourChar = activeColour.ToString().ToLower().First();
        stringBuilder.Append($" {colourChar} KQkq - {HalfMoveClock} {FullMoveNumber}");
        return stringBuilder.ToString();
    }

    private void FromForsythEdwardsNotation(string fen)
    {
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

            var index = IndexFromRankAndFile(rank, file);
            _board[index] = Piece.FromForsythEdwardsNotation(positionCharacter).ByteValue;
            ++file;
        }

        var colour = parts[1];
        WhiteTurn = colour switch
        {
            "w" => true,
            "b" => false,
            _ => throw new ArgumentException($"Invalid colour {colour}")
        };

        var halfMoveClock = parts[4];
        HalfMoveClock = ushort.Parse(halfMoveClock);

        var fullMoveNumber = parts[5];
        FullMoveNumber = ushort.Parse(fullMoveNumber);
    }

    public override string ToString() => ToForsythEdwardsNotation();

    public List<Move> GetValidMovesForColour(Colour colour)
    {
        var moves = new List<Move>();

        for (byte index = 0; index < _board.Length; ++index)
        {
            var piece = _board[index].ToPiece();
            if (piece == null || piece.Colour != colour)
                continue;

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
        var (rank, file) = RankAndFileFromIndex(position);

        // Advance and capture both move forward
        var rankAdvance = (byte) (colour == Colour.White ? rank + 1 : rank - 1);
        if (IsRankOrFileInBounds(rankAdvance) is false)
            return Enumerable.Empty<Move>();

        var moves = new List<Move>();

        // Advance
        var indexAdvance = IndexFromRankAndFile(rankAdvance, file);
        if (this[indexAdvance] is null)
        {
            moves.Add(new Move((Position) position, (Position) indexAdvance));

            // Double advance from starting position
            if ((colour == Colour.White && rank == 2) || (colour == Colour.Black && rank == 7))
            {
                var rankDoubleAdvance = (byte) (colour == Colour.White ? rank + 2 : rank - 2);
                var indexDoubleAdvance = IndexFromRankAndFile(rankDoubleAdvance, file);
                if (this[indexDoubleAdvance] is null)
                    moves.Add(new Move((Position) position, (Position) indexDoubleAdvance));
            }
        }

        // Capture left
        var fileCaptureLeft = (byte) (file - 1);
        if (IsRankOrFileInBounds(fileCaptureLeft))
        {
            var indexCaptureLeft = IndexFromRankAndFile(rankAdvance, fileCaptureLeft);
            var pieceCaptureLeft = this[indexCaptureLeft];
            if (pieceCaptureLeft is not null && pieceCaptureLeft.Colour != colour)
                moves.Add(new Move((Position) position, (Position) indexCaptureLeft));
        }

        // Capture right
        var fileCaptureRight = (byte) (file + 1);
        if (IsRankOrFileInBounds(fileCaptureRight))
        {
            var indexCaptureRight = IndexFromRankAndFile(rankAdvance, fileCaptureRight);
            var pieceCaptureRight = this[indexCaptureRight];
            if (pieceCaptureRight is not null && pieceCaptureRight.Colour != colour)
                moves.Add(new Move((Position) position, (Position) indexCaptureRight));
        }

        return moves;
    }

    private static bool IsRankOrFileInBounds(byte rankOrFile) => rankOrFile is >= 1 and <= Size;

    private static byte IndexFromRankAndFile(byte rank, byte file)
    {
        if (IsRankOrFileInBounds(rank) == false)
            throw new ArgumentOutOfRangeException(nameof(rank));

        if (IsRankOrFileInBounds(file) == false)
            throw new ArgumentOutOfRangeException(nameof(file));

        return (byte) ((rank - 1) * Size + (file - 1));
    }

    public static Position PositionFromRankAndFile(byte rank, byte file) => (Position) IndexFromRankAndFile(rank, file);

    private static (byte rank, byte file) RankAndFileFromIndex(byte index)
    {
        if (index >= IndexSize)
            throw new ArgumentOutOfRangeException(nameof(index));

        return ((byte) (index / Size + 1), (byte) (index % Size + 1));
    }

    public static (byte rank, byte file) RankAndFileFromPosition(Position position) => RankAndFileFromIndex((byte) position);
}

public static class BoardExtensions
{
    public static Piece? ToPiece(this PieceTypeInternal piece) => piece switch
    {
        PieceTypeInternal.None => null,
        PieceTypeInternal.BlackPawn => Piece.BlackPawn,
        PieceTypeInternal.BlackBishop => Piece.BlackBishop,
        PieceTypeInternal.BlackKnight => Piece.BlackKnight,
        PieceTypeInternal.BlackRook => Piece.BlackRook,
        PieceTypeInternal.BlackQueen => Piece.BlackQueen,
        PieceTypeInternal.BlackKing => Piece.BlackKing,
        PieceTypeInternal.WhitePawn => Piece.WhitePawn,
        PieceTypeInternal.WhiteBishop => Piece.WhiteBishop,
        PieceTypeInternal.WhiteKnight => Piece.WhiteKnight,
        PieceTypeInternal.WhiteRook => Piece.WhiteRook,
        PieceTypeInternal.WhiteQueen => Piece.WhiteQueen,
        PieceTypeInternal.WhiteKing => Piece.WhiteKing,
        _ => throw new ArgumentException($"Invalid piece byte {piece}")
    };

    public static PieceTypeInternal ToPieceTypeInternal(this Piece? piece) => piece?.ByteValue ?? PieceTypeInternal.None;
}