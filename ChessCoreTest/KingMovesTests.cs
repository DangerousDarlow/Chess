using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class KingMovesTests : MovesTests
{
    [Test]
    public void King_can_move_in_eight_directions()
    {
        var moves = MoveCalculator.GetMovesForColour(Board.CreateFromForsythEdwardsNotation("8/8/8/8/3K4/8/8/8 w - - 0 1"), Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(d4, d5),
            new(d4, e5),
            new(d4, e4),
            new(d4, e3),
            new(d4, d3),
            new(d4, c3),
            new(d4, c4),
            new(d4, c5)
        }, moves);
    }

    [Test]
    public void King_can_capture_in_eight_directions()
    {
        var moves = MoveCalculator.GetMovesForColour(Board.CreateFromForsythEdwardsNotation("8/8/8/2ppp3/2pKp3/2ppp3/8/8 w - - 0 1"), Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(d4, d5, MoveType.Capture),
            new(d4, e5, MoveType.Capture),
            new(d4, e4, MoveType.Capture),
            new(d4, e3, MoveType.Capture),
            new(d4, d3, MoveType.Capture),
            new(d4, c3, MoveType.Capture),
            new(d4, c4, MoveType.Capture),
            new(d4, c5, MoveType.Capture)
        }, moves);
    }

    [Test]
    public void King_cannot_capture_own_colour()
    {
        var moves = MoveCalculator.GetMovesForColour(Board.CreateFromForsythEdwardsNotation("8/8/8/2PPP3/2PKP3/2PPP3/8/8 w - - 0 1"), Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(c5, c6),
            new(d5, d6),
            new(e5, e6)
        }, moves);
    }

    [Test]
    public void King_can_castle_king_side()
    {
        var board = Board.CreateFromForsythEdwardsNotation("rnbqk2r/pppp1ppp/3b1n2/4p3/4P3/3B1N2/PPPP1PPP/RNBQK2R w KQkq - 4 4");
        var moves = MoveCalculator.GetMovesForColour(board, Colour.White);

        CollectionAssert.Contains(moves, new Move(e1, g1, MoveType.Castle),
            "King side castle is not present in move list but is possible");

        CollectionAssert.DoesNotContain(moves, new Move(e1, c1, MoveType.Castle),
            "Queen side castle is present in move list but is not possible");

        board.ApplyMove(new Move(e1, g1, MoveType.Castle));
        Assert.Multiple(() =>
        {
            Assert.That(board[g1], Is.EqualTo(Piece.WhiteKing), "White king is not in correct position");
            Assert.That(board[f1], Is.EqualTo(Piece.WhiteRook), "White rook is not in correct position");
            Assert.That(board.IsWhiteKingsideCastleAvailable, Is.False, "White king side castle is still available");
            Assert.That(board.IsWhiteQueensideCastleAvailable, Is.False, "White queen side castle is still available");
        });

        moves = MoveCalculator.GetMovesForColour(board, Colour.Black);

        CollectionAssert.Contains(moves, new Move(e8, g8, MoveType.Castle),
            "King side castle is not present in move list but is possible");

        CollectionAssert.DoesNotContain(moves, new Move(e8, c8, MoveType.Castle),
            "Queen side castle is present in move list but is not possible");

        board.ApplyMove(new Move(e8, g8, MoveType.Castle));
        Assert.Multiple(() =>
        {
            Assert.That(board[g8], Is.EqualTo(Piece.BlackKing), "Black king is not in correct position");
            Assert.That(board[f8], Is.EqualTo(Piece.BlackRook), "Black rook is not in correct position");
            Assert.That(board.IsBlackKingsideCastleAvailable, Is.False, "Black king side castle is still available");
            Assert.That(board.IsBlackQueensideCastleAvailable, Is.False, "Black queen side castle is still available");
        });
    }

    [Test]
    public void King_can_castle_queen_side()
    {
        var board = Board.CreateFromForsythEdwardsNotation("r3kbnr/p1ppqppp/bpn5/4p3/4P3/BPN5/P1PPQPPP/R3KBNR w KQkq - 2 6");
        var moves = MoveCalculator.GetMovesForColour(board, Colour.White);

        CollectionAssert.Contains(moves, new Move(e1, c1, MoveType.Castle),
            "Queen side castle is not present in move list but is possible");

        CollectionAssert.DoesNotContain(moves, new Move(e1, g1, MoveType.Castle),
            "King side castle is present in move list but is not possible");

        board.ApplyMove(new Move(e1, c1, MoveType.Castle));
        Assert.Multiple(() =>
        {
            Assert.That(board[c1], Is.EqualTo(Piece.WhiteKing), "White king is not in correct position");
            Assert.That(board[d1], Is.EqualTo(Piece.WhiteRook), "White rook is not in correct position");
            Assert.That(board.IsWhiteKingsideCastleAvailable, Is.False, "White king side castle is still available");
            Assert.That(board.IsWhiteQueensideCastleAvailable, Is.False, "White queen side castle is still available");
        });

        moves = MoveCalculator.GetMovesForColour(board, Colour.Black);

        CollectionAssert.Contains(moves, new Move(e8, c8, MoveType.Castle),
            "Queen side castle is not present in move list but is possible");

        CollectionAssert.DoesNotContain(moves, new Move(e8, g8, MoveType.Castle),
            "King side castle is present in move list but is not possible");

        board.ApplyMove(new Move(e8, c8, MoveType.Castle));
        Assert.Multiple(() =>
        {
            Assert.That(board[c8], Is.EqualTo(Piece.BlackKing), "Black king is not in correct position");
            Assert.That(board[d8], Is.EqualTo(Piece.BlackRook), "Black rook is not in correct position");
            Assert.That(board.IsBlackKingsideCastleAvailable, Is.False, "Black king side castle is still available");
            Assert.That(board.IsBlackQueensideCastleAvailable, Is.False, "Black queen side castle is still available");
        });
    }

    [Test]
    public void Castling_is_not_possible_if_blocked_by_pieces()
    {
        var board = Board.CreateFromForsythEdwardsNotation("rnbqk1nr/ppppbppp/8/4p3/4P3/8/PPPPBPPP/RNBQK1NR w KQkq - 2 3");
        var moves = MoveCalculator.GetMovesForColour(board, Colour.White);
        moves.AddRange(MoveCalculator.GetMovesForColour(board, Colour.Black));

        CollectionAssert.DoesNotContain(moves, new Move(e1, g1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e1, c1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, g8, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, c8, MoveType.Castle));

        board = Board.CreateFromForsythEdwardsNotation("rnbqkb1r/pppp1ppp/5n2/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3");
        moves = MoveCalculator.GetMovesForColour(board, Colour.White);
        moves.AddRange(MoveCalculator.GetMovesForColour(board, Colour.Black));

        CollectionAssert.DoesNotContain(moves, new Move(e1, g1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e1, c1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, g8, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, c8, MoveType.Castle));

        board = Board.CreateFromForsythEdwardsNotation("rn2kbnr/pppbpppp/3q4/3p4/3P4/3Q4/PPPBPPPP/RN2KBNR w KQkq - 0 1");
        moves = MoveCalculator.GetMovesForColour(board, Colour.White);
        moves.AddRange(MoveCalculator.GetMovesForColour(board, Colour.Black));

        CollectionAssert.DoesNotContain(moves, new Move(e1, g1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e1, c1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, g8, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, c8, MoveType.Castle));

        board = Board.CreateFromForsythEdwardsNotation("r1b1kbnr/ppp1pppp/2nq4/3p4/3P4/2NQ4/PPP1PPPP/R1B1KBNR w KQkq - 0 1");
        moves = MoveCalculator.GetMovesForColour(board, Colour.White);
        moves.AddRange(MoveCalculator.GetMovesForColour(board, Colour.Black));

        CollectionAssert.DoesNotContain(moves, new Move(e1, g1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e1, c1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, g8, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, c8, MoveType.Castle));

        board = Board.CreateFromForsythEdwardsNotation("r2qkbnr/pppbpppp/2n5/3p4/3P4/2N5/PPPBPPPP/R2QKBNR w KQkq - 0 1");
        moves = MoveCalculator.GetMovesForColour(board, Colour.White);
        moves.AddRange(MoveCalculator.GetMovesForColour(board, Colour.Black));

        CollectionAssert.DoesNotContain(moves, new Move(e1, g1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e1, c1, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, g8, MoveType.Castle));
        CollectionAssert.DoesNotContain(moves, new Move(e8, c8, MoveType.Castle));
    }
}