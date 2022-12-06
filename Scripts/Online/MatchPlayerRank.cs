using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;

namespace RtShogi.Scripts.Online
{
    public readonly struct MatchPlayerRank
    {
        public readonly int Value;
        private const int InvalidValue = 0;
        private const int MinValue = 1;
        private const int MaxValue = 3;

        public MatchPlayerRank(int value)
        {
            Value = value;
        }

        public static MatchPlayerRank MakeRandom()
        {
            return new MatchPlayerRank(Random.Range(MinValue, MaxValue + 1));
        }

        public static MatchPlayerRank MakeInvalidRank()
        {
            return new MatchPlayerRank(InvalidValue);
        }

        public List<MatchPlayerRank> GetNearRanks()
        {
            var down = new MatchPlayerRank(Value - 1);
            var up = new MatchPlayerRank(Value + 1);
            return new List<MatchPlayerRank> { down, up }.Where(r => r.IsValid()).ToList();
        }

        public bool IsValid()
        {
            return Value != InvalidValue && MinValue <= Value && Value <= MaxValue;
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