using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class QueenMovesTests
{
    [Test]
    public void Queen_can_move_in_eight_directions()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/8/3Q4/8/8/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(d4, d5),
            new(d4, d6),
            new(d4, d7),
            new(d4, d8),
            new(d4, e5),
            new(d4, f6),
            new(d4, g7),
            new(d4, h8),
            new(d4, e4),
            new(d4, f4),
            new(d4, g4),
            new(d4, h4),
            new(d4, e3),
            new(d4, f2),
            new(d4, g1),
            new(d4, d3),
            new(d4, d2),
            new(d4, d1),
            new(d4, c3),
            new(d4, b2),
            new(d4, a1),
            new(d4, c4),
            new(d4, b4),
            new(d4, a4),
            new(d4, c5),
            new(d4, b6),
            new(d4, a7)
        }, moves);
    }

    [Test]
    public void Queen_can_capture_in_eight_directions()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/2ppp3/2pQp3/2ppp3/8/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

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
    public void Queen_cannot_capture_own_colour()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/2PPP3/2PQP3/2PPP3/8/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(c5, c6),
            new(d5, d6),
            new(e5, e6)
        }, moves);
    }
}