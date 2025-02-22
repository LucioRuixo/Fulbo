using UnityEngine;

namespace Fulbo.Match.UI
{
    public class Menu : MonoBehaviour
    {
        protected Player player;

        public void Initialize(Player player) => this.player = player;

        public virtual void Enable() => gameObject.SetActive(true);

        public virtual void Disable() => gameObject.SetActive(false);
    }
}