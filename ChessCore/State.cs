namespace ChessCore;

public class State
{
    private static readonly RankAndFileChange North = new(1, 0);
    private static readonly RankAndFileChange NorthEast = new(1, 1);
    private static readonly RankAndFileChange East = new(0, 1);
    private static readonly RankAndFileChange SouthEast = new(-1, 1);
    private static readonly RankAndFileChange South = new(-1, 0);
    private static readonly RankAndFileChange SouthWest = new(-1, -1);
    private static readonly RankAndFileChange West = new(0, -1);
    private static readonly RankAndFileChange NorthWest = new(1, -1);

    private static readonly RankAndFileChange KnightNorthEast = new(2, -1);
    private static readonly RankAndFileChange KnightNorthWest = new(2, 1);
    private static readonly RankAndFileChange KnightEastNorth = new(1, 2);
    private static readonly RankAndFileChange KnightEastSouth = new(-1, 2);
    private static readonly RankAndFileChange KnightSouthEast = new(-2, 1);
    private static readonly RankAndFileChange KnightSouthWest = new(-2, -1);
    private static readonly RankAndFileChange KnightWestSouth = new(-1, -2);
    private static readonly RankAndFileChange KnightWestNorth = new(1, -2);

    public State(Board board, Move? previousMove = null)
    {
        Board = board;
        PreviousMove = previousMove;
    }

    private Board Board { get; }

    private Move? PreviousMove { get; }

    public List<Move> GetMovesForColour(Colour colour)
    {
        var moves = new List<Move>();

        foreach (var (position, piece) in Board.PiecesOfColour(colour))
            moves.AddRange(
                piece.Type switch
                {
                    PieceType.Pawn => GetPawnMoves(position, colour),
                    PieceType.Bishop => GetBishopMoves(position, colour),
                    PieceType.Knight => GetKnightMoves(position, colour),
                    PieceType.Rook => GetRookMoves(position, colour),
                    PieceType.Queen => GetQueenMoves(position, colour),
                    PieceType.King => GetKingMoves(position, colour),
                    _ => throw new Exception($"Unknown piece type {piece.Type}")
                });

        return moves;
    }

    private IEnumerable<Move> GetPawnMoves(Position positionFrom, Colour colour)
    {
        var (rankFrom, fileFrom) = Board.RankAndFileFromPosition(positionFrom);

        var rankTo = (byte) (colour == Colour.White ? rankFrom + 1 : rankFrom - 1);
        if (Board.IsRankOrFileInBounds(rankTo) is false)
            return Enumerable.Empty<Move>();

        var moves = new List<Move>();

        var positionTo = Board.PositionFromRankAndFile(rankTo, fileFrom);
        if (Board[positionTo] is null)
        {
            moves.Add(new Move(positionFrom, positionTo));

            // Double advance from starting position
            if ((colour == Colour.White && rankFrom == 2) || (colour == Colour.Black && rankFrom == 7))
            {
                var rankToDoubleAdvance = (byte) (colour == Colour.White ? rankFrom + 2 : rankFrom - 2);
                var positionToDoubleAdvance = Board.PositionFromRankAndFile(rankToDoubleAdvance, fileFrom);
                if (Board[positionToDoubleAdvance] is null)
                    moves.Add(new Move(positionFrom, positionToDoubleAdvance));
            }
        }

        // Capture left
        AddPawnCaptureMove(positionFrom, colour, (byte) (fileFrom - 1), rankTo, rankFrom, moves);

        // Capture right
        AddPawnCaptureMove(positionFrom, colour, (byte) (fileFrom + 1), rankTo, rankFrom, moves);

        return moves;
    }

    private void AddPawnCaptureMove(Position positionFrom, Colour colour, byte fileTo, byte rankTo, byte rankFrom, ICollection<Move> moves)
    {
        if (!Board.IsRankOrFileInBounds(fileTo)) return;

        var endPositionOfCapturingPiece = Board.PositionFromRankAndFile(rankTo, fileTo);
        var pieceCaptured = Board[endPositionOfCapturingPiece];
        if (pieceCaptured is not null && pieceCaptured.Colour != colour)
        {
            moves.Add(new Move(positionFrom, endPositionOfCapturingPiece, MoveType.Capture));

            // If a capture can be performed then en passant can't be performed
            return;
        }

        // En passant from here on

        // En passant can only be to rank 6 for white and rank 3 for black
        var endRankOfPositionCapturingEnPassant = (byte) (colour == Colour.White ? 6 : 3);
        if (rankTo != endRankOfPositionCapturingEnPassant) return;

        // Position of opponent's pawn for en passant capture. This isn't the end position of the en passant move.
        var positionOfEnPassantTarget = Board.PositionFromRankAndFile(rankFrom, fileTo);

        var pieceEnPassantCaptured = Board[positionOfEnPassantTarget];

        // En passant can only capture an opponent's pawn
        if (pieceEnPassantCaptured is null || pieceEnPassantCaptured.Colour == colour || pieceEnPassantCaptured.Type != PieceType.Pawn) return;

        // Rank the en passant target pawn made a double advance from
        var startRankOfEnPassantTarget = (byte) (colour == Colour.White ? 7 : 2);

        var opponentDoubleAdvance = new Move(Board.PositionFromRankAndFile(startRankOfEnPassantTarget, fileTo), positionOfEnPassantTarget);
        if (PreviousMove != opponentDoubleAdvance) return;

        moves.Add(new Move(positionFrom, endPositionOfCapturingPiece, MoveType.EnPassant));
    }

    private IEnumerable<Move> GetBishopMoves(Position position, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorthEast(position, colour, true));
        moves.AddRange(AddMovesSouthEast(position, colour, true));
        moves.AddRange(AddMovesSouthWest(position, colour, true));
        moves.AddRange(AddMovesNorthWest(position, colour, true));
        return moves;
    }

    private IEnumerable<Move> GetKnightMoves(Position position, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMoveAfterPositionChange(position, KnightNorthEast, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(position, KnightNorthWest, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(position, KnightEastNorth, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(position, KnightEastSouth, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(position, KnightSouthEast, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(position, KnightSouthWest, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(position, KnightWestSouth, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(position, KnightWestNorth, colour, false));
        return moves;
    }

    private IEnumerable<Move> GetRookMoves(Position position, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorth(position, colour, true));
        moves.AddRange(AddMovesEast(position, colour, true));
        moves.AddRange(AddMovesSouth(position, colour, true));
        moves.AddRange(AddMovesWest(position, colour, true));
        return moves;
    }

    private IEnumerable<Move> GetQueenMoves(Position position, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorth(position, colour, true));
        moves.AddRange(AddMovesNorthEast(position, colour, true));
        moves.AddRange(AddMovesEast(position, colour, true));
        moves.AddRange(AddMovesSouthEast(position, colour, true));
        moves.AddRange(AddMovesSouth(position, colour, true));
        moves.AddRange(AddMovesSouthWest(position, colour, true));
        moves.AddRange(AddMovesWest(position, colour, true));
        moves.AddRange(AddMovesNorthWest(position, colour, true));
        return moves;
    }

    private IEnumerable<Move> GetKingMoves(Position position, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorth(position, colour, false));
        moves.AddRange(AddMovesNorthEast(position, colour, false));
        moves.AddRange(AddMovesEast(position, colour, false));
        moves.AddRange(AddMovesSouthEast(position, colour, false));
        moves.AddRange(AddMovesSouth(position, colour, false));
        moves.AddRange(AddMovesSouthWest(position, colour, false));
        moves.AddRange(AddMovesWest(position, colour, false));
        moves.AddRange(AddMovesNorthWest(position, colour, false));
        return moves;
    }

    private IEnumerable<Move> AddMovesNorth(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, North, colour, iterate);

    private IEnumerable<Move> AddMovesNorthEast(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, NorthEast, colour, iterate);

    private IEnumerable<Move> AddMovesEast(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, East, colour, iterate);

    private IEnumerable<Move> AddMovesSouthEast(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, SouthEast, colour, iterate);

    private IEnumerable<Move> AddMovesSouth(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, South, colour, iterate);

    private IEnumerable<Move> AddMovesSouthWest(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, SouthWest, colour, iterate);

    private IEnumerable<Move> AddMovesWest(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, West, colour, iterate);

    private IEnumerable<Move> AddMovesNorthWest(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, NorthWest, colour, iterate);

    private IEnumerable<Move> AddMoveAfterPositionChange(Position position, RankAndFileChange change, Colour colour, bool iterate)
    {
        var moves = new List<Move>();
        var (rank, file) = Board.RankAndFileFromPosition(position);

        do
        {
            rank = (byte) (rank + change.RankChange);
            file = (byte) (file + change.FileChange);

            if (Board.IsRankOrFileInBounds(rank) is false || Board.IsRankOrFileInBounds(file) is false)
                return moves;

            var toPosition = Board.PositionFromRankAndFile(rank, file);
            var piece = Board[toPosition];

            if (piece is null)
            {
                moves.Add(new Move(position, toPosition));
                continue;
            }

            if (piece.Colour != colour)
                moves.Add(new Move(position, toPosition, MoveType.Capture));

            return moves;

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            // Loop exits when position is out of bounds or occupied by another piece
        } while (iterate);

        return moves;
    }
}

public record RankAndFileChange(sbyte RankChange, sbyte FileChange);