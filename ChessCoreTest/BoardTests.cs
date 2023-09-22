using ChessCore;

namespace ChessCoreTest;

public class Tests
{
    private Board _board = null!;

    [SetUp]
    public void Setup() => _board = Board.CreateWithNewGameSetup();

    [Test]
    public void Board_can_be_created_and_setup_for_new_game() => AssertSetupForNewGame(_board);

    private static void AssertSetupForNewGame(Board board)
    {
        Assert.That(board["a1"], Is.EqualTo(Piece.WhiteRook));
        Assert.That(board["b1"], Is.EqualTo(Piece.WhiteKnight));
        Assert.That(board["c1"], Is.EqualTo(Piece.WhiteBishop));
        Assert.That(board["d1"], Is.EqualTo(Piece.WhiteQueen));
        Assert.That(board["e1"], Is.EqualTo(Piece.WhiteKing));
        Assert.That(board["f1"], Is.EqualTo(Piece.WhiteBishop));
        Assert.That(board["g1"], Is.EqualTo(Piece.WhiteKnight));
        Assert.That(board["h1"], Is.EqualTo(Piece.WhiteRook));
        Assert.That(board["a2"], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board["b2"], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board["c2"], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board["d2"], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board["e2"], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board["f2"], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board["g2"], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board["h2"], Is.EqualTo(Piece.WhitePawn));

        Assert.That(board["a7"], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board["b7"], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board["c7"], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board["d7"], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board["e7"], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board["f7"], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board["g7"], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board["h7"], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board["a8"], Is.EqualTo(Piece.BlackRook));
        Assert.That(board["b8"], Is.EqualTo(Piece.BlackKnight));
        Assert.That(board["c8"], Is.EqualTo(Piece.BlackBishop));
        Assert.That(board["d8"], Is.EqualTo(Piece.BlackQueen));
        Assert.That(board["e8"], Is.EqualTo(Piece.BlackKing));
        Assert.That(board["f8"], Is.EqualTo(Piece.BlackBishop));
        Assert.That(board["g8"], Is.EqualTo(Piece.BlackKnight));
        Assert.That(board["h8"], Is.EqualTo(Piece.BlackRook));

        Assert.That(board.ActiveColour, Is.EqualTo(Colour.White));
        Assert.That(board.FullMoveNumber, Is.EqualTo(1));
        Assert.That(board.HalfMoveClock, Is.EqualTo(0));
    }

    [Test]
    public void Board_can_be_created_and_setup_from_new_game_forsyth_edwards_notation() =>
        AssertSetupForNewGame(Board.CreateFromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));

    [Test]
    public void Forsyth_Edwards_Notation_is_correct_for_new_game_state() =>
        Assert.That(_board.ToForsythEdwardsNotation(), Is.EqualTo("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));

    [Test]
    [TestCase(1, 1, 0, "a1")]
    [TestCase(1, 8, 7, "h1")]
    [TestCase(8, 1, 56, "a8")]
    [TestCase(8, 8, 63, "h8")]
    public void Rank_and_file_maps_to_board_position(byte rank, byte file, byte index, string position) =>
        // rank = horizontal row
        // file = vertical column
        Assert.Multiple(() =>
        {
            Assert.That(BoardExtensions.IndexFromRankAndFile(rank, file), Is.EqualTo(index));
            Assert.That(BoardExtensions.IndexFromRankAndFile(rank, file).PositionAsAlgebraicNotation(), Is.EqualTo(position));
        });
}