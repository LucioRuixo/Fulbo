using System.Linq;

namespace Fulbo.Match
{
    using Attributes;

    #region Classes
    public class DuelData : RollData
    {
        public int? ContenderRoll { get; set; }
        public int[] ContenderModifier { get; private set; }
        public AttributeTypes ContenderAttribute { get; private set; }

        public DuelData() : base() { }

        public DuelData(int die, AttributeTypes actorAttribute, int[] actorModifier, AttributeTypes contenterAttribute, int[] contenderModifier, int? actorRoll = null, int? contenderRoll = null) :
            base(die, actorAttribute, actorModifier, contenderRoll + contenderModifier.Sum(), actorRoll)
        {
            ContenderRoll = contenderRoll;
            ContenderModifier = contenderModifier;
            ContenderAttribute = contenterAttribute;
        }
    }
    #endregion

    public class DuelResult : RollResult
    {
        public int? ContenderRoll { get; private set; }
        public int[] ContenderModifier { get; private set; }
        public int? ContenderTotal => ContenderRoll + ContenderModifier.Sum();

        public AttributeTypes ContenderAttribute { get; private set; }

        public bool ContenderCrit => ContenderRoll == Die;
        public bool ContenderMiss => ContenderRoll == 1;

        public override bool Succeeded
        {
            get
            {
                if (Crit) return true; // If player rolled a crit, suceed
                if (ContenderMiss && !Miss) return true; // If contender missed and player didn't, suceed

                if (base.Succeeded)
                {
                    if (Miss) return ContenderMiss; // If player missed BUT rolled higher/equal and contender missed too, suceed
                    else return true; // If rolled highr/equal, suceed
                }

                return false; // If non of the above, fail
            }
        }

        public override void Initialize(RollData data)
        {
            DuelData duelData = data as DuelData;

            ContenderRoll = duelData.ContenderRoll;
            ContenderModifier = duelData.ContenderModifier;
            ContenderAttribute = duelData.ContenderAttribute;

            base.Initialize(data);
        }

        public override void SetRoll(RollData data)
        {
            ContenderRoll = (data as DuelData).ContenderRoll;
            Required = ContenderRoll + ContenderModifier.Sum();

            base.SetRoll(data);
        }
    }
}