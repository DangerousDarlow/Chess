namespace ChessCore;

public interface IMoveCalculator
{
    List<Move> GetMovesForColour(IBoard board, Colour colour);
}

public class MoveCalculator : IMoveCalculator
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

    public List<Move> GetMovesForColour(IBoard board, Colour colour)
    {
        var moves = new List<Move>();

        foreach (var (positionFrom, piece) in board.PiecesOfColour(colour))
            moves.AddRange(
                piece.Type switch
                {
                    PieceType.Pawn => GetPawnMoves(positionFrom, board, colour),
                    PieceType.Bishop => GetBishopMoves(positionFrom, board, colour),
                    PieceType.Knight => GetKnightMoves(positionFrom, board, colour),
                    PieceType.Rook => GetRookMoves(positionFrom, board, colour),
                    PieceType.Queen => GetQueenMoves(positionFrom, board, colour),
                    PieceType.King => GetKingMoves(positionFrom, board, colour),
                    _ => throw new Exception($"Unknown piece type {piece.Type}")
                });

        return moves;
    }

    private IEnumerable<Move> GetPawnMoves(Position positionFrom, IBoard board, Colour colour)
    {
        var (rankFrom, fileFrom) = Board.RankAndFileFromPosition(positionFrom);

        var rankTo = (byte) (colour == Colour.White ? rankFrom + 1 : rankFrom - 1);
        if (Board.IsRankOrFileInBounds(rankTo) is false)
            return Enumerable.Empty<Move>();

        var moves = new List<Move>();

        var positionTo = Board.PositionFromRankAndFile(rankTo, fileFrom);
        if (board[positionTo] is null)
        {
            moves.Add(new Move(positionFrom, positionTo));

            // Double advance from starting position
            if ((colour == Colour.White && rankFrom == 2) || (colour == Colour.Black && rankFrom == 7))
            {
                var rankToDoubleAdvance = (byte) (colour == Colour.White ? rankFrom + 2 : rankFrom - 2);
                var positionToDoubleAdvance = Board.PositionFromRankAndFile(rankToDoubleAdvance, fileFrom);
                if (board[positionToDoubleAdvance] is null)
                    moves.Add(new Move(positionFrom, positionToDoubleAdvance, MoveType.DoublePawnAdvance));
            }
        }

        // Capture left
        AddPawnCaptureMove(positionFrom, board, colour, (byte) (fileFrom - 1), rankTo, moves);

        // Capture right
        AddPawnCaptureMove(positionFrom, board, colour, (byte) (fileFrom + 1), rankTo, moves);

        return moves;
    }

    private void AddPawnCaptureMove(Position positionFrom, IBoard board, Colour colour, byte fileTo, byte rankTo, List<Move> moves)
    {
        if (!Board.IsRankOrFileInBounds(fileTo)) return;

        var positionTo = Board.PositionFromRankAndFile(rankTo, fileTo);
        var pieceCaptured = board[positionTo];
        if (pieceCaptured is not null && pieceCaptured.Colour != colour)
            moves.Add(new Move(positionFrom, positionTo, MoveType.Capture));
        else if (positionTo == board.EnPassantTarget)
            moves.Add(new Move(positionFrom, positionTo, MoveType.EnPassant));
    }

    private IEnumerable<Move> GetBishopMoves(Position positionFrom, IBoard board, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorthEast(positionFrom, board, colour, true));
        moves.AddRange(AddMovesSouthEast(positionFrom, board, colour, true));
        moves.AddRange(AddMovesSouthWest(positionFrom, board, colour, true));
        moves.AddRange(AddMovesNorthWest(positionFrom, board, colour, true));
        return moves;
    }

    private IEnumerable<Move> GetKnightMoves(Position positionFrom, IBoard board, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightNorthEast, board, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightNorthWest, board, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightEastNorth, board, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightEastSouth, board, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightSouthEast, board, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightSouthWest, board, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightWestSouth, board, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightWestNorth, board, colour, false));
        return moves;
    }

    private IEnumerable<Move> GetRookMoves(Position positionFrom, IBoard board, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorth(positionFrom, board, colour, true));
        moves.AddRange(AddMovesEast(positionFrom, board, colour, true));
        moves.AddRange(AddMovesSouth(positionFrom, board, colour, true));
        moves.AddRange(AddMovesWest(positionFrom, board, colour, true));
        return moves;
    }

    private IEnumerable<Move> GetQueenMoves(Position positionFrom, IBoard board, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorth(positionFrom, board, colour, true));
        moves.AddRange(AddMovesNorthEast(positionFrom, board, colour, true));
        moves.AddRange(AddMovesEast(positionFrom, board, colour, true));
        moves.AddRange(AddMovesSouthEast(positionFrom, board, colour, true));
        moves.AddRange(AddMovesSouth(positionFrom, board, colour, true));
        moves.AddRange(AddMovesSouthWest(positionFrom, board, colour, true));
        moves.AddRange(AddMovesWest(positionFrom, board, colour, true));
        moves.AddRange(AddMovesNorthWest(positionFrom, board, colour, true));
        return moves;
    }

    private IEnumerable<Move> GetKingMoves(Position positionFrom, IBoard board, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorth(positionFrom, board, colour, false));
        moves.AddRange(AddMovesNorthEast(positionFrom, board, colour, false));
        moves.AddRange(AddMovesEast(positionFrom, board, colour, false));
        moves.AddRange(AddMovesSouthEast(positionFrom, board, colour, false));
        moves.AddRange(AddMovesSouth(positionFrom, board, colour, false));
        moves.AddRange(AddMovesSouthWest(positionFrom, board, colour, false));
        moves.AddRange(AddMovesWest(positionFrom, board, colour, false));
        moves.AddRange(AddMovesNorthWest(positionFrom, board, colour, false));
        return moves;
    }

    private IEnumerable<Move> AddMovesNorth(Position positionFrom, IBoard board, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, North, board, colour, iterate);

    private IEnumerable<Move> AddMovesNorthEast(Position positionFrom, IBoard board, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, NorthEast, board, colour, iterate);

    private IEnumerable<Move> AddMovesEast(Position positionFrom, IBoard board, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, East, board, colour, iterate);

    private IEnumerable<Move> AddMovesSouthEast(Position positionFrom, IBoard board, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, SouthEast, board, colour, iterate);

    private IEnumerable<Move> AddMovesSouth(Position positionFrom, IBoard board, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, South, board, colour, iterate);

    private IEnumerable<Move> AddMovesSouthWest(Position positionFrom, IBoard board, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, SouthWest, board, colour, iterate);

    private IEnumerable<Move> AddMovesWest(Position positionFrom, IBoard board, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, West, board, colour, iterate);

    private IEnumerable<Move> AddMovesNorthWest(Position positionFrom, IBoard board, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, NorthWest, board, colour, iterate);

    private IEnumerable<Move> AddMoveAfterPositionChange(Position positionFrom, RankAndFileChange change, IBoard board, Colour colour, bool iterate)
    {
        var moves = new List<Move>();
        var (rank, file) = Board.RankAndFileFromPosition(positionFrom);

        do
        {
            rank = (byte) (rank + change.RankChange);
            file = (byte) (file + change.FileChange);

            if (Board.IsRankOrFileInBounds(rank) is false || Board.IsRankOrFileInBounds(file) is false)
                return moves;

            var positionTo = Board.PositionFromRankAndFile(rank, file);
            var piece = board[positionTo];

            if (piece is null)
            {
                moves.Add(new Move(positionFrom, positionTo));
                continue;
            }

            if (piece.Colour != colour)
                moves.Add(new Move(positionFrom, positionTo, MoveType.Capture));

            return moves;

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            // Loop exits when position is out of bounds or occupied by another piece
        } while (iterate);

        return moves;
    }
}

public record RankAndFileChange(sbyte RankChange, sbyte FileChange);