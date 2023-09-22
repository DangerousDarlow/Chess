using ChessCore;

namespace ChessCoreTest;

public class Tests
{
    private Board _board = null!;

    [SetUp]
    public void Setup()
    {
        _board = new Board();
    }

    [Test]
    public void Board_is_setup_correctly_on_construction()
    {
        Assert.That(_board["a1"], Is.EqualTo(Pieces.WhiteRook));
        Assert.That(_board["b1"], Is.EqualTo(Pieces.WhiteKnight));
        Assert.That(_board["c1"], Is.EqualTo(Pieces.WhiteBishop));
        Assert.That(_board["d1"], Is.EqualTo(Pieces.WhiteQueen));
        Assert.That(_board["e1"], Is.EqualTo(Pieces.WhiteKing));
        Assert.That(_board["f1"], Is.EqualTo(Pieces.WhiteBishop));
        Assert.That(_board["g1"], Is.EqualTo(Pieces.WhiteKnight));
        Assert.That(_board["h1"], Is.EqualTo(Pieces.WhiteRook));
        Assert.That(_board["a2"], Is.EqualTo(Pieces.WhitePawn));
        Assert.That(_board["b2"], Is.EqualTo(Pieces.WhitePawn));
        Assert.That(_board["c2"], Is.EqualTo(Pieces.WhitePawn));
        Assert.That(_board["d2"], Is.EqualTo(Pieces.WhitePawn));
        Assert.That(_board["e2"], Is.EqualTo(Pieces.WhitePawn));
        Assert.That(_board["f2"], Is.EqualTo(Pieces.WhitePawn));
        Assert.That(_board["g2"], Is.EqualTo(Pieces.WhitePawn));
        Assert.That(_board["h2"], Is.EqualTo(Pieces.WhitePawn));

        Assert.That(_board["a7"], Is.EqualTo(Pieces.BlackPawn));
        Assert.That(_board["b7"], Is.EqualTo(Pieces.BlackPawn));
        Assert.That(_board["c7"], Is.EqualTo(Pieces.BlackPawn));
        Assert.That(_board["d7"], Is.EqualTo(Pieces.BlackPawn));
        Assert.That(_board["e7"], Is.EqualTo(Pieces.BlackPawn));
        Assert.That(_board["f7"], Is.EqualTo(Pieces.BlackPawn));
        Assert.That(_board["g7"], Is.EqualTo(Pieces.BlackPawn));
        Assert.That(_board["h7"], Is.EqualTo(Pieces.BlackPawn));
        Assert.That(_board["a8"], Is.EqualTo(Pieces.BlackRook));
        Assert.That(_board["b8"], Is.EqualTo(Pieces.BlackKnight));
        Assert.That(_board["c8"], Is.EqualTo(Pieces.BlackBishop));
        Assert.That(_board["d8"], Is.EqualTo(Pieces.BlackQueen));
        Assert.That(_board["e8"], Is.EqualTo(Pieces.BlackKing));
        Assert.That(_board["f8"], Is.EqualTo(Pieces.BlackBishop));
        Assert.That(_board["g8"], Is.EqualTo(Pieces.BlackKnight));
        Assert.That(_board["h8"], Is.EqualTo(Pieces.BlackRook));

        Assert.That(_board.ActiveColour, Is.EqualTo(Colour.White));
        Assert.That(_board.FullMoveNumber, Is.EqualTo(1));
        Assert.That(_board.HalfMoveClock, Is.EqualTo(0));
    }

    [Test]
    public void Forsyth_Edwards_Notation_is_correct_for_initial_state() =>
        Assert.That(_board.ToForsythEdwardsNotation(), Is.EqualTo("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));

    [Test]
    [TestCase(1, 1, 0, "a1")]
    [TestCase(1, 8, 7, "h1")]
    [TestCase(8, 1, 56, "a8")]
    [TestCase(8, 8, 63, "h8")]
    public void Rank_and_file_maps_to_board_position(byte rank, byte file, byte index, string position)
    {
        // rank = horizontal row
        // file = vertical column

        Assert.Multiple(() =>
        {
            Assert.That(BoardExtensions.IndexFromRankAndFile(rank, file), Is.EqualTo(index));
            Assert.That(BoardExtensions.IndexFromRankAndFile(rank, file).PositionAsAlgebraicNotation(), Is.EqualTo(position));
        });
    }
}