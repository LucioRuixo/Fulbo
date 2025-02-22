using System;
using UnityEngine;

namespace Fulbo.Match
{
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

        public event Action<MatchPlayer> InitialPlayerSetEvent;
        public event Action MatchStartEvent;
        public event Action MatchEndEvent;

        private void Awake()
        {
            TurnManager = GetComponent<TurnManager>();

            pitch.Initialize(this);
            InitializeTeams();
            ball.Initialize(this);
        }

        private void Start()
        {
            InitialPlayerSetEvent?.Invoke(Home.GetPlayers()[4]);
            MatchStartEvent?.Invoke();
        }

        private void OnDestroy() => MatchEndEvent?.Invoke();

        private void InitializeTeams()
        {
            GameObject playerPrefab = Resources.Load<GameObject>(MatchPlayer.PrefabResourcesPath);

            home.Initialize(Sides.Home, this, playerPrefab);
            away.Initialize(Sides.Away, this, playerPrefab);
        }

        #region Queries
        public Halves GetDefendedHalfBySide(Sides side) => side == Sides.Home ? Halves.Left : Halves.Right;
        public Halves GetAttackedHalfBySide(Sides side) => side == Sides.Home ? Halves.Right : Halves.Left;

        public Goal GetDefendedGoalBySide(Sides side) => side == Sides.Home ? Pitch.LeftGoal : Pitch.RightGoal;
        public Goal GetAttackedGoalBySide(Sides side) => side == Sides.Home ? Pitch.RightGoal : Pitch.LeftGoal;

        public Team GetTeam(Sides side) => side == Sides.None ? null : side == Sides.Home ? Home : Away;
        public Team GetRival(Sides side) => side == Sides.None ? null : side == Sides.Home ? Away : Home;
        #endregion
    }
}