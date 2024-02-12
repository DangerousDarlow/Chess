using System.Runtime.Caching;
using static ChessCore.Position;

namespace ChessCore;

public interface IEmptyBoardMoves
{
    IEnumerable<Position> GetMoves(PieceType pieceType, Position position);
}

public class EmptyBoardMoves : IEmptyBoardMoves
{
    private readonly MemoryCache _cache = new("EmptyBoardMoves");
    private readonly CacheItemPolicy _cacheItemPolicy = new();

    public IEnumerable<Position> GetMoves(PieceType pieceType, Position position)
    {
        var key = $"{pieceType}:{position}";
        if (_cache.Contains(key))
            if (_cache.Get(key) is IEnumerable<Position> cachedMoves)
                return cachedMoves;

        var moves = pieceType switch
        {
            PieceType.King => GetKingMoves(position),
            _ => throw new Exception($"Unknown piece type {pieceType}")
        };

        _cache.Add(key, moves, _cacheItemPolicy);
        return moves;
    }

    private static List<Position> GetKingMoves(Position position)
    {
        var moves = new List<Position>();
        moves.AddIfNotNone(position.Transform(RankAndFileChanges.North));
        moves.AddIfNotNone(position.Transform(RankAndFileChanges.NorthEast));
        moves.AddIfNotNone(position.Transform(RankAndFileChanges.East));
        moves.AddIfNotNone(position.Transform(RankAndFileChanges.SouthEast));
        moves.AddIfNotNone(position.Transform(RankAndFileChanges.South));
        moves.AddIfNotNone(position.Transform(RankAndFileChanges.SouthWest));
        moves.AddIfNotNone(position.Transform(RankAndFileChanges.West));
        moves.AddIfNotNone(position.Transform(RankAndFileChanges.NorthWest));
        return moves;
    }
}

public static class MovesListExtensions
{
    public static void AddIfNotNone(this List<Position> moves, Position position)
    {
        if (position == None)
            return;

        moves.Add(position);
    }
}