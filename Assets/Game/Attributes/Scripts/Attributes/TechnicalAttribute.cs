using System;

namespace Fulbo.Attributes
{
    [Serializable] public abstract class TechnicalAttribute : Attribute { protected TechnicalAttribute(int score) : base(score) { } }

    #region Classes
    [Serializable] public class Dribbling : TechnicalAttribute { public override string DisplayName => "Dribbling"; public Dribbling(int score) : base(score) { } }

    [Serializable] public class Passing : TechnicalAttribute { public override string DisplayName => "Passing"; public Passing(int score) : base(score) { } }

    [Serializable] public class Shooting : TechnicalAttribute { public override string DisplayName => "Shooting"; public Shooting(int score) : base(score) { } }

    [Serializable] public class Tackling : TechnicalAttribute { public override string DisplayName => "Tackling"; public Tackling(int score) : base(score) { } }
    #endregion
}