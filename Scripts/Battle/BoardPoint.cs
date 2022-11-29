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
            
        }
        public BoardPoint DebugAssert()
        {
            Debug.Assert(new IntRange(0, BoardManager.BoardSize.W-1).IsInRange(X), $"out of map: ({(X, Z)}");
            Debug.Assert(new IntRange(0, BoardManager.BoardSize.H-1).IsInRange(X), $"out of map: ({(X, Z)}");
            return this;
        }

        public BoardPoint Move(BoardPoint point)
        {
            return Move(point.X, point.Z);
        }
        public BoardPoint Move(int x, int z)
        {
            return new BoardPoint(X + x, Z + z);
        }

        public BoardPoint ToFlipped()
        {
            return new BoardPoint(BoardManager.BoardSize.W - 1 - X, BoardManager.BoardSize.H - 1 - Z);
        }
    }

}