using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public struct BoardPoint
    {
        public readonly int X;
        public readonly int Z;

        public BoardPoint(int x, int z)
        {
            X = x;
            Z = z;
            
            Debug.Assert(new IntRange(0, BoardManager.BoardSize.W-1).IsInRange(x), $"({(x, z)}");
            Debug.Assert(new IntRange(0, BoardManager.BoardSize.H-1).IsInRange(z), $"({(x, z)}");
        }
    }

}