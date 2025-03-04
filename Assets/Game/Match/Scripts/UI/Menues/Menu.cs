using UnityEngine;

namespace Fulbo.Match.UI
{
    public class Menu : MonoBehaviour
    {
        protected HumanPlayer human;

        public void Initialize(HumanPlayer human) => this.human = human;

        public virtual void Enable() => gameObject.SetActive(true);

        public virtual void Disable() => gameObject.SetActive(false);
    }
}