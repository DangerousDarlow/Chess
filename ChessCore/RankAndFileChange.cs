namespace ChessCore;

public record RankAndFileChange(sbyte RankChange, sbyte FileChange);

public static class RankAndFileChanges
{
    public static readonly RankAndFileChange North = new(1, 0);
    public static readonly RankAndFileChange NorthEast = new(1, 1);
    public static readonly RankAndFileChange East = new(0, 1);
    public static readonly RankAndFileChange SouthEast = new(-1, 1);
    public static readonly RankAndFileChange South = new(-1, 0);
    public static readonly RankAndFileChange SouthWest = new(-1, -1);
    public static readonly RankAndFileChange West = new(0, -1);
    public static readonly RankAndFileChange NorthWest = new(1, -1);
}

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