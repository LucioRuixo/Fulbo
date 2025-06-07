using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Match.UI
{
    public class ActionPointIcon : MonoBehaviour
    {
        [SerializeField] private Image filler;

        public void Fill() => filler.gameObject.SetActive(true);

        public void Empty() => filler.gameObject.SetActive(false);
    }
}