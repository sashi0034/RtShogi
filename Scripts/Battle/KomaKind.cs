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

    public struct KomaKind
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

        public EKomaKind? ToUnformed()
        {
            return Koma switch
            {
                EKomaKind.HuFormed => EKomaKind.Hu,
                EKomaKind.KeimaFormed => EKomaKind.Keima,
                EKomaKind.KyoshaFormed => EKomaKind.Kyosha,
                EKomaKind.KakuFormed => EKomaKind.Kaku,
                EKomaKind.HishaFormed => EKomaKind.Hisha,
                EKomaKind.GinFormed => EKomaKind.Gin,
                _ => null
            };
        }

        public bool IsUnformed()
        {
            return ToFormed() != null;
        }

        public bool IsNonFormed()
        {
            return
                Koma is EKomaKind.Oh or EKomaKind.Gyoku or EKomaKind.Kin ||
                IsUnformed();
        }

        public bool IsKing()
        {
            return Koma == EKomaKind.Oh || Koma == EKomaKind.Gyoku;
        }
    }

    public enum ETeam
    {
        Ally,
        Enemy,
    }

    public static class TeamUtil
    {
        public static ETeam FlipTeam(ETeam team)
        {
            return team switch
            {
                ETeam.Ally => ETeam.Enemy,
                ETeam.Enemy => ETeam.Ally,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };
        }
    } 
}