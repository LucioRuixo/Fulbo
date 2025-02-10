using UnityEngine;

namespace Fulbo.Match
{
    [RequireComponent(typeof(MatchInitializer))]
    public class Match : MonoBehaviour
    {
        [SerializeField] private Pitch pitch;

        public Pitch Pitch => pitch;

        private void Awake() => GetComponent<MatchInitializer>().Initialize(this);

        public Halves GetDefendedHalfBySide(Sides side) => side == Sides.Home ? Halves.Left : Halves.Right;
        public Halves GetAttackedHalfBySide(Sides side) => side == Sides.Home ? Halves.Right : Halves.Left;

        public Goal GetDefendedGoalBySide(Sides side) => side == Sides.Home ? Pitch.LeftGoal : Pitch.RightGoal;
        public Goal GetAttackedGoalBySide(Sides side) => side == Sides.Home ? Pitch.RightGoal : Pitch.LeftGoal;
    }
}