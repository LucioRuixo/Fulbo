using System;
using System.Linq;
using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    #region Constants
    public struct PlayerID
    {
        public Sides Side { get; private set; }
        public int Index { get; private set; }

        public PlayerID(Sides side, int index)
        {
            Side = side;
            Index = index;
        }

        #region Operators
        public static implicit operator string(PlayerID id) => id.ToString();

        public static bool operator ==(PlayerID l, PlayerID r) => l.Side == r.Side && l.Index == r.Index;

        public static bool operator !=(PlayerID l, PlayerID r) => !(l == r);

        public override string ToString() => $"{Side}, {Index}";

        public override bool Equals(object obj) => obj is PlayerID iD && Side == iD.Side && Index == iD.Index;

        public override int GetHashCode() => HashCode.Combine(Side, Index);
        #endregion
    }
    #endregion

    public class MatchPlayer : MonoBehaviour, ISelectable
    {
        #region Constants
        public const string PrefabResourcesPath = "Player";

        public const int MovementDistance = 1;
        #endregion

        [SerializeField] private BrainMB brain;
        [SerializeField] private MPBody body;
        [SerializeField] private MPHUD hud;

        [Space]

        [SerializeField] private Transform ballReference;

        private Match match;

        // Player Info
        // --------------------
        public int Index { get; private set; } = -1;
        public PlayerID ID { get; private set; }

        public bool IsGK { get; private set; }

        public Vector3 Position { get => transform.position; set => transform.position = value; }

        public Team Team { get; private set; }
        public Sides Side => Team.Side;
        public Halves DefendedHalf => Team.DefendedHalf;
        public Halves AttackedHalf => Team.AttackedHalf;
        public Goal DefendedGoal => Team.DefendedGoal;
        public Goal AttackedGoal => Team.AttackedGoal;
        public Vector3 AttackDirection => Team.AttackDirection;
        public Team Rival => Team.Rival;

        public ActionPoints ActionPoints { get; private set; }
        public Focus Focus { get; private set; }

        public PlayerAttributes Attributes { get; private set; }
        // --------------------

        // References
        // --------------------
        public Square StartSquare { get; private set; }
        public Square CurrentSquare { get; private set; }

        public Brain Brain { get; private set; }

        public Vector3 BallReference => ballReference.position;
        public Pitch Pitch => match.Pitch;
        public Board Board => match.Pitch.Board;

        public MPHUD HUD => hud;

        public HumanPlayer Human { get; private set; }
        // --------------------

        // TESTING
        // --------------------
        private Vector2Int[] startPositions_Home = new Vector2Int[]
        {
            new Vector2Int( 0,  5), 
            new Vector2Int( 8,  3), 
            new Vector2Int( 8,  7), 
            new Vector2Int( 9,  5), 
            new Vector2Int(11,  7), 
            new Vector2Int(12,  3), 
            new Vector2Int(12, 10), 
            new Vector2Int(13,  0), 
            new Vector2Int(14,  8), 
            new Vector2Int(15,  2), 
            new Vector2Int(15,  5), 
        };

        private Vector2Int[] startPositions_Away = new Vector2Int[]
        {
            new Vector2Int(17, 5),
            new Vector2Int(16, 7),
            new Vector2Int(16, 4),
            new Vector2Int(16, 2),
            new Vector2Int(15, 8),
            new Vector2Int(14, 1),
            new Vector2Int(14, 4),
            new Vector2Int(13, 6),
            new Vector2Int(13, 9),
            new Vector2Int(11, 3),
            new Vector2Int(10, 6),
        };
        // --------------------

        public event Action SelectedEvent;
        public event Action UnselectedEvent;
        public event Action ChooseActionEvent;

        private void OnDestroy()
        {
            Board.PlayerMovedToSquareEvent -= OnPlayerMovedToSquare;
            match.Ball.DribblerSetEvent -= OnDribblerSet;
            match.Ball.DribblerClearedEvent -= OnDribblerCleared;
        }

        public void Initialize(int index, Team team, Match match)
        {
            // Player data
            Index = index;
            Team = team;
            IsGK = Index == 0;

            ID = new PlayerID(Side, Index);

            // Match
            this.match = match;
            match.Ball.DribblerSetEvent += OnDribblerSet;
            match.Ball.DribblerClearedEvent += OnDribblerCleared;

            // Attributes
            Attributes = IsGK ?
                new GKAttributes    (new int[] { 10, 10, 10, 10 }, new int[] { 10, 10, 10, 10 }, new int[] { 10, 10, 10, 10 }, new int[] { 10 }) : 
                new PlayerAttributes(new int[] { 10, 10, 10, 10 }, new int[] { 10, 10, 10, 10 }, new int[] { 10, 10, 10, 10 });

            // Squares
            StartSquare = CurrentSquare = Board.Get((Side == Sides.Home ? startPositions_Home : startPositions_Away)[Index]);
            Board.PlayerMovedToSquareEvent += OnPlayerMovedToSquare;

            // Body
            body.Initialize(this);

            // Brain
            // TESTING
            // --------------------
            InitializeBrain();
            SetAIBrain();
            // --------------------

            // Actions
            ActionPoints = new ActionPoints(this, match.TurnManager);
            Focus = new Focus(this, match.TurnManager);

            // HUD
            hud.Initialize(this);
        }

        public Square[] GetAdjacentSquares(bool includeCurrent = false, int distance = 1) => Board.GetAdjacentSquares(CurrentSquare, distance, includeCurrent);

        public Square[] GetValidMovementSquares() => GetAdjacentSquares(false, MovementDistance).Where(square => Board.IsEmpty(square.ID, Side)).ToArray();

        public Square[] GetValidReceptionSquares() => GetAdjacentSquares(true, MovementDistance).Where(square => square == CurrentSquare || Board.IsEmpty(square.ID, Side)).ToArray();

        #region Brain
        private void InitializeBrain() => brain.Initialize(this, match, hud);

        private void UseBrain(Brain brain)
        {
            if (!brain) return;
            if (brain == Brain) return;

            if (Brain)
            {
                Brain.OnUnused();
                Brain.ChooseActionEvent -= OnBrainChooseAction;
            }

            Brain = brain;
            Brain.OnUsed();
            Brain.ChooseActionEvent += OnBrainChooseAction;
        }

        public void SetHumanBrain(HumanPlayer human)
        {
            Human = human;
            UseBrain(brain.HumanBrain);
        }

        public void SetAIBrain()
        {
            Human = null;
            UseBrain(brain.AIBrain);
        }
        #endregion

        #region ISelectable
        public void OnSelected()
        {
            if (MatchMenu.BlockingPointer) return;

            SelectedEvent?.Invoke();
        }

        public void OnUnselected() => UnselectedEvent?.Invoke();
        #endregion

        #region Handlers
        private void OnDribblerSet(MatchPlayer dribbler)
        {
            if (dribbler != this) return;

            body.Animator.PlayPose(MPAnimator.Poses.Run);
        }

        private void OnDribblerCleared(MatchPlayer previousDribbler)
        {
            if (previousDribbler != this) return;

            body.Animator.PlayPose(MPAnimator.Poses.Idle);
        }

        private void OnPlayerMovedToSquare(Square square, MatchPlayer player)
        {
            if (player != this) return;

            CurrentSquare = square;
            if (match.Ball.Square == CurrentSquare && !match.Ball.Dribbler) match.Ball.SetDribbler(player);
        }

        private void OnBrainChooseAction() => ChooseActionEvent?.Invoke();
        #endregion
    }
}