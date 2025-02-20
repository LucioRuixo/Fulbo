using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    #region Enumerators
    public enum MPActions
    {
        None = -1,
        Move
    }
    #endregion

    public abstract class MPAction : MonoBehaviour
    {
        protected MatchPlayer player;
        protected Squares squares;
        protected MPHUD hud;

        public void Initialize(MatchPlayer player, Squares squares, MPHUD hud)
        {
            this.player = player;
            this.squares = squares;
            this.hud = hud;
        }

        public virtual bool Feed(ISelectable selection) => true;

        public virtual void OnChosen() { }

        public virtual void OnUnchosen() { }

        public virtual void OnExecuted() { }
    }
}