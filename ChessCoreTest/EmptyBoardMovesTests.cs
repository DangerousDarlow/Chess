using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class EmptyBoardMovesTests
{
    private EmptyBoardMoves EmptyBoardMoves { get; } = new();

    [Test]
    public void Knight_can_move_to_eight_positions() => CollectionAssert.AreEquivalent(
        new[] {c6, e6, f5, f3, e2, c2, b3, b5},
        EmptyBoardMoves.GetMoves(PieceType.Knight, d4));

    [Test]
    public void Knight_cannot_move_off_the_board_South_or_West() => CollectionAssert.AreEquivalent(
        new[] {b3, c2},
        EmptyBoardMoves.GetMoves(PieceType.Knight, a1));

    [Test]
    public void Knight_cannot_move_off_the_board_North_or_East() => CollectionAssert.AreEquivalent(
        new[] {f7, g6},
        EmptyBoardMoves.GetMoves(PieceType.Knight, h8));

    [Test]
    public void Bishop_can_move_diagonally() => CollectionAssert.AreEquivalent(
        new[]
        {
            e5, f6, g7, h8, None,
            e3, f2, g1, None,
            c3, b2, a1, None,
            c5, b6, a7, None
        },
        EmptyBoardMoves.GetMoves(PieceType.Bishop, d4));

    [Test]
    public void Rook_can_move_horizontally_and_vertically() => CollectionAssert.AreEquivalent(
        new[]
        {
            d5, d6, d7, d8, None,
            e4, f4, g4, h4, None,
            d3, d2, d1, None,
            c4, b4, a4, None,
        },
        EmptyBoardMoves.GetMoves(PieceType.Rook, d4));

    [Test]
    public void Queen_can_move_in_eight_directions() => CollectionAssert.AreEquivalent(
        new[]
        {
            d5, d6, d7, d8, None,
            e5, f6, g7, h8, None,
            e4, f4, g4, h4, None,
            e3, f2, g1, None,
            d3, d2, d1, None,
            c3, b2, a1, None,
            c4, b4, a4, None,
            c5, b6, a7, None
        },
        EmptyBoardMoves.GetMoves(PieceType.Queen, d4));

    [Test]
    public void Directions_with_no_moves_are_not_included() => CollectionAssert.AreEquivalent(
        new[]
        {
            a2, a3, a4, a5, a6, a7, a8, None,
            b2, c3, d4, e5, f6, g7, h8, None,
            b1, c1, d1, e1, f1, g1, h1, None,
        },
        EmptyBoardMoves.GetMoves(PieceType.Queen, a1));

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