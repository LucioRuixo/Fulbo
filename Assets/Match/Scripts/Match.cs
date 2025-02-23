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
        [SerializeField] private Player player;
        [SerializeField] private Pitch pitch;

        [Space]

        [SerializeField] private Team home;
        [SerializeField] private Team away;
        [SerializeField] private Ball ball;

        public Player Player => player;
        public TurnManager TurnManager { get; private set; }

        public Pitch Pitch => pitch;
        public Team Home => home;
        public Team Away => away;
        public Ball Ball => ball;

        public bool OnPlay { get; private set; } = false;

        public MatchInfo Info { get; private set; }

        public event Action<MatchPlayer> InitialPlayerSetEvent;
        public event Action PlayStartEvent;
        public event Action PlayEndEvent;

        public event Action MatchStartEvent;
        public event Action<Sides> GoalEvent;
        public event Action MatchEndEvent;

        private void Awake()
        {
            Info = new MatchInfo(this);

            TurnManager = GetComponent<TurnManager>();

            pitch.Initialize(this);
            InitializeTeams();
            ball.Initialize(this);

            TurnManager.ShotEvent += OnShot;
        }

        private void Start()
        {
            MatchStartEvent?.Invoke();
            ResetPlay();
        }

        private void OnDestroy()
        {
            MatchEndEvent?.Invoke();

            TurnManager.ShotEvent -= OnShot;
        }

        private void InitializeTeams()
        {
            GameObject playerPrefab = Resources.Load<GameObject>(MatchPlayer.PrefabResourcesPath);

            home.Initialize(Sides.Home, this, playerPrefab);
            away.Initialize(Sides.Away, this, playerPrefab);
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

        #region Queries
        public Halves GetDefendedHalfBySide(Sides side) => side == Sides.Home ? Halves.Left : Halves.Right;
        public Halves GetAttackedHalfBySide(Sides side) => side == Sides.Home ? Halves.Right : Halves.Left;

        public Goal GetDefendedGoalBySide(Sides side) => side == Sides.Home ? Pitch.LeftGoal : Pitch.RightGoal;
        public Goal GetAttackedGoalBySide(Sides side) => side == Sides.Home ? Pitch.RightGoal : Pitch.LeftGoal;

        public Team GetTeam(Sides side) => side == Sides.None ? null : side == Sides.Home ? Home : Away;
        public Team GetRival(Sides side) => side == Sides.None ? null : side == Sides.Home ? Away : Home;
        #endregion

        #region Handlers
        private void OnShot(MatchPlayer kicker)
        {
            GoalEvent?.Invoke(kicker.Side);
            ResetPlay();
        }
        #endregion
    }
}