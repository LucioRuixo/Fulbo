using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Match.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private Match match;

        [Space]

        [SerializeField] private TMP_Text homeScore;
        [SerializeField] private TMP_Text awayScore;

        [Space]

        [SerializeField] private TMP_Text homeName;
        [SerializeField] private TMP_Text awayName;

        [Space]

        [SerializeField] private Image homeColor;
        [SerializeField] private Image awayColor;

        private void Awake() => match.MatchStartEvent += OnMatchStart;

        private void Start()
        {
            homeScore.text = 0.ToString();
            awayScore.text = 0.ToString();

            homeName.text = Team.GetAbbreviation(Sides.Home);
            awayName.text = Team.GetAbbreviation(Sides.Away);

            homeColor.color = Team.GetMaterial(Sides.Home).color;
            awayColor.color = Team.GetMaterial(Sides.Away).color;
        }

        private void OnDestroy()
        {
            match.MatchStartEvent -= OnMatchStart;
            match.Info.ScoreUpdateEvent -= OnScoreUpdate;
        }

        private TMP_Text GetScore(Sides side) => side == Sides.Home ? homeScore : side == Sides.Away ? awayScore : null;

        #region Handlers
        private void OnMatchStart() => match.Info.ScoreUpdateEvent += OnScoreUpdate;

        private void OnScoreUpdate(Sides side, uint score)
        {
            if (side == Sides.None) return;

            GetScore(side).text = score.ToString();
        }
        #endregion
    }
}