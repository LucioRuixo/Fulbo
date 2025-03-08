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

            rollsContainer.GetChild(0).GetComponent<TMP_Text>().text = RollString(result.Attribute, result.Modifier, result.Roll);
            total.text = NumberToString(duelResult.Total);

            contenderRollsContainer.GetChild(0).GetComponent<TMP_Text>().text = RollString(duelResult.ContenderAttribute, duelResult.ContenderModifier, duelResult.ContenderRoll);
            contenderTotal.text = NumberToString(duelResult.ContenderTotal);

            OnContentUpdated();
        }
    }
}