using System;

namespace Fulbo.Attributes
{
    public abstract class GKAttribute : Attribute { protected GKAttribute(int score) : base(score) { } }

    #region Classes
    [Serializable] public class Saving : GKAttribute { public override string DisplayName => "Saving"; public Saving(int score) : base(score) { } }
    #endregion
}