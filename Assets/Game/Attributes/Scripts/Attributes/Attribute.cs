using System;
using UnityEngine;

namespace Fulbo.Attributes
{
    #region Enumerators
    [Flags]
    public enum AttributeTypes
    {
        // 1 << 1 -> Mental
        Composture = 1 << 1,
        Determination = 1 << 2,
        Solidarity = 1 << 3,
        Charisma = 1 << 4,

        // 1 << 11 -> Physical
        Speed = 1 << 11,
        Strength = 1 << 12,
        Endurance = 1 << 13,
        Aerial = 1 << 14,

        // 1 << 21 -> Technical
        Dribbling = 1 << 21,
        Passing = 1 << 22,
        Shooting = 1 << 23,
        Tackling = 1 << 24,

        // 1 << 31 -> GK
        Saving = 1 << 31
    }
    #endregion

    [Serializable]
    public abstract class Attribute
    {
        [SerializeField] private int score;

        public abstract string DisplayName { get; }

        public virtual int MinValue => 1;
        public virtual int MaxValue => 20;

        public int Score { get => score; set => Mathf.Clamp(value, MinValue, MaxValue); }
        public int Modifier => Score;

        public Attribute(int score) => this.score = score;
    }
}