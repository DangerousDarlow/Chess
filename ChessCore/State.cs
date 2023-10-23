namespace ChessCore;

public class State
{
    public State(Board board)
    {
        Board = board;
    }

    private Board Board { get; }

    public List<Move> GetMovesForColour(Colour colour)
    {
        var moves = new List<Move>();

        foreach (var (position, piece) in Board.PiecesOfColour(colour))
            moves.AddRange(
                piece.Type switch
                {
                    PieceType.Pawn => GetPawnMoves(position, colour),
                    PieceType.Bishop => GetBishopMoves(position, colour),
                    PieceType.Queen => GetQueenMoves(position, colour),
                    PieceType.King => GetKingMoves(position, colour)
                });

        return moves;
    }

    private IEnumerable<Move> GetPawnMoves(Position position, Colour colour)
    {
        var (rank, file) = Board.RankAndFileFromPosition(position);

        var rankAdvance = (byte) (colour == Colour.White ? rank + 1 : rank - 1);
        if (Board.IsRankOrFileInBounds(rankAdvance) is false)
            return Enumerable.Empty<Move>();

        var moves = new List<Move>();

        var positionAdvance = Board.PositionFromRankAndFile(rankAdvance, file);
        if (Board[positionAdvance] is null)
        {
            moves.Add(new Move(position, positionAdvance));

            // Double advance from starting position
            if ((colour == Colour.White && rank == 2) || (colour == Colour.Black && rank == 7))
            {
                var rankDoubleAdvance = (byte) (colour == Colour.White ? rank + 2 : rank - 2);
                var positionDoubleAdvance = Board.PositionFromRankAndFile(rankDoubleAdvance, file);
                if (Board[positionDoubleAdvance] is null)
                    moves.Add(new Move(position, positionDoubleAdvance));
            }
        }

        // Capture left
        AddPawnCaptureMove(position, colour, (byte) (file - 1), rankAdvance, moves);

        // Capture right
        AddPawnCaptureMove(position, colour, (byte) (file + 1), rankAdvance, moves);

        return moves;
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

    private void AddPawnCaptureMove(Position position, Colour colour, byte file, byte rank, ICollection<Move> moves)
    {
        if (!Board.IsRankOrFileInBounds(file)) return;

        var positionCapture = Board.PositionFromRankAndFile(rank, file);
        var pieceCapture = Board[positionCapture];
        if (pieceCapture is not null && pieceCapture.Colour != colour)
            moves.Add(new Move(position, positionCapture, MoveType.Capture));
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
        AddMoveAfterPositionChange(position, new RankAndFileChange(1, 0), colour, iterate);

    private IEnumerable<Move> AddMovesNorthEast(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, new RankAndFileChange(1, 1), colour, iterate);

    private IEnumerable<Move> AddMovesEast(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, new RankAndFileChange(0, 1), colour, iterate);

    private IEnumerable<Move> AddMovesSouthEast(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, new RankAndFileChange(-1, 1), colour, iterate);

    private IEnumerable<Move> AddMovesSouth(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, new RankAndFileChange(-1, 0), colour, iterate);

    private IEnumerable<Move> AddMovesSouthWest(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, new RankAndFileChange(-1, -1), colour, iterate);

    private IEnumerable<Move> AddMovesWest(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, new RankAndFileChange(0, -1), colour, iterate);

    private IEnumerable<Move> AddMovesNorthWest(Position position, Colour colour, bool iterate) =>
        AddMoveAfterPositionChange(position, new RankAndFileChange(1, -1), colour, iterate);

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