namespace ChessCore;

public class State
{
    public State(Board board)
    {
        Board = board;
    }

    public Board Board { get; }

    public List<Move> GetMovesForColour(Colour colour)
    {
        var moves = new List<Move>();

        foreach (var (position, piece) in Board.PiecesOfColour(colour))
            moves.AddRange(
                piece.Type switch
                {
                    PieceType.Pawn => GetPawnMoves(position, colour)
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
        var fileCaptureLeft = (byte) (file - 1);
        if (Board.IsRankOrFileInBounds(fileCaptureLeft))
        {
            var positionCaptureLeft = Board.PositionFromRankAndFile(rankAdvance, fileCaptureLeft);
            var pieceCaptureLeft = Board[positionCaptureLeft];
            if (pieceCaptureLeft is not null && pieceCaptureLeft.Colour != colour)
                moves.Add(new Move(position, positionCaptureLeft));
        }

        // Capture right
        var fileCaptureRight = (byte) (file + 1);
        if (Board.IsRankOrFileInBounds(fileCaptureRight))
        {
            var positionCaptureRight = Board.PositionFromRankAndFile(rankAdvance, fileCaptureRight);
            var pieceCaptureRight = Board[positionCaptureRight];
            if (pieceCaptureRight is not null && pieceCaptureRight.Colour != colour)
                moves.Add(new Move(position, positionCaptureRight));
        }

        return moves;
    }
}