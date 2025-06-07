using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Match.UI
{
    public class FocusBar : MonoBehaviour
    {
        [SerializeField] private Transform cellContainer;

        private MatchPlayer player;

        private List<GameObject> cells = new List<GameObject>();

        private void OnDestroy() => player.Focus.ChargeEvent -= OnFocusCharge;

        public void Initialize(MatchPlayer player)
        {
            this.player = player;

            for (int i = 0; i < Focus.MaxCharge; i++) cells.Add(cellContainer.GetChild(i).gameObject);
            OnFocusCharge(player.Focus.Charge);

            player.Focus.ChargeEvent += OnFocusCharge;
        }

        #region Handlers
        private void OnFocusCharge(int charge)
        {
            for (int i = 0; i < Focus.MaxCharge; i++) cells[i].SetActive(i < charge);
        }
        #endregion
    }
}