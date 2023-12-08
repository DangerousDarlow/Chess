using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class KnightMovesTests
{
    [Test]
    public void Knight_can_move_to_eight_positions()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/3N4/8/8/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(d4, c6),
            new(d4, e6),
            new(d4, f5),
            new(d4, f3),
            new(d4, e2),
            new(d4, c2),
            new(d4, b3),
            new(d4, b5)
        }, moves);
    }

    [Test]
    public void Knight_cannot_move_off_board()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/8/N7 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(a1, b3),
            new(a1, c2)
        }, moves);
    }

    [Test]
    public void Knight_can_capture_eight_positions()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/2p1p3/1p3p2/3N4/1p3p2/2p1p3/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(d4, c6, MoveType.Capture),
            new(d4, e6, MoveType.Capture),
            new(d4, f5, MoveType.Capture),
            new(d4, f3, MoveType.Capture),
            new(d4, e2, MoveType.Capture),
            new(d4, c2, MoveType.Capture),
            new(d4, b3, MoveType.Capture),
            new(d4, b5, MoveType.Capture)
        }, moves);
    }

    [Test]
    public void Knight_cannot_capture_own_colour()
    {
        var moveCalculator = new MoveCalculator(Board.CreateFromForsythEdwardsNotation("8/8/2P1P3/1P3P2/3N4/1P3P2/2P1P3/8 w - - 0 1"));
        var moves = moveCalculator.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(e6, e7),
            new(f5, f6),
            new(f3, f4),
            new(e2, e3),
            new(c2, c3),
            new(b3, b4),
            new(b5, b6),
            new(c6, c7),
            new(e2, e4, MoveType.DoublePawnAdvance),
            new(c2, c4, MoveType.DoublePawnAdvance)
        }, moves);
    }
}