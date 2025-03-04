using UnityEngine;

namespace Fulbo
{
    using Attributes;

    public class PlayerAttributes : BasicAttributes
    {
        [SerializeField] private Speed speed;
        [SerializeField] private Strength strength;
        [SerializeField] private Endurance endurance;
        [SerializeField] private Aerial aerial;

        [SerializeField] private Dribbling dribbling;
        [SerializeField] private Shooting shooting;
        [SerializeField] private Passing passing;
        [SerializeField] private Tackling tackling;

        public Speed Speed => speed;
        public Strength Strength => strength;
        public Endurance Endurance => endurance;
        public Aerial Aerial => aerial;

        public Dribbling Dribbling => dribbling;
        public Shooting Shooting => shooting;
        public Passing Passing => passing;
        public Tackling Tackling => tackling;

        public PlayerAttributes(int[] mentalScores, int[] physicalScores, int[] technicalScores) : base(mentalScores)
        {
            speed     = new Speed    ( physicalScores[0]);
            strength  = new Strength ( physicalScores[1]);
            endurance = new Endurance( physicalScores[2]);
            aerial    = new Aerial   ( physicalScores[3]);

            dribbling = new Dribbling(technicalScores[0]);
            shooting  = new Shooting (technicalScores[1]);
            passing   = new Passing  (technicalScores[2]);
            tackling  = new Tackling (technicalScores[3]);
        }

        public override Attribute GetAttribute(AttributeTypes attribute) => attribute switch
        {
            AttributeTypes.Speed => Speed,
            AttributeTypes.Strength => Strength,
            AttributeTypes.Endurance => Endurance,
            AttributeTypes.Aerial => Aerial,

            AttributeTypes.Dribbling => Dribbling,
            AttributeTypes.Shooting => Shooting,
            AttributeTypes.Passing => Passing,
            AttributeTypes.Tackling => Tackling,

            _ => base.GetAttribute(attribute)
        };
    }
}