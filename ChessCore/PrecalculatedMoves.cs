using static ChessCore.Position;

namespace ChessCore;

public static class PrecalculatedMoves
{
    public static readonly Position[][,] QueenMoves;

    static PrecalculatedMoves()
    {
        QueenMoves = new Position[64][,];

        QueenMoves[(byte) d4] = new[,]
        {
            {d5, d6, d7, d8},
            {e5, f6, g7, h8},
            {e4, f4, g4, h4},
            {e3, f2, g1, None},
            {d3, d2, d1, None},
            {c3, b2, a1, None},
            {c4, b4, a4, None},
            {c5, b6, a7, None}
        };
    }
}