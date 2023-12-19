using System.Text;
using static ChessCore.Position;

namespace ChessCore;

public interface IBoard
{
    bool IsWhiteTurn { get; }
    bool IsWhiteKingsideCastleAvailable { get; }
    bool IsWhiteQueensideCastleAvailable { get; }
    bool IsBlackKingsideCastleAvailable { get; }
    bool IsBlackQueensideCastleAvailable { get; }
    Position? EnPassantTarget { get; }
    ushort FullMoveNumber { get; }
    ushort HalfMoveClock { get; }
    Piece? this[Position position] { get; }
    void ApplyMove(Move move);
    IEnumerable<(Position position, Piece piece)> PiecesOfColour(Colour colour);
    string ToForsythEdwardsNotation();
    string ToString();
}

public class Board : IBoard
{
    private const int Size = 8;

    private const int IndexSize = Size * Size;

    /// <summary>
    ///     There are 32 chess pieces so half this array will be empty. An array of pieces was considered as an
    ///     alternative however position and piece bytes would need to be stored therefore the array would be
    ///     the same size. In addition querying the piece at a location would require iteration.
    /// </summary>
    private readonly PieceTypeInternal[] _board = new PieceTypeInternal[IndexSize];

    private readonly BitVector8 _flags = new();

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

        IsWhiteTurn = true;
        IsWhiteKingsideCastleAvailable = true;
        IsWhiteQueensideCastleAvailable = true;
        IsBlackKingsideCastleAvailable = true;
        IsBlackQueensideCastleAvailable = true;
    }

    public bool IsWhiteTurn
    {
        get => _flags[0];
        private set => _flags[0] = value;
    }

    public bool IsWhiteKingsideCastleAvailable
    {
        get => _flags[1];
        private set => _flags[1] = value;
    }

    public bool IsWhiteQueensideCastleAvailable
    {
        get => _flags[2];
        private set => _flags[2] = value;
    }

    public bool IsBlackKingsideCastleAvailable
    {
        get => _flags[3];
        private set => _flags[3] = value;
    }

    public bool IsBlackQueensideCastleAvailable
    {
        get => _flags[4];
        private set => _flags[4] = value;
    }

    public Position? EnPassantTarget { get; private set; }

    public ushort FullMoveNumber { get; private set; } = 1;

    public ushort HalfMoveClock { get; private set; }

    public Piece? this[Position position]
    {
        get => position == None ? null : _board[(byte) position].ToPiece();
        private set
        {
            if (position != None)
                _board[(byte) position] = value.ToPieceTypeInternal();
        }
    }

    public void ApplyMove(Move move)
    {
        var piece = this[move.From];
        if (piece == null)
            throw new Exception($"Invalid {move}; No piece");

        this[move.To] = piece;
        this[move.From] = null;

        IsWhiteTurn = !IsWhiteTurn;

        if (move.Type == MoveType.DoublePawnAdvance)
        {
            var (rankFrom, fileFrom) = RankAndFileFromPosition(move.From);
            var (rankTo, _) = RankAndFileFromPosition(move.To);
            var rank = rankTo > rankFrom ? (byte) (rankFrom + 1) : (byte) (rankFrom - 1);
            EnPassantTarget = PositionFromRankAndFile(rank, fileFrom);
        }
        else
        {
            EnPassantTarget = null;
        }

        if (move.Type == MoveType.Capture || piece.Type == PieceType.Pawn)
            HalfMoveClock = 0;
        else
            ++HalfMoveClock;

        if (IsWhiteTurn)
            ++FullMoveNumber;
    }

    public IEnumerable<(Position position, Piece piece)> PiecesOfColour(Colour colour)
    {
        for (byte index = 0; index < _board.Length; ++index)
        {
            var piece = _board[index].ToPiece();
            if (piece?.Colour == colour)
                yield return ((Position) index, piece);
        }
    }

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

        var activeColour = IsWhiteTurn ? Colour.White : Colour.Black;
        var colourChar = activeColour.ToString().ToLower().First();
        stringBuilder.Append($" {colourChar} ");

        if (IsWhiteKingsideCastleAvailable)
            stringBuilder.Append('K');

        if (IsWhiteQueensideCastleAvailable)
            stringBuilder.Append('Q');

        if (IsBlackKingsideCastleAvailable)
            stringBuilder.Append('k');

        if (IsBlackQueensideCastleAvailable)
            stringBuilder.Append('q');

        var enPassantTarget = EnPassantTarget == null ? "-" : EnPassantTarget.ToString();
        stringBuilder.Append($" {enPassantTarget} {HalfMoveClock} {FullMoveNumber}");
        return stringBuilder.ToString();
    }

    public override string ToString() => ToForsythEdwardsNotation();

    public static Board CreateWithNewGameSetup() => new();

    public static Board CreateFromForsythEdwardsNotation(string fen) => new(fen);

    private void FromForsythEdwardsNotation(string fen)
    {
        Array.Fill(_board, PieceTypeInternal.None);

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
        IsWhiteTurn = colour switch
        {
            "w" => true,
            "b" => false,
            _ => throw new ArgumentException($"Invalid colour {colour}")
        };

        var castlingAvailability = parts[2];
        IsWhiteKingsideCastleAvailable = castlingAvailability.Contains('K');
        IsWhiteQueensideCastleAvailable = castlingAvailability.Contains('Q');
        IsBlackKingsideCastleAvailable = castlingAvailability.Contains('k');
        IsBlackQueensideCastleAvailable = castlingAvailability.Contains('q');

        var enPassantTarget = parts[3];
        if (enPassantTarget == "-")
            EnPassantTarget = null;
        else
            EnPassantTarget = Enum.Parse<Position>(enPassantTarget);

        var halfMoveClock = parts[4];
        HalfMoveClock = ushort.Parse(halfMoveClock);

        var fullMoveNumber = parts[5];
        FullMoveNumber = ushort.Parse(fullMoveNumber);
    }

    public static bool IsRankOrFileInBounds(byte rankOrFile) => rankOrFile is >= 1 and <= Size;

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