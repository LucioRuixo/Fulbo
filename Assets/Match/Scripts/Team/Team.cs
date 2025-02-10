using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Match
{
    public class Team : MonoBehaviour
    {
        #region Constants
        public const int PlayerCount = 1;
        #endregion

        private Match match;

        public Sides Side { get; private set; } = Sides.None;

        public Halves DefendedHalf { get; private set; }
        public Halves AttackedHalf { get; private set; }

        public Goal DefendedGoal { get; private set; }
        public Goal AttackedGoal { get; private set; }

        public List<MatchPlayer> Players { get; private set; } = new List<MatchPlayer>();

        public void Initialize(Sides side, Match match, GameObject playerPrefab)
        {
            Side = side;

            this.match = match;
            DefendedHalf = match.GetDefendedHalfBySide(Side);
            AttackedHalf = match.GetAttackedHalfBySide(Side);
            DefendedGoal = match.GetDefendedGoalBySide(Side);
            AttackedGoal = match.GetAttackedGoalBySide(Side);

            SpawnPlayers(playerPrefab);
        }

        private void SpawnPlayers(GameObject playerPrefab)
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                MatchPlayer player = Instantiate(playerPrefab, transform).GetComponent<MatchPlayer>();
                player.Initialize(this, match);
                player.transform.position = player.StartSquare.Position;
                Players.Add(player);
            }
        }

        public static Color GetColor(Sides side) => side == Sides.Home ? Color.blue : side == Sides.Away ? Color.red : Color.white;
    }
}