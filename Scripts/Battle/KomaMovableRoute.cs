#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace RtShogi.Scripts.Battle
{
    public record KomaMovableRoute(
        EKomaKind Kind,
        ImBoardPoint CurrPoint,
        ETeam OpponentTeam,
        Func<ImBoardPoint, KomaUnit?> KomaGetter)
    {
        BoardPoint currPoint => CurrPoint.Raw;
        private Func<BoardPoint, KomaUnit?> komaGetter => (point) => KomaGetter(new ImBoardPoint(point));
        private int sideLength => BoardManager.SideLength;

        private bool isNullOrEnemyPoint(BoardPoint point)
        {
            if (!point.IsValidPoint()) return false;
            var koma = komaGetter(point);
            if (koma == null) return true;
            return koma.Team == OpponentTeam;
        }

        private List<BoardPoint> includeOnlyNullOrEnemy(List<BoardPoint> list)
        {
            return list.Where(isNullOrEnemyPoint).ToList();
        }
        
        public List<ImBoardPoint> GetMovablePoints()
        {
            return (Kind switch
            {
                EKomaKind.Hu => includeOnlyNullOrEnemy(new List<BoardPoint>() { currPoint.Move(0, 1) }),
                EKomaKind.Keima => includeOnlyNullOrEnemy(new List<BoardPoint>() { currPoint.Move(-1, 2), currPoint.Move(1, 2) }),
                EKomaKind.Kyosha => getMovableOfKyosha(),
                EKomaKind.Kaku => getMovableOfKaku(false),
                EKomaKind.Hisha => getMovableOfHisha(false),
                EKomaKind.Gin => getMovableOfGin(),
                EKomaKind.Kin => getMovableOfKin(),
                EKomaKind.Gyoku => getMovableOfOh(),
                EKomaKind.Oh => getMovableOfOh(),
                EKomaKind.HuFormed => getMovableOfKin(),
                EKomaKind.KeimaFormed => getMovableOfKin(),
                EKomaKind.KyoshaFormed => getMovableOfKin(),
                EKomaKind.KakuFormed => getMovableOfKaku(true),
                EKomaKind.HishaFormed => getMovableOfHisha(true),
                EKomaKind.GinFormed => getMovableOfKin(),
                _ => throw new NotImplementedException()
            }).Select(point => new ImBoardPoint(point)).ToList();
        }

        private List<BoardPoint> getMovableOfKyosha()
        {
            var result = new List<BoardPoint>();
            pushUntilEnemyFound(result, 0, 1);
            return result;
        }
        
        private void pushUntilEnemyFound(List<BoardPoint> currList, int vecX, int vecZ)
        {
            foreach (var index in Enumerable.Range(1, sideLength))
            {
                var next = currPoint.Move(index * vecX, index * vecZ);
                if (!next.IsValidPoint()) break;
                
                var koma = komaGetter(next);
                if (koma != null && koma.Team != OpponentTeam) break;
                
                currList.Add(next);
                if (koma != null && koma.Team == OpponentTeam) break;
            }
        }

        private List<BoardPoint> getMovableOfKaku(bool isFormed)
        {
            var result = includeOnlyNullOrEnemy(isFormed
                ? new List<BoardPoint>
                {
                    currPoint.Move(0, 1), currPoint.Move(0, -1),
                    currPoint.Move(-1, 0), currPoint.Move(1, 0)
                }
                : new List<BoardPoint> {});
            pushUntilEnemyFound(result, 1, 1);
            pushUntilEnemyFound(result, 1, -1);
            pushUntilEnemyFound(result, -1, 1);
            pushUntilEnemyFound(result, -1, -1);
            return result;
        }
        
        private List<BoardPoint> getMovableOfHisha(bool isFormed)
        {
            var result = includeOnlyNullOrEnemy(isFormed
                ? new List<BoardPoint>
                {
                    currPoint.Move(1, 1), currPoint.Move(1, -1),
                    currPoint.Move(-1, 1), currPoint.Move(-1, -1)
                }
                : new List<BoardPoint> {});
            pushUntilEnemyFound(result, 1, 0);
            pushUntilEnemyFound(result, -1, 0);
            pushUntilEnemyFound(result, 0, 1);
            pushUntilEnemyFound(result, 0, -1);
            return result;
        }
        
        private List<BoardPoint> getMovableOfGin()
        {
            return includeOnlyNullOrEnemy(new List<BoardPoint>
            {
                currPoint.Move(-1, 1), currPoint.Move(0, 1), currPoint.Move(1, 1),
                currPoint.Move(-1, -1), currPoint.Move(1, -1),
            });
        }
        
        private List<BoardPoint> getMovableOfKin()
        {
            return includeOnlyNullOrEnemy(new List<BoardPoint>
            {
                currPoint.Move(-1, 1), currPoint.Move(+0, 1), currPoint.Move(+1, 1), 
                currPoint.Move(-1, 0), currPoint.Move(+1, 0),
                currPoint.Move(+0, -1),
            });
        }
        
        private List<BoardPoint> getMovableOfOh()
        {
            return includeOnlyNullOrEnemy(new List<BoardPoint>
            {
                currPoint.Move(-1, +1), currPoint.Move(0, +1), currPoint.Move(1, +1),
                currPoint.Move(-1, +0), currPoint.Move(1, +0),
                currPoint.Move(-1, -1), currPoint.Move(0, -1), currPoint.Move(1, -1),
            });
        }

    }
}