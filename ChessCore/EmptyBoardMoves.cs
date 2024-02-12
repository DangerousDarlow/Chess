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

    private static IEnumerable<Position> GetKingMoves(Position position)
    {
        var moves = new List<Position>();
        moves.AddIfNotNone(position.Transform(North));
        moves.AddIfNotNone(position.Transform(NorthEast));
        moves.AddIfNotNone(position.Transform(East));
        moves.AddIfNotNone(position.Transform(SouthEast));
        moves.AddIfNotNone(position.Transform(South));
        moves.AddIfNotNone(position.Transform(SouthWest));
        moves.AddIfNotNone(position.Transform(West));
        moves.AddIfNotNone(position.Transform(NorthWest));
        return moves;
    }

    private static readonly RankAndFileChange North = new(1, 0);
    private static readonly RankAndFileChange NorthEast = new(1, 1);
    private static readonly RankAndFileChange East = new(0, 1);
    private static readonly RankAndFileChange SouthEast = new(-1, 1);
    private static readonly RankAndFileChange South = new(-1, 0);
    private static readonly RankAndFileChange SouthWest = new(-1, -1);
    private static readonly RankAndFileChange West = new(0, -1);
    private static readonly RankAndFileChange NorthWest = new(1, -1);
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