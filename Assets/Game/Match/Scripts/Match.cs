using System;
using UnityEngine;

namespace Fulbo.Match
{
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

        public bool OnPlay { get; private set; } = false;

        public MatchInfo Info { get; private set; }

        public event Action<MatchPlayer> InitialPlayerSetEvent;

        // Play Events
        // --------------------
        public event Action PlayStartEvent;
        public event Action<MatchPlayer, RollResult> PassEvent;
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
        }

        private void Start() => StartMatch();

        private void OnDestroy() => EndMatch();

        private void InitializeTeams()
        {
            GameObject playerPrefab = Resources.Load<GameObject>(MatchPlayer.PrefabResourcesPath);

            home.Initialize(Sides.Home, this, playerPrefab);
            away.Initialize(Sides.Away, this, playerPrefab);
        }

        private void StartMatch()
        {
            Ball.DribblerSetEvent += OnDribblerSet;
            Ball.DribblerClearedEvent += OnDribblerCleared;

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

            InitialPlayerSetEvent?.Invoke(Home.GetPlayers()[4]);

            OnPlay = true;
            PlayStartEvent?.Invoke();
        }

        private void EndMatch()
        {
            MatchEndEvent?.Invoke();

            Ball.DribblerSetEvent -= OnDribblerSet;
            Ball.DribblerClearedEvent -= OnDribblerCleared;
        }

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
            if (!dribbler) return;

            if (Dribbler) OnDribblerCleared();

            Dribbler = dribbler;
            Dribbler.Brain.GetAction<MPA_Pass>().PassEvent += OnPass;
            Dribbler.Brain.GetAction<MPA_Shoot>().ShotAttemptEvent += OnShotAttempt;
            Dribbler.Brain.GetAction<MPA_Shoot>().ShotEvent += OnShot;
        }

        private void OnDribblerCleared()
        {
            if (!Dribbler) return;

            Dribbler.Brain.GetAction<MPA_Pass>().PassEvent -= OnPass;
            Dribbler.Brain.GetAction<MPA_Shoot>().ShotAttemptEvent -= OnShotAttempt;
            Dribbler.Brain.GetAction<MPA_Shoot>().ShotEvent -= OnShot;
            Dribbler = null;
        }

        private void OnPass(MatchPlayer passer, MatchPlayer receiver, RollResult result) => PassEvent?.Invoke(receiver, result);

        private void OnShotAttempt(MatchPlayer kicker, RollResult result)
        {
            ShotAttemptEvent?.Invoke(kicker, result);
            if (result.Failed) OnShotMissed(kicker);
        }

        private void OnShotMissed(MatchPlayer kicker)
        {
            ShotMissedEvent.Invoke(kicker);
            CallPlayEnd();
        }

        private void OnShot(MatchPlayer kicker, RollResult result)
        {
            if (result.Failed) OnShotSaved(kicker);
            else OnGoal(kicker);
        }

        private void OnShotSaved(MatchPlayer kicker)
        {
            ShotSavedEvent.Invoke(kicker);
            CallPlayEnd();
        }

        private void OnGoal(MatchPlayer scorer)
        {
            GoalEvent?.Invoke(scorer.Side);
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