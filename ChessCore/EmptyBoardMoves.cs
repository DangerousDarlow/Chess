using System.Runtime.Caching;
using static ChessCore.PieceType;
using static ChessCore.Position;

namespace ChessCore;

public class EmptyBoardMoves
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
            Knight => GetKnightMoves(position),
            Bishop => GetBishopMoves(position),
            Rook => GetRookMoves(position),
            Queen => GetQueenMoves(position),
            King => GetKingMoves(position),
            _ => throw new NotSupportedException($"Unsupported piece type {pieceType}")
        };

        _cache.Add(key, moves, _cacheItemPolicy);
        return moves;
    }

    private static List<Position> GetKnightMoves(Position position)
    {
        var moves = new List<Position>();
        moves.AddPositions(position, RankAndFileChanges.KnightNorthEast);
        moves.AddPositions(position, RankAndFileChanges.KnightNorthWest);
        moves.AddPositions(position, RankAndFileChanges.KnightEastNorth);
        moves.AddPositions(position, RankAndFileChanges.KnightEastSouth);
        moves.AddPositions(position, RankAndFileChanges.KnightSouthEast);
        moves.AddPositions(position, RankAndFileChanges.KnightSouthWest);
        moves.AddPositions(position, RankAndFileChanges.KnightWestNorth);
        moves.AddPositions(position, RankAndFileChanges.KnightWestSouth);
        return moves;
    }

    private static List<Position> GetBishopMoves(Position position)
    {
        var moves = new List<Position>();
        moves.AddPositions(position, RankAndFileChanges.NorthEast, true);
        moves.AddPositions(position, RankAndFileChanges.SouthEast, true);
        moves.AddPositions(position, RankAndFileChanges.SouthWest, true);
        moves.AddPositions(position, RankAndFileChanges.NorthWest, true);
        return moves;
    }

    private static List<Position> GetRookMoves(Position position)
    {
        var moves = new List<Position>();
        moves.AddPositions(position, RankAndFileChanges.North, true);
        moves.AddPositions(position, RankAndFileChanges.East, true);
        moves.AddPositions(position, RankAndFileChanges.South, true);
        moves.AddPositions(position, RankAndFileChanges.West, true);
        return moves;
    }

    private static List<Position> GetQueenMoves(Position position)
    {
        var moves = new List<Position>();
        moves.AddPositions(position, RankAndFileChanges.North, true);
        moves.AddPositions(position, RankAndFileChanges.NorthEast, true);
        moves.AddPositions(position, RankAndFileChanges.East, true);
        moves.AddPositions(position, RankAndFileChanges.SouthEast, true);
        moves.AddPositions(position, RankAndFileChanges.South, true);
        moves.AddPositions(position, RankAndFileChanges.SouthWest, true);
        moves.AddPositions(position, RankAndFileChanges.West, true);
        moves.AddPositions(position, RankAndFileChanges.NorthWest, true);
        return moves;
    }

    private static List<Position> GetKingMoves(Position position)
    {
        var moves = new List<Position>();
        moves.AddPositions(position, RankAndFileChanges.North);
        moves.AddPositions(position, RankAndFileChanges.NorthEast);
        moves.AddPositions(position, RankAndFileChanges.East);
        moves.AddPositions(position, RankAndFileChanges.SouthEast);
        moves.AddPositions(position, RankAndFileChanges.South);
        moves.AddPositions(position, RankAndFileChanges.SouthWest);
        moves.AddPositions(position, RankAndFileChanges.West);
        moves.AddPositions(position, RankAndFileChanges.NorthWest);
        return moves;
    }
}

internal static class MovesListExtensions
{
    public static void AddPositions(this List<Position> moves, Position position, RankAndFileChange change, bool repeat = false)
    {
        position = position.Change(change);
        if (position == None)
            return;

        moves.Add(position);
        if (!repeat)
            return;

        while (position != None)
        {
            position = position.Change(change);
            moves.Add(position);
        }
    }
}