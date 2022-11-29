using System;
using System.Collections.Generic;
using System.Linq;

namespace RtShogi.Scripts.Battle
{
    public record KomaMovableRoute(
        EKomaKind Kind,
        BoardPoint CurrPoint,
        Func<BoardPoint, bool> CanMoveTo)
    {
        private int sideLength => BoardManager.SideLength;

        private List<BoardPoint> validate(List<BoardPoint> list)
        {
            return list.Where(point => CanMoveTo(point)).ToList();
        }
        
        public List<BoardPoint> GetMovablePoints()
        {
            return Kind switch
            {
                EKomaKind.Hu => validate(new List<BoardPoint>() { CurrPoint.Move(0, 1) }),
                EKomaKind.Keima => validate(new List<BoardPoint>() { CurrPoint.Move(-1, 2), CurrPoint.Move(1, 2) }),
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
            };
        }

        private List<BoardPoint> getMovableOfKyosha()
        {
            var result = new List<BoardPoint>() { };
            foreach (var z in Enumerable.Range(1, sideLength))
            {
                var next = CurrPoint.Move(0, z);
                if (!CanMoveTo(next)) break;
                result.Add(next);
            }
            return result;
        }

        private List<BoardPoint> getMovableOfKaku(bool isFormed)
        {
            var result = validate(isFormed
                ? new List<BoardPoint>
                {
                    CurrPoint.Move(0, 1), CurrPoint.Move(0, -1),
                    CurrPoint.Move(-1, 0), CurrPoint.Move(1, 0)
                }
                : new List<BoardPoint> {});
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => CurrPoint.Move(i, i)).TakeWhile(next => CanMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => CurrPoint.Move(i, -i)).TakeWhile(next => CanMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => CurrPoint.Move(-i, i)).TakeWhile(next => CanMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => CurrPoint.Move(-i, -i)).TakeWhile(next => CanMoveTo(next)));
            return result;
        }
        
        private List<BoardPoint> getMovableOfHisha(bool isFormed)
        {
            var result = validate(isFormed
                ? new List<BoardPoint>
                {
                    CurrPoint.Move(1, 1), CurrPoint.Move(1, -1),
                    CurrPoint.Move(-1, 1), CurrPoint.Move(-1, -1)
                }
                : new List<BoardPoint> {});
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => CurrPoint.Move(i, 0)).TakeWhile(next => CanMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => CurrPoint.Move(-i, 0)).TakeWhile(next => CanMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => CurrPoint.Move(0, i)).TakeWhile(next => CanMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => CurrPoint.Move(0, -i)).TakeWhile(next => CanMoveTo(next)));
            return result;
        }
        
        private List<BoardPoint> getMovableOfGin()
        {
            return validate(new List<BoardPoint>
            {
                CurrPoint.Move(-1, 1), CurrPoint.Move(0, 1), CurrPoint.Move(1, 1),
                CurrPoint.Move(-1, -1), CurrPoint.Move(1, -1),
            });
        }
        
        private List<BoardPoint> getMovableOfKin()
        {
            return validate(new List<BoardPoint>
            {
                CurrPoint.Move(-1, 1), CurrPoint.Move(+0, 1), CurrPoint.Move(+1, 1), 
                CurrPoint.Move(-1, 0), CurrPoint.Move(+1, 0),
                CurrPoint.Move(+0, -1),
            });
        }
        
        private List<BoardPoint> getMovableOfOh()
        {
            return validate(new List<BoardPoint>
            {
                CurrPoint.Move(-1, +1), CurrPoint.Move(0, +1), CurrPoint.Move(1, +1),
                CurrPoint.Move(-1, +0), CurrPoint.Move(1, +0),
                CurrPoint.Move(-1, -1), CurrPoint.Move(0, -1), CurrPoint.Move(1, -1),
            });
        }

    }
}