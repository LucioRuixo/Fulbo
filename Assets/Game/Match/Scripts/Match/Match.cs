using System;
using System.Linq;
using UnityEngine;

namespace Fulbo.Match
{
    using Settings;

    #region Classes
    public class MatchInfo
    {
        private Match match;

        public uint HomeScore { get; private set; }
        public uint AwayScore { get; private set; }

        public event Action<Sides, uint> ScoreUpdateEvent;

        public MatchInfo(Match match)
        {
            this.match = match;
            match.GoalEvent += OnGoal;

            Reset();
        }

        public void Reset() => HomeScore = AwayScore = 0;

        #region Handlers
        private void OnGoal(Sides side)
        {
            if (side == Sides.Home) ScoreUpdateEvent?.Invoke(side, ++HomeScore);
            else if (side == Sides.Away) ScoreUpdateEvent?.Invoke(side, ++AwayScore);
        }
        #endregion
    }
    #endregion

    public class Match : MonoBehaviour
    {
        [SerializeField] private HumanPlayer human;
        [SerializeField] private Pitch pitch;

        [Space]

        [SerializeField] private Team home;
        [SerializeField] private Team away;
        [SerializeField] private Ball ball;

        public HumanPlayer Human => human;
        public TurnManager TurnManager { get; private set; }

        public Pitch Pitch => pitch;
        public Team Home => home;
        public Team Away => away;

        public Ball Ball => ball;
        public MatchPlayer Dribbler { get; private set; }
        public MatchPlayer[] AllPlayers { get; private set; }

        public bool OnPlay { get; private set; } = false;

        public MatchInfo Info { get; private set; }

        public event Action<MatchPlayer> InitialPlayerSetEvent;

        // Play Events
        // --------------------
        public event Action PlayStartEvent;
        public event Action<MatchPlayer, MatchPlayer, Square> PassMissedEvent;
        public event Action<MatchPlayer, MatchPlayer, Square, RollResult> PassEvent;
        public event Action<MatchPlayer, RollResult> ShotAttemptEvent;
        public event Action<MatchPlayer> ShotMissedEvent;
        public event Action<MatchPlayer> ShotSavedEvent;
        public event Action<MatchPlayer, DuelResult> ShotEvent;
        public event Action PlayEndCalledEvent;
        public event Action PlayEndEvent;
        // --------------------

        // Match Events
        // --------------------
        public event Action MatchStartEvent;
        public event Action<Sides> GoalEvent;
        public event Action MatchEndEvent;
        // --------------------

        private void Awake()
        {
            Info = new MatchInfo(this);

            TurnManager = GetComponent<TurnManager>();

            pitch.Initialize(this);
            InitializeTeams();
            ball.Initialize(this);

            ball.DribblerSetEvent += OnDribblerSet;
            SubscribeToPlayerActionEvents();
        }

        private void Start() => StartMatch();

        private void OnDestroy()
        {
            EndMatch();

            ball.DribblerSetEvent -= OnDribblerSet;
            UnsubscribeFromPlayerActionEvents();
        }

        #region Initialization
        private void InitializeTeams()
        {
            GameObject playerPrefab = Resources.Load<GameObject>(MatchPlayer.PrefabResourcesPath);

            home.Initialize(Sides.Home, this, playerPrefab);
            away.Initialize(Sides.Away, this, playerPrefab);

            AllPlayers = Home.Players.Concat(Away.Players).ToArray();
        }

        private void SubscribeToPlayerActionEvents()
        {
            MPA_Pass.PassAttemptEvent += OnPassAttempt;
            MPA_Shoot.ShotAttemptEvent += OnShotAttempt;
            MPA_Shoot.ShotEvent += OnShot;
        }

        private void UnsubscribeFromPlayerActionEvents()
        {
            MPA_Pass.PassAttemptEvent -= OnPassAttempt;
            MPA_Shoot.ShotAttemptEvent -= OnShotAttempt;
            MPA_Shoot.ShotEvent -= OnShot;
        }
        #endregion

        #region Match
        private void StartMatch()
        {
            MatchStartEvent?.Invoke();
            ResetPlay();
        }

        private void ResetPlay()
        {
            if (OnPlay)
            {
                OnPlay = false;
                PlayEndEvent?.Invoke();
            }

            Home.ResetPlayers();
            Away.ResetPlayers();

            MatchPlayer initialPlayer = Home.GetPlayers()[4];
            SetDribbler(initialPlayer);
            InitialPlayerSetEvent?.Invoke(initialPlayer);

            OnPlay = true;
            PlayStartEvent?.Invoke();
        }

        private void EndMatch() => MatchEndEvent?.Invoke();
        #endregion

        #region Ball
        private void SetDribbler(MatchPlayer dribbler)
        {
            Dribbler = dribbler;
            Ball.SetDribbler(Dribbler);
        }
        #endregion

        #region Queries
        public Halves GetDefendedHalfBySide(Sides side) => side == Sides.Home ? Halves.Left : Halves.Right;
        public Halves GetAttackedHalfBySide(Sides side) => side == Sides.Home ? Halves.Right : Halves.Left;

        public Goal GetDefendedGoalBySide(Sides side) => side == Sides.Home ? Pitch.LeftGoal : Pitch.RightGoal;
        public Goal GetAttackedGoalBySide(Sides side) => side == Sides.Home ? Pitch.RightGoal : Pitch.LeftGoal;

        public Team GetTeam(Sides side) => side == Sides.None ? null : side == Sides.Home ? Home : Away;
        public Team GetRival(Sides side) => side == Sides.None ? null : side == Sides.Home ? Away : Home;
        #endregion

        
        #region Handlers
        private void OnDribblerSet(MatchPlayer dribbler)
        {
            if (dribbler.Side == Human.Side) return;

            CallPlayEnd();
        }

        private void OnPassAttempt(MatchPlayer passer, MatchPlayer receiver, Square receptionSquare, RollResult result)
        {
            Ball.ClearDribbler();

            if (result.Failed)
            {
                bool passLandedInsidePitch = Pitch.Board.TryGetRandomAdjacentSquare(receptionSquare, out Square landingSquare, out Vector2 landingPosition, MatchSettings.MissedPassLandingDistance, false, false);

                if (passLandedInsidePitch)
                {
                    if (landingSquare == receiver.CurrentSquare) SetDribbler(receiver);
                    else Ball.SetSquare(landingSquare);
                }
                else Ball.SetLoosePosition(landingPosition);

                PassMissedEvent?.Invoke(passer, receiver, receptionSquare);
                if (!passLandedInsidePitch) CallPlayEnd();
            }
            else SetDribbler(receiver);
        }

        private void OnShotAttempt(MatchPlayer kicker, RollResult result)
        {
            Ball.ClearDribbler();

            ShotAttemptEvent?.Invoke(kicker, result);

            if (result.Failed)
            {
                ShotMissedEvent.Invoke(kicker);
                CallPlayEnd();
            }
        }

        private void OnShot(MatchPlayer kicker, RollResult result)
        {
            if (result.Failed) ShotSavedEvent.Invoke(kicker);
            else GoalEvent?.Invoke(kicker.Side);

            CallPlayEnd();
        }

        private void CallPlayEnd()
        {
            TurnManager.PlayEndEvent += ResetPlayOnEnd;
            PlayEndCalledEvent?.Invoke();
        }

        private void ResetPlayOnEnd()
        {
            TurnManager.PlayEndEvent -= ResetPlayOnEnd;
            ResetPlay();
        }
        #endregion
    }
}