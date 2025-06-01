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

            SetRollsText(result.Attribute, result.Modifier, result.Roll, rollsContainer);
            total.text = NumberToString(skillCheckResult.Total);

            required.text = skillCheckResult.Required.ToString();
            difficulty.text = skillCheckResult.Difficulty.Value.ToFormattedString();

            OnContentUpdated();
        }
    }
}