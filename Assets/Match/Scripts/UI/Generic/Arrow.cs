using UnityEngine;

namespace Fulbo.Match.UI
{
    public class Arrow : MPHUDElement
    {
        private float size;
        private float? height;

        public void Initialize(float size, float? height = null)
        {
            this.size = size;
            this.height = height;
            transform.localScale = Vector3.one * size;
        }

        public void Point(Vector3 from, Vector3 to)
        {
            if (height.HasValue) from.y = height.Value;
            if (height.HasValue) to.y = height.Value;

            Vector3 fromTo = to - from;
            float distance = fromTo.magnitude;

            transform.position = from + fromTo * 0.5f;
            transform.rotation = Quaternion.LookRotation(fromTo.normalized, Vector3.up);
            sprite.size = new Vector2(distance / size, sprite.size.y);
        }
    }
}