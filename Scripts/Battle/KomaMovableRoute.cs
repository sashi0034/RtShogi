using System;
using System.Collections.Generic;
using System.Linq;

namespace RtShogi.Scripts.Battle
{
    public record KomaMovableRoute(
        EKomaKind Kind,
        ImBoardPoint CurrPoint,
        Func<ImBoardPoint, bool> CanMoveTo)
    {
        BoardPoint currPoint => CurrPoint.Raw;
        private Func<BoardPoint, bool> canMoveTo => (point) => CanMoveTo(new ImBoardPoint(point));
        private int sideLength => BoardManager.SideLength;

        private List<BoardPoint> validate(List<BoardPoint> list)
        {
            return list.Where(point => canMoveTo(point)).ToList();
        }
        
        public List<BoardPoint> GetMovablePoints()
        {
            return Kind switch
            {
                EKomaKind.Hu => validate(new List<BoardPoint>() { currPoint.Move(0, 1) }),
                EKomaKind.Keima => validate(new List<BoardPoint>() { currPoint.Move(-1, 2), currPoint.Move(1, 2) }),
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
                var next = currPoint.Move(0, z);
                if (!canMoveTo(next)) break;
                result.Add(next);
            }
            return result;
        }

        private List<BoardPoint> getMovableOfKaku(bool isFormed)
        {
            var result = validate(isFormed
                ? new List<BoardPoint>
                {
                    currPoint.Move(0, 1), currPoint.Move(0, -1),
                    currPoint.Move(-1, 0), currPoint.Move(1, 0)
                }
                : new List<BoardPoint> {});
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => currPoint.Move(i, i)).TakeWhile(next => canMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => currPoint.Move(i, -i)).TakeWhile(next => canMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => currPoint.Move(-i, i)).TakeWhile(next => canMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => currPoint.Move(-i, -i)).TakeWhile(next => canMoveTo(next)));
            return result;
        }
        
        private List<BoardPoint> getMovableOfHisha(bool isFormed)
        {
            var result = validate(isFormed
                ? new List<BoardPoint>
                {
                    currPoint.Move(1, 1), currPoint.Move(1, -1),
                    currPoint.Move(-1, 1), currPoint.Move(-1, -1)
                }
                : new List<BoardPoint> {});
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => currPoint.Move(i, 0)).TakeWhile(next => canMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => currPoint.Move(-i, 0)).TakeWhile(next => canMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => currPoint.Move(0, i)).TakeWhile(next => canMoveTo(next)));
            result.AddRange(Enumerable.Range(1, sideLength)
                .Select(i => currPoint.Move(0, -i)).TakeWhile(next => canMoveTo(next)));
            return result;
        }
        
        private List<BoardPoint> getMovableOfGin()
        {
            return validate(new List<BoardPoint>
            {
                currPoint.Move(-1, 1), currPoint.Move(0, 1), currPoint.Move(1, 1),
                currPoint.Move(-1, -1), currPoint.Move(1, -1),
            });
        }
        
        private List<BoardPoint> getMovableOfKin()
        {
            return validate(new List<BoardPoint>
            {
                currPoint.Move(-1, 1), currPoint.Move(+0, 1), currPoint.Move(+1, 1), 
                currPoint.Move(-1, 0), currPoint.Move(+1, 0),
                currPoint.Move(+0, -1),
            });
        }
        
        private List<BoardPoint> getMovableOfOh()
        {
            return validate(new List<BoardPoint>
            {
                currPoint.Move(-1, +1), currPoint.Move(0, +1), currPoint.Move(1, +1),
                currPoint.Move(-1, +0), currPoint.Move(1, +0),
                currPoint.Move(-1, -1), currPoint.Move(0, -1), currPoint.Move(1, -1),
            });
        }

    }
}