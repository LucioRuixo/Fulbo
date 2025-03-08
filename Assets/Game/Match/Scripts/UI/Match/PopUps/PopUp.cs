using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Match.UI
{
    public abstract class PopUp : MonoBehaviour
    {
        protected RectTransform rect;

        public Canvas Canvas { get; set; }
        public Camera Camera { get; set; }
        public RectTransform Rect => rect ??= GetComponent<RectTransform>();

        protected LayoutGroup[] layoutGroups;

        private void Awake() => layoutGroups = GetComponentsInChildren<LayoutGroup>(true);

        protected void OnContentUpdated() => UIUtils.ForceRebuildChildrenLayoutsImmediate(layoutGroups);
    }
}