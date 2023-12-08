using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class PawnMovesTests
{
    [Test]
    public void Pawn_can_advance_one_rank()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/P7/8/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a3, a4)
        }, moves);
    }

    [Test]
    public void Pawn_can_advance_two_ranks_from_starting_rank()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/P7/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a2, a3),
            new(a2, a4, MoveType.DoublePawnAdvance)
        }, moves);
    }

    [Test]
    public void Pawn_blocked_by_own_piece_cannot_advance()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/P7/P7/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a3, a4)
        }, moves);
    }

    [Test]
    public void Pawn_blocked_by_opponent_piece_cannot_advance()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/p7/P7/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void White_pawn_cannot_advance_off_board()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("P7/8/8/8/8/8/8/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void Black_pawn_cannot_advance_off_board()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/8/p7 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.Black);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void Pawn_can_capture_diagonally()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/ppp5/1P6 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(b1, a2, MoveType.Capture),
            new(b1, c2, MoveType.Capture)
        }, moves);
    }

    [Test]
    public void Pawn_cannot_capture_own_colour()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/PPP5/1P6/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a3, a4),
            new(b3, b4),
            new(c3, c4)
        }, moves);
    }

    [Test]
    public void White_pawn_can_capture_en_passant()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/3p4/5p2/4P3/8/8/8/8 w - - 0 1");
        board.ApplyMove(new Move(d7, d5, MoveType.DoublePawnAdvance));
        var moveCalculator = new MoveCalculator(board);

        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(e5, e6),
            new(e5, f6, MoveType.Capture),
            new(e5, d6, MoveType.EnPassant)
        }, moves);
    }

    [Test]
    public void Pawn_cannot_capture_en_passant_if_previous_move_was_not_double_advance()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/3p1p2/4P3/8/8/8/8 w - - 0 1");
        board.ApplyMove(new Move(d6, d5));
        var moveCalculator = new MoveCalculator(board);

        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(e5, e6),
            new(e5, f6, MoveType.Capture)
        }, moves);
    }

    [Test]
    public void Black_pawn_can_capture_en_passant()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/p1p5/8/1P6/8 b - - 0 1");
        board.ApplyMove(new Move(b2, b4, MoveType.DoublePawnAdvance));
        var moveCalculator = new MoveCalculator(board);

        var moves = moveCalculator.GetMovesForColour(Colour.Black);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a4, a3),
            new(c4, c3),
            new(a4, b3, MoveType.EnPassant),
            new(c4, b3, MoveType.EnPassant)
        }, moves);
    }
}