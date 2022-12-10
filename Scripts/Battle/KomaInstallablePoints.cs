#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using RtShogi.Scripts.Param;

namespace RtShogi.Scripts.Battle
{
    public record KomaInstallablePoints(        
        EKomaKind Kind,
        ETeam Team,
        Func<ImBoardPoint, KomaUnit?> KomaGetter)
    {
        public List<ImBoardPoint> GetInstallablePoints()
        {
            // TODO: 正しいものを作る
            // TODO: 次回
            return Kind switch
            {
                EKomaKind.Hu => getInstallableOfHu(),
                EKomaKind.Keima => getInstallableOfRegular(2),
                EKomaKind.Kyosha => getInstallableOfRegular(1),
                EKomaKind.Kaku => getInstallableOfRegular(0),
                EKomaKind.Hisha => getInstallableOfRegular(0),
                EKomaKind.Gin => getInstallableOfRegular(0),
                EKomaKind.Kin => getInstallableOfRegular(0),
                EKomaKind.Gyoku => throw new NotImplementedException(),
                EKomaKind.Oh => throw new NotImplementedException(),
                EKomaKind.HuFormed => throw new NotImplementedException(),
                EKomaKind.KeimaFormed => throw new NotImplementedException(),
                EKomaKind.KyoshaFormed => throw new NotImplementedException(),
                EKomaKind.KakuFormed => throw new NotImplementedException(),
                EKomaKind.HishaFormed => throw new NotImplementedException(),
                EKomaKind.GinFormed => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private List<ImBoardPoint> getInstallableOfHu()
        {
            var result = new List<ImBoardPoint>() { };
            foreach (int x in Enumerable.Range(0, ConstParameter.BoardSideLength))
            {
                int numZ = ConstParameter.BoardSideLength - 1;
                if (isHuOverlapLineX(x, numZ)) continue;
                
                result.AddRange(
                    from z in Enumerable.Range(0, numZ) 
                    let koma = KomaGetter(new ImBoardPoint(x, z)) 
                    where koma == null
                    select new ImBoardPoint(x, z));
            }

            return result;
        }

        private bool isHuOverlapLineX(int currX, int numZ)
        {
            foreach (int z in Enumerable.Range(0, numZ))
            {
                var koma = KomaGetter(new ImBoardPoint(currX, z));
                if (koma == null) continue;
                bool hasHu = koma.Kind == EKomaKind.Hu && koma.Team == Team;
                if (hasHu) return true;
            }
            return false;
        }

        private List<ImBoardPoint> getInstallableOfRegular(int invalidEndZ)
        {
            return (
                from x in Enumerable.Range(0, ConstParameter.BoardSideLength)
                from z in Enumerable.Range(0, ConstParameter.BoardSideLength - invalidEndZ)
                where KomaGetter(new ImBoardPoint(x, z)) == null
                select new ImBoardPoint(x, z)
            ).ToList();
        }
    }
}