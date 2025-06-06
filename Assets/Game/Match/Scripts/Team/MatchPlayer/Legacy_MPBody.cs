using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Match
{
    public class Legacy_MPBody : MonoBehaviour
    {
        public MatchPlayer Player { get; private set; }

        public void Initialize(MatchPlayer player)
        {
            Player = player;
            GetComponent<MeshRenderer>().SetMaterials(new List<Material>() { Team.GetMaterial(Player.Side) });
        }
    }
}