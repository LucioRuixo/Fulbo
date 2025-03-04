using UnityEngine;

namespace Fulbo
{
    using Attributes;

    public class GKAttributes : PlayerAttributes
    {
        [SerializeField] private Saving saving;

        public Saving Saving => saving;

        public GKAttributes(int[] mentalScores, int[] physicalScores, int[] technicalScores, int[] gkScores) : base(mentalScores, physicalScores, technicalScores) => saving = new Saving(gkScores[0]);

        public override Attribute GetAttribute(AttributeTypes attribute) => attribute switch
        {
            AttributeTypes.Saving => Saving,
            _ => base.GetAttribute(attribute)
        };
    }
}