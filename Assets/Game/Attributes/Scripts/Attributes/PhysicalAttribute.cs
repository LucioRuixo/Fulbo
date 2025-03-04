using System;

namespace Fulbo.Attributes
{
    [Serializable] public abstract class PhysicalAttribute : Attribute { protected PhysicalAttribute(int score) : base(score) { } }

    #region Classes
    [Serializable] public class Speed : PhysicalAttribute { public override string DisplayName => "Speed"; public Speed(int score) : base(score) { } }

    [Serializable] public class Strength : PhysicalAttribute { public override string DisplayName => "Strength"; public Strength(int score) : base(score) { } }

    [Serializable] public class Endurance : PhysicalAttribute { public override string DisplayName => "Endurance"; public Endurance(int score) : base(score) { } }

    [Serializable] public class Aerial : PhysicalAttribute { public override string DisplayName => "Aerial"; public Aerial(int score) : base(score) { } }
    #endregion
}