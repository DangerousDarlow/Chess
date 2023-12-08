using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class BishopMovesTests
{
    [Test]
    public void Bishop_can_move_diagonally()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/3B4/8/8/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(d4, e5),
            new(d4, f6),
            new(d4, g7),
            new(d4, h8),
            new(d4, e3),
            new(d4, f2),
            new(d4, g1),
            new(d4, c3),
            new(d4, b2),
            new(d4, a1),
            new(d4, c5),
            new(d4, b6),
            new(d4, a7)
        }, moves);
    }

    [Test]
    public void Bishop_can_capture_diagonally()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/2ppp3/2pBp3/2ppp3/8/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(d4, e5, MoveType.Capture),
            new(d4, e3, MoveType.Capture),
            new(d4, c3, MoveType.Capture),
            new(d4, c5, MoveType.Capture)
        }, moves);
    }

    [Test]
    public void Bishop_cannot_capture_own_colour()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/2PPP3/2PBP3/2PPP3/8/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(c5, c6),
            new(d5, d6),
            new(e5, e6)
        }, moves);
    }
}