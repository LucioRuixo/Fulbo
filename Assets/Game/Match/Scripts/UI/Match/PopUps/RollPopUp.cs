using System.Linq;
using UnityEngine;

namespace Fulbo.Match.UI
{
    using Attributes;
    using TMPro;

    public abstract class RollPopUp : PopUp
    {
        public virtual void Initialize(RollResult result) => UpdateContent(result);

        protected virtual string NumberToString(int? value) => value.HasValue ? value.Value.ToString() : "?";

        protected virtual string AttributeToString(AttributeTypes attribute) => attribute.ToString().Abbreviate();

        protected virtual string RollString(AttributeTypes attribute, int modifier, int? roll) => $"{AttributeToString(attribute)} {modifier} + DIE {NumberToString(roll)}";

        protected void SetRollsText(AttributeTypes attribute, int[] modifier, int? roll, RectTransform container)
        {
            container.GetChild(0).GetComponent<TMP_Text>().text = RollString(attribute.GetFlags().ToArray()[0], modifier[0], roll);

            int rollTextCount = container.childCount;
            while (container.childCount < modifier.Length)
            {
                Instantiate(container.GetChild(0).gameObject, container).
                GetComponent<TMP_Text>().text = RollString(attribute.GetFlags().ToArray()[rollTextCount], modifier[rollTextCount], roll);
                rollTextCount++;
            }

            for (int i = 1; i < container.childCount; i++) container.GetChild(i).GetComponent<TMP_Text>().text = RollString(attribute.GetFlags().ToArray()[i], modifier[i], roll);
        }

        public abstract void UpdateContent(RollResult result);
    }
}