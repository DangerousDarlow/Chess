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
        AddPawnCaptureMove(position, colour, (byte) (file - 1), rankAdvance, moves);

        // Capture right
        AddPawnCaptureMove(position, colour, (byte) (file + 1), rankAdvance, moves);

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
}