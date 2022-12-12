using System.Collections.Generic;
using System.Linq;
using RtShogi.Scripts.Battle;
using UnityEngine;

namespace RtShogi.Scripts.Param
{
    [CreateAssetMenu(fileName = nameof(ConstParameter), menuName = "ScriptableObjects/Create" + nameof(ConstParameter))]
    public class ConstParameter : SingletonScriptableObject<ConstParameter>
    {
        [SerializeField] private float komaFormAbleEffectiveTime = 3f;
        public float KomaFormAbleEffectiveTime => komaFormAbleEffectiveTime;

        [SerializeField] private Color[] boardPieceHighlightColors;
        public IReadOnlyList<Color> BoardPieceHighlightColors => boardPieceHighlightColors;
        
        [SerializeField] private float[] cooldownTimeList;
        public float[] CooldownTimeList => cooldownTimeList;

        public const int MinPlayerRank = 1;

        [SerializeField] private int maxPlayerRank = 1;
        public int MaxPlayerRank => maxPlayerRank;

        [SerializeField] private int maxPlayerNameLength = 12;
        public int MaxPlayerNameLength => maxPlayerNameLength;

        [SerializeField] private int maxMatchingWaitSeconds = 60 * 10;
        public int MaxMatchingWaitSeconds => maxMatchingWaitSeconds;
        

        private static KomaInitialPutInfo[] _komaInitialPutInfo = getKomaInitialPutInfos();
        public static IReadOnlyList<KomaInitialPutInfo> KomaInitialPutInfos => _komaInitialPutInfo;
        
        public const int BoardSideLength = 9;
        
        public static readonly IntSize BoardSize = new IntSize(BoardSideLength, BoardSideLength);

        public const string SaveDataKey = "JsonSaveData"; 
        
        private static KomaInitialPutInfo[] getKomaInitialPutInfos()
        {
            var result = new List<KomaInitialPutInfo>();
            
            foreach(int x in Enumerable.Range(0, 9))
                result.Add(new KomaInitialPutInfo(new BoardPoint(x, 2), EKomaKind.Hu));
            
            result.Add(new KomaInitialPutInfo(new BoardPoint(1, 1), EKomaKind.Kaku));
            result.Add(new KomaInitialPutInfo(new BoardPoint(7, 1), EKomaKind.Hisha));
            
            result.Add(new KomaInitialPutInfo(new BoardPoint(0, 0), EKomaKind.Kyosha));
            result.Add(new KomaInitialPutInfo(new BoardPoint(1, 0), EKomaKind.Keima));
            result.Add(new KomaInitialPutInfo(new BoardPoint(2, 0), EKomaKind.Gin));
            result.Add(new KomaInitialPutInfo(new BoardPoint(3, 0), EKomaKind.Kin));
            result.Add(new KomaInitialPutInfo(new BoardPoint(4, 0), EKomaKind.Oh));
            result.Add(new KomaInitialPutInfo(new BoardPoint(5, 0), EKomaKind.Kin));
            result.Add(new KomaInitialPutInfo(new BoardPoint(6, 0), EKomaKind.Gin));
            result.Add(new KomaInitialPutInfo(new BoardPoint(7, 0), EKomaKind.Keima));
            result.Add(new KomaInitialPutInfo(new BoardPoint(8, 0), EKomaKind.Kyosha));

            return result.ToArray();
        }
        
    }
}