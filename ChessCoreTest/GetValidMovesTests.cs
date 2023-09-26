using ChessCore;

namespace ChessCoreTest;

public class GetValidMovesTests
{
    [Test]
    public void White_pawn_on_a2_can_move_to_a3_only()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/P7/8 w - - 0 1");
        var moves = board.GetValidMovesForColour(Colour.White);
        Assert.That(moves.Count, Is.EqualTo(1));
        Assert.That(moves[0].From, Is.EqualTo("a2"));
        Assert.That(moves[0].To, Is.EqualTo("a3"));
    }

    [Test]
    public void White_pawn_on_a2_with_white_pawn_on_a3_cannot_move()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/P7/P7/8 w - - 0 1");
        var moves = board.GetValidMovesForColour(Colour.White);
        Assert.That(moves.Count, Is.EqualTo(1));
        Assert.That(moves[0].From, Is.EqualTo("a3"));
        Assert.That(moves[0].To, Is.EqualTo("a4"));
    }

    [Test]
    public void White_pawn_on_a2_with_black_pawn_on_a3_cannot_move()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/p7/P7/8 w - - 0 1");
        var moves = board.GetValidMovesForColour(Colour.White);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void White_pawn_on_a8_cannot_move()
    {
        var board = Board.CreateFromForsythEdwardsNotation("P7/8/8/8/8/8/8/8 w - - 0 1");
        var moves = board.GetValidMovesForColour(Colour.White);
        Assert.That(moves, Is.Empty);
    }

    [Test]
    public void Black_pawn_on_a1_cannot_move()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/8/p7 w - - 0 1");
        var moves = board.GetValidMovesForColour(Colour.Black);
        Assert.That(moves, Is.Empty);
    }
}