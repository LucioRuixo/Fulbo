using UnityEngine;

namespace Fulbo.Match
{
    [RequireComponent(typeof(Match))]
    public class MatchInitializer : MonoBehaviour
    {
        private Match match;

        [SerializeField] private Team homeTeam;
        [SerializeField] private Team awayTeam;

        [Space]

        [SerializeField] private GameObject playerPrefab;

        public void Initialize(Match match)
        {
            this.match = match;

            match.Pitch.Initialize();
            homeTeam.Initialize(Sides.Home, match, playerPrefab);
        }
    }
}