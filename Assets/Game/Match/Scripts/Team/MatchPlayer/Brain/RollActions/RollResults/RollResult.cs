namespace Fulbo.Match
{
    using Attributes;

    #region Classes
    public class RollData
    {
        public int Die { get; private set; }
        public int? Required { get; private set; }
        public int? Roll { get; set; }
        public int Modifier { get; private set; }
        public AttributeTypes Attribute { get; private set; }

        public RollData() { }

        public RollData(int die, AttributeTypes attribute, int actorModifier, int? required = null, int? actorRoll = null)
        {
            Die = die;
            Required = required;
            Roll = actorRoll;
            Modifier = actorModifier;
            Attribute = attribute;
        }
    }
    #endregion

    public class RollResult
    {
        public int Die { get; protected set; }
        public int? Required { get; protected set; }

        public int? Roll { get; protected set; }
        public int Modifier { get; protected set; }
        public int? Total => Roll + Modifier;

        public AttributeTypes Attribute { get; protected set; }

        public bool Failed => !Succeeded;
        public virtual bool Succeeded => Total >= Required;

        public bool Crit => Roll == Die;
        public bool Miss => Roll == 1;

        public virtual void Initialize(RollData data)
        {
            Die = data.Die;
            Required = data.Required;

            Roll = data.Roll;
            Modifier = data.Modifier;

            Attribute = data.Attribute;
        }

        public virtual void SetRoll(RollData data) => Roll = data.Roll;
    }
}