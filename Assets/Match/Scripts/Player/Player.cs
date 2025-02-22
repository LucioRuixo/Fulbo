using System;
using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    [RequireComponent(typeof(Input))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private MatchUIManager uiManager;

        [Space]

        [SerializeField] private Match match;

        private Input input;

        private Ball Ball => match.Ball;

        public MatchPlayer SelectedPlayer { get; private set; }

        public Sides Side => Sides.Home;
        public Vector3 Position => mainCamera.transform.position;
        public Camera View => mainCamera;

        public event Action<MatchPlayer> PlayerSelectedEvent;

        public event Action<MPActions> ActionChosenEvent;
        public event Action<MPActions> ActionCanceledEvent;
        public event Action<MPActions> ActionConfirmedEvent;

        public event Action<ISelectable> ScreenSelectionEvent;

        private void Awake()
        {
            InitializeInput();
            Ball.DribblerSetEvent += OnDribblerSet;
        }

        private void OnDestroy()
        {
            input.SelectedEvent -= OnInputSelection;
            Ball.DribblerSetEvent -= OnDribblerSet;
        }

        private void Update() => input.Update();

        private void InitializeInput()
        {
            input = new Input(this);
            input.SelectedEvent += OnInputSelection;
        }

        private void SelectPlayer(MatchPlayer player)
        {
            if (player == null) return;

            if (SelectedPlayer)
            {
                UnsubscribeFromPlayerEvents();
                SelectedPlayer.OnUnselected();
                SelectedPlayer.SetAIBrain();
            }

            SelectedPlayer = player;
            SelectedPlayer.OnSelected();
            SelectedPlayer.SetHumanBrain(this);
            SubscribeToPlayerEvents();
        }

        private void SubscribeToPlayerEvents()
        {
            if (!SelectedPlayer) return;

            SelectedPlayer.ChooseActionEvent += OnPlayerChooseAction;
        }

        private void UnsubscribeFromPlayerEvents()
        {
            if (!SelectedPlayer) return;

            SelectedPlayer.ChooseActionEvent -= OnPlayerChooseAction;
        }

        #region Handlers
        private void OnDribblerSet(MatchPlayer dribbler) => SelectPlayer(dribbler);

        private void OnPlayerChooseAction()
        {
            uiManager.ActionMenu.Enable();

            uiManager.ActionMenu.ActionChosenEvent  += OnUIActionChosen;
            uiManager.ActionMenu.ActionCanceledEvent  += OnUIActionCanceled;
            uiManager.ActionMenu.ActionConfirmedEvent += OnUIActionConfirmed;
        }

        private void OnUIActionChosen(MPActions action)
        {
            ActionChosenEvent?.Invoke(action);
            uiManager.ActionMenu.SetConfirmButtonEnabled(!SelectedPlayer.Brain.GetActionByType(action).RequiresFeed);
        }

        private void OnUIActionCanceled(MPActions action) => ActionCanceledEvent?.Invoke(action);

        private void OnUIActionConfirmed(MPActions action)
        {
            uiManager.ActionMenu.ActionChosenEvent  -= OnUIActionChosen;
            uiManager.ActionMenu.ActionCanceledEvent  -= OnUIActionCanceled;
            uiManager.ActionMenu.ActionConfirmedEvent -= OnUIActionConfirmed;

            uiManager.ActionMenu.Disable();

            ActionConfirmedEvent?.Invoke(action);
        }

        private void OnInputSelection(ISelectable selected)
        {
            SelectedPlayer.Brain.FeedResultEvent += OnActionFeedResult;
            ScreenSelectionEvent?.Invoke(selected);
        }

        private void OnActionFeedResult(bool result)
        {
            SelectedPlayer.Brain.FeedResultEvent -= OnActionFeedResult;
            uiManager.ActionMenu.SetConfirmButtonEnabled(result);
        }
        #endregion
    }
}