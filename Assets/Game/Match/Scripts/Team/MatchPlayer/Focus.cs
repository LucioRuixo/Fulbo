using System;

namespace Fulbo.Match
{
    public class Focus
    {
        #region Constants
        public const int MaxCharge = 5;
        #endregion

        private MatchPlayer player;
        private TurnManager turnManager;

        public int Charge { get; private set; } = 0;
        public bool FullyCharged => Charge == MaxCharge;

        public event Action<int> ChargeEvent;
        public event Action FullyChargeEvent;
        public event Action EmptiedEvent;

        public Focus(MatchPlayer player, TurnManager turnManager)
        {
            (this.player = player).ActionPoints.PointsSpentEvent += OnAPSpent;

            this.turnManager = turnManager;
            this.turnManager.TurnEndEvent += OnTurnEndEvent;
            this.turnManager.PlayEndEvent += OnPlayEnd;
        }

        ~Focus()
        {
            player.ActionPoints.PointsSpentEvent -= OnAPSpent;
            turnManager.TurnEndEvent -= OnTurnEndEvent;
            turnManager.PlayEndEvent -= OnPlayEnd;
        }

        private void ChargePoint()
        {
            if (FullyCharged) return;

            Charge++;
            ChargeEvent?.Invoke(Charge);

            if (FullyCharged) FullyChargeEvent?.Invoke();
        }

        private void Empty()
        {
            Charge = 0;
            EmptiedEvent?.Invoke();
        }

        #region Handlers
        private void OnAPSpent(int spentAP, bool focusAPSpent)
        {
            if (focusAPSpent) Empty();
        }

        private void OnTurnEndEvent(int endedTurn) => ChargePoint();

        private void OnPlayEnd() => Empty();
        #endregion
    }
}