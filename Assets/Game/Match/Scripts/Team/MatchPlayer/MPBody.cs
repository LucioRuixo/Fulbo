using UnityEngine;

namespace Fulbo.Match
{
    public class MPBody : MonoBehaviour
    {
        #region Enumerators
        private enum Materials
        {
            Shirt, 
            Skin, 
            Shorts, 
            Boots
        }
        #endregion

        [SerializeField] private Renderer playerMesh;
        [SerializeField] private Renderer shirtMesh;
        [SerializeField] private Renderer shortsMesh;

        [Space]

        [SerializeField] private MPAnimator animator;

        public MatchPlayer Player { get; private set; }
        public MPAnimator Animator => animator;

        public void Initialize(MatchPlayer player)
        {
            Player = player;

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", Team.GetColor(Player.Side));
            playerMesh.SetPropertyBlock(propertyBlock, (int)Materials.Shirt);
        }
    }
}