namespace Fulbo.Match.UI
{
    using Attributes;

    public abstract class RollPopUp : PopUp
    {
        public virtual void Initialize(RollResult result) => UpdateContent(result);

        protected virtual string NumberToString(int? value) => value.HasValue ? value.Value.ToString() : "?";

        protected virtual string AttributeToString(AttributeTypes attribute) => attribute.ToString().Substring(0, 3).ToUpper();

        protected virtual string RollString(AttributeTypes attribute, int modifier, int? roll) => $"{AttributeToString(attribute)} {modifier} + DIE {NumberToString(roll)}";

        public abstract void UpdateContent(RollResult result);
    }
}