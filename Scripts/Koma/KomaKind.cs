namespace RtShogi.Scripts.Koma
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
    }
    
    public enum EKomaTeam
    {
        Ally,
        Enemy,
    }
}