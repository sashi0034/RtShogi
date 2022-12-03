#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace RtShogi.Scripts.Battle
{
    public record KomaInstallablePoints(        
        EKomaKind Kind,
        Func<ImBoardPoint, EKomaKind?> KomaGetter)
    {
        public List<ImBoardPoint> GetInstallablePoints()
        {
            // TODO: 正しいものを作る
            return 
                (from x in Enumerable.Range(0, BoardMap.SideLength)
                from z in Enumerable.Range(0, BoardMap.SideLength) 
                where !KomaGetter(new ImBoardPoint(new BoardPoint(x, z))).HasValue
                select new ImBoardPoint(new BoardPoint(x, z))).ToList();
        }
    }
}