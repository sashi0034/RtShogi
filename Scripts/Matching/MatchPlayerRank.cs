using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using RtShogi.Scripts.Param;
using UnityEngine;

namespace RtShogi.Scripts.Matching
{
    public readonly struct MatchPlayerRank
    {
        public readonly int Value;
        private const int invalidValue = 0;
        private const int minValue = ConstParameter.MinPlayerRank;
        private static int maxValue => ConstParameter.Instance.MaxPlayerRank;

        public MatchPlayerRank(int value)
        {
            Value = value;
        }

        public static MatchPlayerRank MakeRandom()
        {
            return new MatchPlayerRank(Random.Range(minValue, maxValue + 1));
        }

        public static MatchPlayerRank MakeInvalidRank()
        {
            return new MatchPlayerRank(invalidValue);
        }

        public List<MatchPlayerRank> GetNearRanks()
        {
            var down = new MatchPlayerRank(Value - 1);
            var up = new MatchPlayerRank(Value + 1);
            return new List<MatchPlayerRank> { down, up }.Where(r => r.IsValid()).ToList();
        }

        public bool IsValid()
        {
            return Value != invalidValue && minValue <= Value && Value <= maxValue;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    static class MatchPlayerRankUtil
    {
        public const string KeyRank = "rank";

        public static Hashtable CreateRankPropsRandomly()
        {
            return CreateRankProps(MatchPlayerRank.MakeRandom());
        }

        public static Hashtable CreateRankProps(MatchPlayerRank rank)
        {
            var props = new Hashtable
            {
                [KeyRank] = rank.Value
            };
            return props;
        }

        public static MatchPlayerRank GetFromRankProps(Hashtable props)
        {
            if (props[KeyRank] is int rank) return new MatchPlayerRank(rank);
            
            return MatchPlayerRank.MakeInvalidRank();
        }
    }
}