using ChessCore;

namespace ChessCoreTest;

public abstract class MovesTests
{
    protected IMoveCalculator MoveCalculator { get; } = new MoveCalculator();
}