using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    public class BrainMB : MonoBehaviour
    {
        [SerializeField] private Transform actions;

        private MatchPlayer player;

        public HumanBrain HumanBrain { get; private set; }
        public AIBrain AIBrain { get; private set; }

        public void Initialize(MatchPlayer player, Board board, MPHUD hud)
        {
            this.player = player;

            HumanBrain = new HumanBrain(actions, player, player.Pitch.Board, hud);
            AIBrain = new AIBrain(actions, player, player.Pitch.Board, hud);

            foreach (MPAction action in actions.GetComponents<MPAction>()) action.Initialize(player, board, hud);
        }
    }
}