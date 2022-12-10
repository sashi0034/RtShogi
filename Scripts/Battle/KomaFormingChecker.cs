using System;
using RtShogi.Scripts.Param;

namespace RtShogi.Scripts.Battle
{
    public enum EKomaFormAble
    {
        Impossible,
        FormAble,
        FormForced
    }
    
    public static class KomaFormingChecker
    {
        public static EKomaFormAble CheckFormAble(EKomaKind kind, ImBoardPoint currPoint, ImBoardPoint nextPoint)
        {
            return kind switch
            {
                EKomaKind.Hu => canFormOfHuOrKyosha(nextPoint),
                EKomaKind.Keima => canFormOfKeima(nextPoint),
                EKomaKind.Kyosha => canFormOfHuOrKyosha(nextPoint),
                EKomaKind.Kaku => canFormOfHishaOrKakuOrGin(currPoint, nextPoint),
                EKomaKind.Hisha => canFormOfHishaOrKakuOrGin(currPoint, nextPoint),
                EKomaKind.Gin => canFormOfHishaOrKakuOrGin(currPoint, nextPoint),
                EKomaKind.Kin => EKomaFormAble.Impossible,
                EKomaKind.Gyoku => EKomaFormAble.Impossible,
                EKomaKind.Oh => EKomaFormAble.Impossible,
                EKomaKind.HuFormed => EKomaFormAble.Impossible,
                EKomaKind.KeimaFormed => EKomaFormAble.Impossible,
                EKomaKind.KyoshaFormed => EKomaFormAble.Impossible,
                EKomaKind.KakuFormed => EKomaFormAble.Impossible,
                EKomaKind.HishaFormed => EKomaFormAble.Impossible,
                EKomaKind.GinFormed => EKomaFormAble.Impossible,
                _ => throw new NotImplementedException()
            };
        }

        private static EKomaFormAble canFormOfHuOrKyosha(ImBoardPoint nextPoint)
        {
            if (nextPoint.Raw.Z >= ConstParameter.BoardSize.H - 1) return EKomaFormAble.FormForced;
            if (nextPoint.Raw.Z >= ConstParameter.BoardSize.H - 3) return EKomaFormAble.FormAble;
            return EKomaFormAble.Impossible;
        }
        private static EKomaFormAble canFormOfKeima(ImBoardPoint nextPoint)
        {
            if (nextPoint.Raw.Z >= ConstParameter.BoardSize.H - 2) return EKomaFormAble.FormForced;
            if (nextPoint.Raw.Z >= ConstParameter.BoardSize.H - 3) return EKomaFormAble.FormAble;
            return EKomaFormAble.Impossible;
        }
        private static EKomaFormAble canFormOfHishaOrKakuOrGin(ImBoardPoint currPoint, ImBoardPoint nextPoint)
        {
            if (currPoint.Raw.Z >= ConstParameter.BoardSize.H - 3 || nextPoint.Raw.Z >= ConstParameter.BoardSize.H - 3) 
                return EKomaFormAble.FormAble;
            return EKomaFormAble.Impossible;
        }
    }
}