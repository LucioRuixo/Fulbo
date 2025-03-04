using UnityEngine;

namespace Fulbo.Match.UI
{
    public class MPHUDElement : MonoBehaviour
    {
        [Space]

        [SerializeField] protected SpriteRenderer sprite;

        public void Show() => sprite.gameObject.SetActive(true);

        public void Hide() => sprite.gameObject.SetActive(false);
    }
}