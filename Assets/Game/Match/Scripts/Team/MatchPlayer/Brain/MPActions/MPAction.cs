using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    #region Enumerators
    public enum MPActions
    {
        None = -1,
        Move, 
        Pass, 
        Shoot
    }
    #endregion

    public abstract class MPAction : MonoBehaviour
    {
        protected MatchPlayer player;
        protected Board board;
        protected MPHUD hud;

        public abstract bool RequiresFeed { get; }

        public void Initialize(MatchPlayer player, Board board, MPHUD hud)
        {
            this.player = player;
            this.board = board;
            this.hud = hud;
        }

        public virtual void OnChosen(bool completeUI) { }

        public virtual void OnUnchosen() { }

        public virtual bool Feed(ISelectable selection) => true;

        public virtual void OnConfirmed() { }

        public virtual void Execute() { }

        public virtual void OnExit() { }
    }
}