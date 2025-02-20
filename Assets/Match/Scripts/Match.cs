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

        public TurnManager TurnManager { get; private set; }
        public Pitch Pitch => pitch;
        public Team Home => home;
        public Team Away => away;

        public event Action MatchStartEvent;

        private void Awake()
        {
            TurnManager = GetComponent<TurnManager>();

            pitch.Initialize(this);
            InitializeTeams();
        }

        private void Start()
        {
            MatchStartEvent?.Invoke();

            TurnManager.Play(player.SelectedPlayer);
        }

        private void InitializeTeams()
        {
            GameObject playerPrefab = Resources.Load<GameObject>(MatchPlayer.PrefabResourcesPath);

            home.Initialize(Sides.Home, this, playerPrefab);
            away.Initialize(Sides.Away, this, playerPrefab);
        }

        public Halves GetDefendedHalfBySide(Sides side) => side == Sides.Home ? Halves.Left : Halves.Right;
        public Halves GetAttackedHalfBySide(Sides side) => side == Sides.Home ? Halves.Right : Halves.Left;

        public Goal GetDefendedGoalBySide(Sides side) => side == Sides.Home ? Pitch.LeftGoal : Pitch.RightGoal;
        public Goal GetAttackedGoalBySide(Sides side) => side == Sides.Home ? Pitch.RightGoal : Pitch.LeftGoal;

        public Team GetTeam(Sides side) => side == Sides.None ? null : side == Sides.Home ? Home : Away;
        public Team GetRival(Sides side) => side == Sides.None ? null : side == Sides.Home ? Away : Home;
    }
}