using UnityEngine;

namespace Fulbo.Match.UI
{
    public class MatchUIManager : MonoBehaviour
    {
        [SerializeField] private Player player;

        [Header("Menues")]
        [SerializeField] private ActionMenu actionMenu;

        public ActionMenu ActionMenu => actionMenu;

        private void Awake() => InitializeMenues();

        private void InitializeMenues()
        {
            ActionMenu.Initialize(player);
        }
    }
}