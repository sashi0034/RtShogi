#nullable enable

using System;

namespace RtShogi.Scripts.Battle
{
    public enum EKomaKind
    {
        // 表
        Hu = 0,
        Keima,
        Kyosha,
        Kaku,
        Hisha,
        Gin,
        Kin,
        Gyoku,
        Oh,

        // 成り
        HuFormed,
        KeimaFormed,
        KyoshaFormed,
        KakuFormed,
        HishaFormed,
        GinFormed,
    }

    public class KomaKind
    {
        public const EKomaKind UnformedLastKind = EKomaKind.Oh;
        public const int NumUnformed = (int)UnformedLastKind + 1;
        public const int CountKinds = (int)EKomaKind.GinFormed + 1;

        public readonly EKomaKind Koma;
        public KomaKind(EKomaKind koma)
        {
            Koma = koma;
        }

        public EKomaKind? ToFormed()
        {
            return Koma switch
            {
                EKomaKind.Hu => EKomaKind.HuFormed,
                EKomaKind.Keima => EKomaKind.KeimaFormed,
                EKomaKind.Kyosha => EKomaKind.KyoshaFormed,
                EKomaKind.Kaku => EKomaKind.KakuFormed,
                EKomaKind.Hisha => EKomaKind.HishaFormed,
                EKomaKind.Gin => EKomaKind.GinFormed,
                _ => null
            };
        }
    }
    
    public enum ETeam
    {
        Ally,
        Enemy,
    }
}