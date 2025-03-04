using System;

namespace Fulbo.Attributes
{
    [Serializable] public abstract class MentalAttribute : Attribute { protected MentalAttribute(int score) : base(score) { } }

    #region Classes
    [Serializable] public class Composture : MentalAttribute { public override string DisplayName => "Speed"; public Composture(int score) : base(score) { } }

    [Serializable] public class Determination : MentalAttribute { public override string DisplayName => "Strength"; public Determination(int score) : base(score) { } }

    [Serializable] public class Solidarity : MentalAttribute { public override string DisplayName => "Endurance"; public Solidarity(int score) : base(score) { } }

    [Serializable] public class Charisma : MentalAttribute { public override string DisplayName => "Aerial"; public Charisma(int score) : base(score) { } }
    #endregion
}