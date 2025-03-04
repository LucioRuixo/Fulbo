namespace Fulbo.Match
{
    using Attributes;

    public abstract class RollAction<RollData, RollResult, RollPopUp> : RollActionBase
        where RollData : Fulbo.Match.RollData, new() 
        where RollResult : Fulbo.Match.RollResult, new() 
        where RollPopUp : UI.RollPopUp
    {
        public abstract AttributeTypes ActorAttribute { get; }

        public RollData Data { get; protected set; }
        public RollResult Result { get; protected set; }

        public RollPopUp PopUp { get; protected set; }

        public RollAction(MatchPlayer actor) : base(actor) { }

        public virtual void Roll(RollData forcedData = null)
        {
            RollData data = forcedData ?? new RollData() { Roll = Die.Roll(Data.Die) };
            Result.SetRoll(data);
        }

        public override void SetPopUp(UI.RollPopUp popUp)
        {
            base.SetPopUp(popUp);

            PopUp = popUp as RollPopUp;
            PopUp.Initialize(Result);

            OnPopUpSet(PopUp);
        }

        protected virtual void OnPopUpSet(RollPopUp popUp) { }
    }
}