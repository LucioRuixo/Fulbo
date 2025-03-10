using UnityEngine;
using UnityEngine.UI;

namespace Fulbo
{
    public class UIUtils
    {
        public static Vector2 ScreenToCanvasPoint(Vector2 point, Vector2 referenceResolution)
        {
            int x = Mathf.RoundToInt(Mathf.Lerp(referenceResolution.x * -0.5f, referenceResolution.x * 0.5f, point.x / Screen.width));
            int y = Mathf.RoundToInt(Mathf.Lerp(referenceResolution.y * -0.5f, referenceResolution.y * 0.5f, point.y / Screen.height));
            return new Vector2(x, y);
        }

        public static void ForceRebuildChildrenLayoutsImmediate(LayoutGroup[] layoutGroups)
        {
            if (layoutGroups == null) return;
            
            for (int i = layoutGroups.Length - 1; i >= 0; i--) LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroups[i].transform as RectTransform);
        }
    }
}