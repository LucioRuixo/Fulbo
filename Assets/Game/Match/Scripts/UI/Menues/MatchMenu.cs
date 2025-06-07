using UnityEngine;
using UnityEngine.EventSystems;

namespace Fulbo.Match.UI
{
    [RequireComponent(typeof(EventTrigger))]
    public class MatchMenu : MonoBehaviour
    {
        protected HumanPlayer human;

        public static bool BlockingPointer { get; private set; }

        public virtual void Initialize(HumanPlayer human) => this.human = human;

        public virtual void Enable() => gameObject.SetActive(true);

        public virtual void Disable() => gameObject.SetActive(false);

        #region Handlers
        public void OnPointerEnter() => BlockingPointer = true;

        public void OnPointerExit() => BlockingPointer = false;
        #endregion
    }
}