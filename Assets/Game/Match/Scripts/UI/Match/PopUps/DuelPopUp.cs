using TMPro;
using UnityEngine;

namespace Fulbo.Match.UI
{
    public class DuelPopUp : RollPopUp
    {
        [SerializeField] private RectTransform rollsContainer;
        [SerializeField] private TMP_Text total;

        [Space]

        [SerializeField] private TMP_Text contenderTotal;
        [SerializeField] private RectTransform contenderRollsContainer;

        protected virtual string ContenderRollString(DuelResult result) => $"{AttributeToString(result.ContenderAttribute)} {result.ContenderModifier} + DIE {NumberToString(result.ContenderRoll)}";

        public override void UpdateContent(RollResult result)
        {
            DuelResult duelResult = result as DuelResult;

            SetRollsText(duelResult.Attribute, duelResult.Modifier, duelResult.Roll, rollsContainer);
            total.text = NumberToString(duelResult.Total);

            SetRollsText(duelResult.ContenderAttribute, duelResult.ContenderModifier, duelResult.ContenderRoll, contenderRollsContainer);
            contenderTotal.text = NumberToString(duelResult.ContenderTotal);

            OnContentUpdated();
        }
    }
}