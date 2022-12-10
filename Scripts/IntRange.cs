﻿using UnityEngine.ProBuilder;
using UnityEngine.Timeline;
using Math = System.Math;

namespace RtShogi.Scripts
{
    public struct IntRange
    {
        public readonly int Min;
        public readonly int Max;

        public IntRange(int inclusiveMin, int inclusiveMax)
        {
            Min = inclusiveMin;
            Max = inclusiveMax;
        }

        public bool IsInRange(int value)
        {
            return Min <= value && value <= Max;
        }

        public int RoundInRange(int value)
        {
            return Math.Max(Min, Math.Min(value, Max));
        }
    }
}