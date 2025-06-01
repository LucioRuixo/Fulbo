using UnityEngine;

namespace Fulbo
{
    using Attributes;
    using System.Linq;

    public class BasicAttributes
    {
        [SerializeField] private Composture composture;
        [SerializeField] private Determination determination;
        [SerializeField] private Solidarity solidarity;
        [SerializeField] private Charisma charisma;

        public Composture Composture => composture;
        public Determination Determination => determination;
        public Solidarity Solidarity => solidarity;
        public Charisma Charisma => charisma;

        public BasicAttributes(int[] scores)
        {
            composture    = new Composture   (scores[0]);
            determination = new Determination(scores[1]);
            solidarity    = new Solidarity   (scores[2]);
            charisma      = new Charisma     (scores[3]);
        }

        public virtual Attribute GetAttribute(AttributeTypes attribute) => attribute switch
        {
            AttributeTypes.Composture => Composture,
            AttributeTypes.Determination => Determination,
            AttributeTypes.Solidarity => Solidarity,
            AttributeTypes.Charisma => Charisma,

            _ => null
        };

        public int GetScore(AttributeTypes attribute) => GetAttribute(attribute).Score;

        public int[] GetModifier(AttributeTypes attribute) => attribute.GetFlags().Cast<AttributeTypes>().Select(attribute => GetAttribute(attribute).Modifier).ToArray();
    }
}