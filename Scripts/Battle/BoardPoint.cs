using System;
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
        
        public static BoardPoint operator+ (double z, BoardPoint w)
        {
            return new BoardPoint();
        }
    }

    /// <summary>
    /// 仮想上にした点 (駒の動ける範囲などを求める際に仕様)
    /// </summary>
    public struct ImBoardPoint
    {
        public readonly BoardPoint Raw;

        public ImBoardPoint(int x, int z) : this(new BoardPoint(x, z))
        { }

        public ImBoardPoint(BoardPoint raw)
        {
            Raw = raw;
        }

        public BoardPoint ToReal(bool isLocal)
        {
            return isLocal
                ? Raw
                : new BoardPoint(BoardMap.BoardSize.W - 1 - Raw.X, BoardMap.BoardSize.H - 1 - Raw.Z);
        }

        public static ImBoardPoint FromReal(BoardPoint point, bool isLocal)
        {
            return isLocal 
                ? new ImBoardPoint(point)
                : new ImBoardPoint(new BoardPoint(BoardMap.BoardSize.W - 1 - point.X, BoardMap.BoardSize.H - 1 - point.Z));
        }

    }

}