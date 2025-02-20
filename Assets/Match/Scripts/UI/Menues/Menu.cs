using UnityEngine;

namespace Fulbo.Match.UI
{
    public class Menu : MonoBehaviour
    {
        public virtual void Enable() => gameObject.SetActive(true);

        public virtual void Disable() => gameObject.SetActive(false);
    }
}