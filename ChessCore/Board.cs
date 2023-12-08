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
        InitialiseFromForsythEdwardsNotation(fen);
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
        this[move.To] = this[move.From];
        this[move.From] = null;

        WhiteTurn = !WhiteTurn;

        if (move.Type == MoveType.DoublePawnAdvance)
        {
            var (rankFrom, fileFrom) = RankAndFileFromPosition(move.From);
            var (rankTo, _) = RankAndFileFromPosition(move.To);
            var rank = rankTo > rankFrom ? (byte) (rankFrom + 1) : (byte) (rankFrom - 1);
            EnPassantTarget = PositionFromRankAndFile(rank, fileFrom);
        }
        else
            EnPassantTarget = null;

        if (WhiteTurn)
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
        var enPassantTarget = EnPassantTarget == null ? "-" : EnPassantTarget.ToString();
        stringBuilder.Append($" {colourChar} KQkq {enPassantTarget} {HalfMoveClock} {FullMoveNumber}");
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Initialise the board from a FEN string
    ///
    /// This function must not be made public with it's current implementation.
    /// It assumes the board is uninitialized. It does not overwrite any existing state.
    /// </summary>
    private void InitialiseFromForsythEdwardsNotation(string fen)
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

        var enPassantTarget = parts[3];
        if (enPassantTarget != "-")
            EnPassantTarget = Enum.Parse<Position>(enPassantTarget);

        var halfMoveClock = parts[4];
        HalfMoveClock = ushort.Parse(halfMoveClock);

        var fullMoveNumber = parts[5];
        FullMoveNumber = ushort.Parse(fullMoveNumber);
    }

    public override string ToString() => ToForsythEdwardsNotation();

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