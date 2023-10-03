using ChessCore;
using static ChessCore.Position;

namespace ChessCoreTest;

public class BoardTests
{
    private Board _board = null!;

    [SetUp]
    public void Setup() => _board = Board.CreateWithNewGameSetup();

    [Test]
    public void Board_can_be_created_and_setup_for_new_game() => AssertSetupForNewGame(_board);

    private static void AssertSetupForNewGame(Board board)
    {
        Assert.That(board[a1], Is.EqualTo(Piece.WhiteRook));
        Assert.That(board[b1], Is.EqualTo(Piece.WhiteKnight));
        Assert.That(board[c1], Is.EqualTo(Piece.WhiteBishop));
        Assert.That(board[d1], Is.EqualTo(Piece.WhiteQueen));
        Assert.That(board[e1], Is.EqualTo(Piece.WhiteKing));
        Assert.That(board[f1], Is.EqualTo(Piece.WhiteBishop));
        Assert.That(board[g1], Is.EqualTo(Piece.WhiteKnight));
        Assert.That(board[h1], Is.EqualTo(Piece.WhiteRook));
        Assert.That(board[a2], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board[b2], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board[c2], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board[d2], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board[e2], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board[f2], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board[g2], Is.EqualTo(Piece.WhitePawn));
        Assert.That(board[h2], Is.EqualTo(Piece.WhitePawn));

        Assert.That(board[a7], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board[b7], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board[c7], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board[d7], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board[e7], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board[f7], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board[g7], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board[h7], Is.EqualTo(Piece.BlackPawn));
        Assert.That(board[a8], Is.EqualTo(Piece.BlackRook));
        Assert.That(board[b8], Is.EqualTo(Piece.BlackKnight));
        Assert.That(board[c8], Is.EqualTo(Piece.BlackBishop));
        Assert.That(board[d8], Is.EqualTo(Piece.BlackQueen));
        Assert.That(board[e8], Is.EqualTo(Piece.BlackKing));
        Assert.That(board[f8], Is.EqualTo(Piece.BlackBishop));
        Assert.That(board[g8], Is.EqualTo(Piece.BlackKnight));
        Assert.That(board[h8], Is.EqualTo(Piece.BlackRook));

        Assert.That(board.WhiteTurn, Is.True);
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
    public void Piece_can_be_moved_to_any_position_on_the_board_ignoring_piece_move_rules()
    {
        var board = Board.CreateFromForsythEdwardsNotation("8/8/8/8/8/8/8/K7 w - - 0 1");

        Assert.That(board[a1], Is.EqualTo(Piece.WhiteKing));
        Assert.That(board[h8], Is.Null);

        // Validation that the move is legal according to chess rules is not the responsibility of this function
        board.ApplyMove(new Move(a1, h8));

        Assert.That(board[a1], Is.Null);
        Assert.That(board[h8], Is.EqualTo(Piece.WhiteKing));
    }

    [Test]
    public void Pieces_of_colour_can_be_retrieved()
    {
        var whitePieces = _board.PiecesOfColour(Colour.White).ToList();
        Assert.That(whitePieces, Has.Count.EqualTo(16));
        Assert.That(whitePieces, Has.Exactly(1).Matches<(Position position, Piece piece)>(x => x.position == a1 && x.piece == Piece.WhiteRook));

        var blackPieces = _board.PiecesOfColour(Colour.Black).ToList();
        Assert.That(blackPieces, Has.Count.EqualTo(16));
        Assert.That(blackPieces, Has.Exactly(1).Matches<(Position position, Piece piece)>(x => x.position == h8 && x.piece == Piece.BlackRook));
    }

    [Test]
    [TestCase(1, 1, a1)]
    [TestCase(1, 8, h1)]
    [TestCase(8, 1, a8)]
    [TestCase(8, 8, h8)]
    public void Rank_and_file_maps_to_board_position(byte rank, byte file, Position position) =>
        // rank = horizontal row
        // file = vertical column
        Assert.Multiple(() =>
        {
            Assert.That(Board.PositionFromRankAndFile(rank, file), Is.EqualTo(position));
            var (rankFromIndex, fileFromIndex) = Board.RankAndFileFromPosition(position);
            Assert.That(rankFromIndex, Is.EqualTo(rank));
            Assert.That(fileFromIndex, Is.EqualTo(file));
        });

    [Test]
    [TestCase(0, 0)]
    [TestCase(9, 9)]
    public void IndexFromRankAndFile_throws_if_out_of_range(byte rank, byte file) =>
        Assert.That(() => Board.PositionFromRankAndFile(rank, file), Throws.TypeOf(typeof(ArgumentOutOfRangeException)));

    [Test]
    public void RankAndFileFromIndex_throws_if_out_of_range() =>
        Assert.That(() => Board.RankAndFileFromPosition((Position) 64), Throws.TypeOf(typeof(ArgumentOutOfRangeException)));
}