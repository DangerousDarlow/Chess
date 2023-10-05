using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class PawnMovesTests
{
    [Test]
    public void Pawn_can_advance_one_rank()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/P7/8/8 w - - 0 1");
        var state = new State(board);
        var moves = state.GetMovesForColour(Colour.White);
        Assert.That(moves, Has.Count.EqualTo(1));
        Assert.That(moves[0].From, Is.EqualTo(a3));
        Assert.That(moves[0].To, Is.EqualTo(a4));
    }

    [Test]
    public void Pawn_can_advance_two_ranks_from_starting_rank()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/P7/8 w - - 0 1");
        var state = new State(board);
        var moves = state.GetMovesForColour(Colour.White);
        Assert.That(moves, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(moves[0].From, Is.EqualTo(a2));
            Assert.That(moves[0].To, Is.EqualTo(a3));
        });

        Assert.Multiple(() =>
        {
            Assert.That(moves[1].From, Is.EqualTo(a2));
            Assert.That(moves[1].To, Is.EqualTo(a4));
        });
    }

    [Test]
    public void Pawn_blocked_by_own_piece_cannot_advance()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/P7/P7/8 w - - 0 1");
        var state = new State(board);
        var moves = state.GetMovesForColour(Colour.White);
        Assert.That(moves, Has.Count.EqualTo(1));
        Assert.That(moves[0].From, Is.EqualTo(a3));
        Assert.That(moves[0].To, Is.EqualTo(a4));
    }

    [Test]
    public void Pawn_blocked_by_opponent_piece_cannot_advance()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/p7/P7/8 w - - 0 1");
        var state = new State(board);
        var moves = state.GetMovesForColour(Colour.White);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void White_pawn_cannot_advance_off_board()
    {
        var board = Board.CreateFromForsythEdwardsNotation("P7/8/8/8/8/8/8/8 w - - 0 1");
        var state = new State(board);
        var moves = state.GetMovesForColour(Colour.White);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void Black_pawn_cannot_advance_off_board()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/8/p7 w - - 0 1");
        var state = new State(board);
        var moves = state.GetMovesForColour(Colour.Black);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void Pawn_can_capture_diagonally_left_or_right()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/ppp5/1P6 w - - 0 1");
        var state = new State(board);
        var moves = state.GetMovesForColour(Colour.White);

        Assert.That(moves, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(moves[0].From, Is.EqualTo(b1));
            Assert.That(moves[0].To, Is.EqualTo(a2));
        });

        Assert.Multiple(() =>
        {
            Assert.That(moves[1].From, Is.EqualTo(b1));
            Assert.That(moves[1].To, Is.EqualTo(c2));
        });
    }
}