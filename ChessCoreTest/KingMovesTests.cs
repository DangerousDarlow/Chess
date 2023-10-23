using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class KingMovesTests
{
    [Test]
    public void King_can_move_in_eight_directions()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/8/3K4/8/8/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

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
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/2ppp3/2pKp3/2ppp3/8/8 w - - 0 1"));
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
    public void King_cannot_capture_own_colour()
    {
        var state = new State(Board.CreateFromForsythEdwardsNotation("8/8/8/2PPP3/2PKP3/2PPP3/8/8 w - - 0 1"));
        var moves = state.GetMovesForColour(Colour.White);

        CollectionAssert.AreEquivalent(new List<Move>
        {
            new(c5, c6),
            new(d5, d6),
            new(e5, e6)
        }, moves);
    }
}