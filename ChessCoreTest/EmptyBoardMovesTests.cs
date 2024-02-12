using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class EmptyBoardMovesTests
{
    private EmptyBoardMoves EmptyBoardMoves { get; } = new();

    [Test]
    public void King_can_move_one_space_in_eight_directions() => CollectionAssert.AreEquivalent(
        new[] {d5, e5, e4, e3, d3, c3, c4, c5},
        EmptyBoardMoves.GetMoves(PieceType.King, d4));

    [Test]
    public void King_cannot_move_off_the_board_South_or_West() => CollectionAssert.AreEquivalent(
        new[] {a2, b2, b1},
        EmptyBoardMoves.GetMoves(PieceType.King, a1));

    [Test]
    public void King_cannot_move_off_the_board_North_or_East() => CollectionAssert.AreEquivalent(
        new[] {h7, g7, g8},
        EmptyBoardMoves.GetMoves(PieceType.King, h8));
}