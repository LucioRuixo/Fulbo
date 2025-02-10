using UnityEngine;

namespace Fulbo.Match
{
    public class MatchPlayerBody : MonoBehaviour, ISelectable
    {
        [SerializeField] private MatchPlayer player;

        public MatchPlayer Player => player;
    }
}