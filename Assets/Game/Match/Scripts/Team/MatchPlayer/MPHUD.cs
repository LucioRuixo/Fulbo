using UnityEngine;

namespace Fulbo.Match.UI
{
    public class MPHUD : MonoBehaviour
    {
        #region Constants
        private const float ArrowHeight = 0.05f;
        #endregion

        [SerializeField] private Ring ring;
        [SerializeField] private Arrow arrow;
        
        private MatchPlayer player;

        public Arrow Arrow => arrow;

        private void OnDestroy()
        {
            if (player)
            {
                player.SelectedEvent -= OnSelected;
                player.UnselectedEvent -= OnUnselected;
            }
        }

        public void Initialize(MatchPlayer player)
        {
            this.player = player;

            player.SelectedEvent += OnSelected;
            player.UnselectedEvent += OnUnselected;

            float arrowSize = player.Pitch.Length / player.Pitch.Board.SquareCount.x;
            arrow.Initialize(arrowSize, ArrowHeight);
        }

        #region Handlers
        private void OnSelected() => ring.Show();

        private void OnUnselected() => ring.Hide();
        #endregion
    }
}