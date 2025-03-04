namespace Fulbo.Match
{
    using UI;

    public abstract class RollActionBase
    {
        protected MatchPlayer actor;

        public RollPopUp BasePopUp { get; private set; }

        public abstract int RollDie { get; }

        public RollActionBase(MatchPlayer actor) => this.actor = actor;

        public virtual void SetPopUp(RollPopUp popUp) => BasePopUp = popUp;
    }
}