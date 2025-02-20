using UnityEngine;

namespace Fulbo.Match.UI
{
    public class MatchUIManager : MonoBehaviour
    {
        [SerializeField] private ActionMenu actionMenu;

        public ActionMenu ActionMenu => actionMenu;
    }
}