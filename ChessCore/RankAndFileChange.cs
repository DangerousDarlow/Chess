namespace ChessCore;

public record RankAndFileChange(sbyte RankChange, sbyte FileChange);

public static class RankAndFileChangeExtensions
{
    public static Position Transform(this Position position, RankAndFileChange change)
    {
        var (rank, file) = Board.RankAndFileFromPosition(position);
        var newRank = (byte) (rank + change.RankChange);
        var newFile = (byte) (file + change.FileChange);
        if (Board.IsRankOrFileInBounds(newRank) && Board.IsRankOrFileInBounds(newFile))
            return Board.PositionFromRankAndFile(newRank, newFile);

        return Position.None;
    }
}