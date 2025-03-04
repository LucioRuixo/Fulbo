using System;
using UnityEngine;

namespace Fulbo.Attributes
{
    #region Enumerators
    public enum AttributeTypes
    {
        // 0 -> Mental
        Composture = 0, 
        Determination, 
        Solidarity, 
        Charisma, 

        // 50 -> Physical
        Speed = 50, 
        Strength, 
        Endurance, 
        Aerial,

        // 100 -> Technical
        Dribbling = 100, 
        Passing, 
        Shooting, 
        Tackling, 

        // 150 -> GK
        Saving = 150
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