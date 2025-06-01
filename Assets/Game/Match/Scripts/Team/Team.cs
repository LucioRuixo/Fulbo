using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fulbo.Match
{
    public class Team : MonoBehaviour
    {
        #region Constants
        public const int PlayerCount = 11;
        #endregion

        private Match match;

        public Sides Side { get; private set; } = Sides.None;

        public Halves DefendedHalf { get; private set; }
        public Halves AttackedHalf { get; private set; }

        public Goal DefendedGoal { get; private set; }
        public Goal AttackedGoal { get; private set; }

        public Vector3 AttackDirection { get; private set; }

        public Team Rival => match.GetRival(Side);

        public List<MatchPlayer> Players { get; private set; } = new List<MatchPlayer>();
        public MatchPlayer GK => Players[0];

        private static Material homeMaterial;
        private static Material awayMaterial;

        public void Initialize(Sides side, Match match, GameObject playerPrefab)
        {
            Side = side;

            this.match = match;
            DefendedHalf = match.GetDefendedHalfBySide(Side);
            AttackedHalf = match.GetAttackedHalfBySide(Side);
            DefendedGoal = match.GetDefendedGoalBySide(Side);
            AttackedGoal = match.GetAttackedGoalBySide(Side);
            AttackDirection = Side == Sides.Home ? Vector3.right : Vector3.left;

            SpawnPlayers(playerPrefab);
        }

        private void SpawnPlayers(GameObject playerPrefab)
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                MatchPlayer player = Instantiate(playerPrefab, transform).GetComponent<MatchPlayer>();
                player.Initialize(i, this, match);
                player.name = $"{Side} | {i}";
                Players.Add(player);
            }
        }

        public void ResetPlayers()
        {
            foreach (MatchPlayer player in Players)
            {
                player.transform.rotation = Quaternion.LookRotation(player.AttackDirection, Vector3.up);
                player.StartSquare.AddPlayer(player);
            }
        }

        public MatchPlayer[] GetPlayers(MatchPlayer[] exclude = null) => Players.Where(player => exclude == null || !exclude.Contains(player)).ToArray();

        public MatchPlayer[] GetClosestPlayersToSquare(Square square, MatchPlayer[] exclude = null)
        {
            List<MatchPlayer> closestPlayers = new List<MatchPlayer>();

            float smallestDistance = float.MaxValue;
            MatchPlayer[] players = exclude == null ? Players.ToArray() : Players.Except(exclude).ToArray();
            foreach (MatchPlayer player in players)
            {
                float distance = MatchUtils.Distance(player.CurrentSquare, square);

                if (distance == smallestDistance) closestPlayers.Add(player);
                else if (distance < smallestDistance)
                {
                    closestPlayers = new List<MatchPlayer>() { player };
                    smallestDistance = distance;
                }
            }

            return closestPlayers.ToArray();
        }

        public MatchPlayer GetRandomClosestPlayerToSquare(Square square, MatchPlayer[] exclude = null)
        {
            MatchPlayer[] closestPlayers = GetClosestPlayersToSquare(square, exclude);
            return closestPlayers[Random.Range(0, closestPlayers.Length)];
        }

        // Debug
        // --------------------
        public static string GetAbbreviation(Sides side) => side == Sides.None ? null : side.ToString().Abbreviate();

        public static Color GetColor(Sides side)
        {
            if (side == Sides.Home) return Color.blue;
            else if (side == Sides.Away) return Color.red;
            else return Color.white;
        }

        public static Material GetMaterial(Sides side)
        {
            if (side == Sides.Home) return homeMaterial ??= Resources.Load<Material>("Home");
            else if (side == Sides.Away) return awayMaterial ??= Resources.Load<Material>("Away");
            else return null;
        }
        // --------------------
    }
}