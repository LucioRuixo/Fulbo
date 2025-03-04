using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Match.UI
{
    public class MatchUIManager : MonoBehaviour
    {
        [SerializeField] private HumanPlayer human;
        [SerializeField] private Match match;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera camera;

        [Space]

        [SerializeField] private RollUI rollUI;

        [Header("Menues")]
        [SerializeField] private ActionMenu actionMenu;

        private CanvasScaler canvasScaler;

        public Vector2 ReferenceResolution => canvasScaler.referenceResolution;

        public RollUI RollUI => rollUI;
        public ActionMenu ActionMenu => actionMenu;

        private void Awake()
        {
            canvasScaler = canvas.GetComponent<CanvasScaler>();

            InitializeManagers();
            InitializeMenues();
        }

        private void InitializeManagers() => rollUI.Initialize(canvas, camera, match);

        private void InitializeMenues() => ActionMenu.Initialize(human);
    }
}