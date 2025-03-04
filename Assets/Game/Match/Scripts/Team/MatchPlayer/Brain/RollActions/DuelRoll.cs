using UnityEngine;

namespace Fulbo.Match
{
    using Attributes;
    using System;
    using UI;
    using UnityEngine.UI;

    public abstract class DuelRoll : RollAction<DuelData, DuelResult, DuelPopUp>
    {
        protected MatchPlayer contender;

        public override int RollDie => 20;
        public abstract AttributeTypes ContenderAttribute { get; }

        public static event Action<DuelRoll, bool> SetDisplayedEvent;

        public DuelRoll(MatchPlayer actor, MatchPlayer contender, bool display = true) : base(actor)
        {
            this.contender = contender;

            Result = new DuelResult();
            Data = new DuelData(
                RollDie,
                ActorAttribute,
                actor.Attributes.GetModifier(ActorAttribute),
                ContenderAttribute,
                contender.Attributes.GetModifier(ContenderAttribute));
            Result.Initialize(Data);

            if (display) SetDisplayed(true);
        }

        public void SetDisplayed(bool display) => SetDisplayedEvent?.Invoke(this, display);

        protected override void OnPopUpSet(DuelPopUp popUp)
        {
            base.OnPopUpSet(popUp);

            Vector3 popUpWorldPosition = (actor.Position + ((actor.AttackedGoal.BottomCenter - actor.Position) * 0.5f)).Horizontal();
            Vector2 popUpScreenPosition = popUp.Camera.WorldToScreenPoint(popUpWorldPosition);
            Vector2 popUpCanvasPosition = UIUtils.ScreenToCanvasPoint(popUpScreenPosition, PopUp.Canvas.GetComponent<CanvasScaler>().referenceResolution);
            popUp.Rect.anchoredPosition = popUpCanvasPosition;
        }

        public override void Roll(DuelData forcedData = null)
        {
            DuelData data = new DuelData()
            { 
                Roll = forcedData?.Roll ?? Die.Roll(RollDie), 
                ContenderRoll = forcedData?.ContenderRoll ?? Die.Roll(RollDie)
            };

            base.Roll(data);
        }

        public static Duel RollDuel<Duel>(Duel duel, DuelData forcedData = null) where Duel : DuelRoll
        {
            duel.Roll(forcedData);
            return duel;
        }
    }
}