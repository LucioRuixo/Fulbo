using System;
using UnityEngine;

namespace Fulbo.Match
{
    public class ActionPoints
    {
        #region Constants
        private const int BaseCountMax = 1;
        private const int FocusExtraPoints = 1;
        #endregion

        private MatchPlayer player;
        private TurnManager turnManager;

        public int BaseAPCount { get; private set; }
        public bool FocusAP => player.Focus.FullyCharged;
        public int Count => BaseAPCount + (FocusAP ? 1 : 0);

        public event Action<int, bool> PointsSpentEvent;
        public event Action PointsUpdatedEvent;

        public ActionPoints(MatchPlayer player, TurnManager turnManager)
        {
            this.player = player;
            this.player.Brain.ActionConfirmedEvent += OnActionConfirmed;

            (this.turnManager = turnManager).TurnEndEvent += OnTurnEnd;

            Recharge();
        }

        ~ActionPoints()
        {
            if (player) player.Brain.ActionConfirmedEvent -= OnActionConfirmed;
            turnManager.TurnEndEvent -= OnTurnEnd;
        }

        private bool Spend(int count, out int spentAP, out bool focusAPSpent)
        {
            spentAP = 0;
            focusAPSpent = false;

            if (count == 0) return false;

            spentAP = Mathf.Min(count, BaseAPCount);
            if (count <= BaseAPCount)
            {
                BaseAPCount -= count;
                return true;
            }

            count -= BaseAPCount;
            BaseAPCount = 0;

            focusAPSpent = true;
            return true;
        }

        private void Recharge()
        {
            BaseAPCount = BaseCountMax;
            PointsUpdatedEvent?.Invoke();
        }

        #region Handlers
        private void OnActionConfirmed(MatchPlayer player, MPAction action)
        {
            if (!Spend(action.APCost, out int spentAP, out bool focusAPSpent)) return;

            PointsSpentEvent?.Invoke(spentAP, focusAPSpent);
            PointsUpdatedEvent?.Invoke();
        }

        private void OnTurnEnd(int endedTurn) => Recharge();
        #endregion
    }
}