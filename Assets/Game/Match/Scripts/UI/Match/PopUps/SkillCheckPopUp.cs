using TMPro;
using UnityEngine;

namespace Fulbo.Match.UI
{
    public class SkillCheckPopUp : RollPopUp
    {
        [SerializeField] private RectTransform rollsContainer;
        [SerializeField] private TMP_Text total;

        [Space]

        [SerializeField] private TMP_Text required;
        [SerializeField] private TMP_Text difficulty;

        public override void UpdateContent(RollResult result)
        {
            SkillCheckResult skillCheckResult = result as SkillCheckResult;

            rollsContainer.GetChild(0).GetComponent<TMP_Text>().text = RollString(result.Attribute, result.Modifier, result.Roll);
            total.text = NumberToString(skillCheckResult.Total);

            required.text = skillCheckResult.Required.ToString();
            difficulty.text = skillCheckResult.Difficulty.ToString();

            OnContentUpdated();
        }
    }
}