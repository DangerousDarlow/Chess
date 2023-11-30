using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class PawnMovesTests
{
    [Test]
    public void Pawn_can_advance_one_rank()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/P7/8/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a3, a4)
        }, moves);
    }

    [Test]
    public void Pawn_can_advance_two_ranks_from_starting_rank()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/P7/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a2, a3),
            new(a2, a4)
        }, moves);
    }

    [Test]
    public void Pawn_blocked_by_own_piece_cannot_advance()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/P7/P7/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a3, a4)
        }, moves);
    }

    [Test]
    public void Pawn_blocked_by_opponent_piece_cannot_advance()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/p7/P7/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void White_pawn_cannot_advance_off_board()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("P7/8/8/8/8/8/8/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void Black_pawn_cannot_advance_off_board()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/8/p7 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.Black);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void Pawn_can_capture_diagonally()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/ppp5/1P6 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(b1, a2, MoveType.Capture),
            new(b1, c2, MoveType.Capture)
        }, moves);
    }

    [Test]
    public void Pawn_cannot_capture_own_colour()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/PPP5/1P6/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

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
        var state = new State(
            Board.CreateFromForsythEdwardsNotation("8/8/5p2/3pP3/8/8/8/8 w - - 0 1"),
            new Move(d7, d5));

        var moves = state.GetMovesForColour(Colour.White);

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
        var state = new State(
            Board.CreateFromForsythEdwardsNotation("8/8/5p2/3pP3/8/8/8/8 w - - 0 1"),
            new Move(f7, f6));

        var moves = state.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(e5, e6),
            new(e5, f6, MoveType.Capture),
        }, moves);
    }

    [Test]
    public void Black_pawn_can_capture_en_passant()
    {
        var state = new State(
            Board.CreateFromForsythEdwardsNotation("8/8/8/8/pPp5/8/8/8 w - - 0 1"),
            new Move(b2, b4));

        var moves = state.GetMovesForColour(Colour.Black);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a4, a3),
            new(c4, c3),
            new(a4, b3, MoveType.EnPassant),
            new(c4, b3, MoveType.EnPassant)
        }, moves);
    }
}