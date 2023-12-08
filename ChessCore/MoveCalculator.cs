namespace ChessCore;

public class MoveCalculator(Board board)
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

    private Board Board { get; } = board;

    public List<Move> GetMovesForColour(Colour colour)
    {
        var moves = new List<Move>();

        foreach (var (positionFrom, piece) in Board.PiecesOfColour(colour))
            moves.AddRange(
                piece.Type switch
                {
                    PieceType.Pawn => GetPawnMoves(positionFrom, colour),
                    PieceType.Bishop => GetBishopMoves(positionFrom, colour),
                    PieceType.Knight => GetKnightMoves(positionFrom, colour),
                    PieceType.Rook => GetRookMoves(positionFrom, colour),
                    PieceType.Queen => GetQueenMoves(positionFrom, colour),
                    PieceType.King => GetKingMoves(positionFrom, colour),
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
                    moves.Add(new Move(positionFrom, positionToDoubleAdvance, MoveType.DoublePawnAdvance));
            }
        }

        // Capture left
        AddPawnCaptureMove(positionFrom, colour, (byte) (fileFrom - 1), rankTo, moves);

        // Capture right
        AddPawnCaptureMove(positionFrom, colour, (byte) (fileFrom + 1), rankTo, moves);

        return moves;
    }

    private void AddPawnCaptureMove(Position positionFrom, Colour colour, byte fileTo, byte rankTo, List<Move> moves)
    {
        if (!Board.IsRankOrFileInBounds(fileTo)) return;

        var positionTo = Board.PositionFromRankAndFile(rankTo, fileTo);
        var pieceCaptured = Board[positionTo];
        if (pieceCaptured is not null && pieceCaptured.Colour != colour)
            moves.Add(new Move(positionFrom, positionTo, MoveType.Capture));
        else if (positionTo == Board.EnPassantTarget)
            moves.Add(new Move(positionFrom, positionTo, MoveType.EnPassant));
    }

    private IEnumerable<Move> GetBishopMoves(Position positionFrom, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorthEast(positionFrom, colour, true));
        moves.AddRange(AddMovesSouthEast(positionFrom, colour, true));
        moves.AddRange(AddMovesSouthWest(positionFrom, colour, true));
        moves.AddRange(AddMovesNorthWest(positionFrom, colour, true));
        return moves;
    }

    private IEnumerable<Move> GetKnightMoves(Position positionFrom, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightNorthEast, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightNorthWest, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightEastNorth, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightEastSouth, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightSouthEast, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightSouthWest, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightWestSouth, colour, false));
        moves.AddRange(AddMoveAfterPositionChange(positionFrom, KnightWestNorth, colour, false));
        return moves;
    }

    private IEnumerable<Move> GetRookMoves(Position positionFrom, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorth(positionFrom, colour, true));
        moves.AddRange(AddMovesEast(positionFrom, colour, true));
        moves.AddRange(AddMovesSouth(positionFrom, colour, true));
        moves.AddRange(AddMovesWest(positionFrom, colour, true));
        return moves;
    }

    private IEnumerable<Move> GetQueenMoves(Position positionFrom, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorth(positionFrom, colour, true));
        moves.AddRange(AddMovesNorthEast(positionFrom, colour, true));
        moves.AddRange(AddMovesEast(positionFrom, colour, true));
        moves.AddRange(AddMovesSouthEast(positionFrom, colour, true));
        moves.AddRange(AddMovesSouth(positionFrom, colour, true));
        moves.AddRange(AddMovesSouthWest(positionFrom, colour, true));
        moves.AddRange(AddMovesWest(positionFrom, colour, true));
        moves.AddRange(AddMovesNorthWest(positionFrom, colour, true));
        return moves;
    }

    private IEnumerable<Move> GetKingMoves(Position positionFrom, Colour colour)
    {
        var moves = new List<Move>();
        moves.AddRange(AddMovesNorth(positionFrom, colour, false));
        moves.AddRange(AddMovesNorthEast(positionFrom, colour, false));
        moves.AddRange(AddMovesEast(positionFrom, colour, false));
        moves.AddRange(AddMovesSouthEast(positionFrom, colour, false));
        moves.AddRange(AddMovesSouth(positionFrom, colour, false));
        moves.AddRange(AddMovesSouthWest(positionFrom, colour, false));
        moves.AddRange(AddMovesWest(positionFrom, colour, false));
        moves.AddRange(AddMovesNorthWest(positionFrom, colour, false));
        return moves;
    }

    private IEnumerable<Move> AddMovesNorth(Position positionFrom, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, North, colour, iterate);

    private IEnumerable<Move> AddMovesNorthEast(Position positionFrom, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, NorthEast, colour, iterate);

    private IEnumerable<Move> AddMovesEast(Position positionFrom, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, East, colour, iterate);

    private IEnumerable<Move> AddMovesSouthEast(Position positionFrom, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, SouthEast, colour, iterate);

    private IEnumerable<Move> AddMovesSouth(Position positionFrom, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, South, colour, iterate);

    private IEnumerable<Move> AddMovesSouthWest(Position positionFrom, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, SouthWest, colour, iterate);

    private IEnumerable<Move> AddMovesWest(Position positionFrom, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, West, colour, iterate);

    private IEnumerable<Move> AddMovesNorthWest(Position positionFrom, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(positionFrom, NorthWest, colour, iterate);

    private IEnumerable<Move> AddMoveAfterPositionChange(Position positionFrom, RankAndFileChange change, Colour colour, bool iterate)
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
            var piece = Board[positionTo];

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