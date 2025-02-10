using UnityEngine;

namespace Fulbo.Match
{
    public class MatchPlayer : MonoBehaviour
    {
        [SerializeField] private MatchPlayerBody body;

        private Match match;

        public Vector3 Position { get => transform.position; set => transform.position = value; }

        public Team Team { get; private set; }
        public Sides Side => Team.Side;
        public Halves DefendedHalf => Team.DefendedHalf;
        public Halves AttackedHalf => Team.AttackedHalf;
        public Goal DefendedGoal => Team.DefendedGoal;
        public Goal AttackedGoal => Team.AttackedGoal;

        public Square StartSquare { get; private set; }
        public Square CurrentSquare { get; private set; }

        public Pitch Pitch => match.Pitch;

        private void OnDestroy() => Pitch.Squares.PlayerMovedToSquareEvent -= OnPlayerMovedToSquare;

        public void Initialize(Team team, Match match)
        {
            this.match = match;
            Team = team;

            StartSquare = Pitch.Squares.Get(9, 5);
            Pitch.Squares.PlayerMovedToSquareEvent += OnPlayerMovedToSquare;

            body.GetComponent<MeshRenderer>().sharedMaterial.color = Team.GetColor(Side);
        }

        #region Handlers
        private void OnPlayerMovedToSquare(Square square, MatchPlayer player)
        {
            if (player != this) return;

            CurrentSquare = square;
        }
        #endregion
    }
}