using TMPro;
using UnityEngine;

namespace Fulbo.Match.UI
{
    public class EventSign : MonoBehaviour
    {
        #region Contants
        private const string ShotMissedText = "SHOT MISSED!";
        private const string ShotSavedText = "SHOT SAVED!";
        private const string GoalText = "GOAL!";
        #endregion

        [SerializeField] private Match match;

        [Space]

        [SerializeField] private RectTransform container;
        [SerializeField] private TMP_Text eventText;

        private void Awake()
        {
            match.ShotMissedEvent += OnShotMissed;
            match.ShotSavedEvent += OnShotSaved;
            match.GoalEvent += OnShotGoal;
            match.PlayEndEvent += OnPlayEnd;
        }

        private void OnDestroy()
        {
            if (match)
            {
                match.ShotMissedEvent -= OnShotMissed;
                match.ShotSavedEvent -= OnShotSaved;
                match.GoalEvent -= OnShotGoal;
                match.PlayEndEvent -= OnPlayEnd;
            }
        }

        private void Show(string text)
        {
            eventText.text = text;
            container.gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();
        }

        private void Hide()
        {
            eventText.text = "";
            container.gameObject.SetActive(false);
        }

        #region Handlers
        private void OnPlayEnd() => Hide();

        private void OnShotMissed(MatchPlayer kicker) => Show(ShotMissedText);

        private void OnShotSaved(MatchPlayer kicker) => Show(ShotSavedText);

        private void OnShotGoal(Sides scoringSide) => Show(GoalText);
        #endregion
    }
}