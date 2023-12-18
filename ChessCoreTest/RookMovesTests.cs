using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class RookMovesTests : MovesTests
{
    [Test]
    public void Rook_can_move_horizontally_and_vertically()
    {
        var moves = MoveCalculator.GetMovesForColour(Board.CreateFromForsythEdwardsNotation("8/8/8/8/3R4/8/8/8 w - - 0 1"), Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(d4, d5),
            new(d4, d6),
            new(d4, d7),
            new(d4, d8),
            new(d4, e4),
            new(d4, f4),
            new(d4, g4),
            new(d4, h4),
            new(d4, d3),
            new(d4, d2),
            new(d4, d1),
            new(d4, c4),
            new(d4, b4),
            new(d4, a4)
        }, moves);
    }

    [Test]
    public void Rook_can_capture_horizontally_and_vertically()
    {
        var moves = MoveCalculator.GetMovesForColour(Board.CreateFromForsythEdwardsNotation("8/8/8/3p4/2pRp3/3p4/8/8 w - - 0 1"), Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(d4, d5, MoveType.Capture),
            new(d4, e4, MoveType.Capture),
            new(d4, d3, MoveType.Capture),
            new(d4, c4, MoveType.Capture)
        }, moves);
    }

    [Test]
    public void Rook_cannot_capture_own_colour()
    {
        var moves = MoveCalculator.GetMovesForColour(Board.CreateFromForsythEdwardsNotation("8/8/8/3P4/2PRP3/3P4/8/8 w - - 0 1"), Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(c4, c5),
            new(d5, d6),
            new(e4, e5)
        }, moves);
    }
}