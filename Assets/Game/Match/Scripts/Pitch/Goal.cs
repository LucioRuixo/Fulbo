using UnityEngine;

namespace Fulbo.Match
{
    public class Goal : MonoBehaviour
    {
        #region Constants
        private const float RealGoalWidth = 7.32f;
        private const float RealGoalHeight = 2.44f;
        #endregion

        [SerializeField] private float postWidth;

        [Space]

        [SerializeField] private Transform leftPost;
        [SerializeField] private Transform rightPost;
        [SerializeField] private Transform crossbar;

        public Vector3 BottomCenter => transform.position;

        private void Awake() => UpdateDimensions();

        public void UpdateDimensions()
        {
            leftPost.localPosition = new Vector3(0f, RealGoalHeight.Half() + postWidth.Half(), -RealGoalWidth.Half() - postWidth.Half());
            leftPost.localScale = new Vector3(postWidth, RealGoalHeight + postWidth, postWidth);

            rightPost.localPosition = new Vector3(0f, RealGoalHeight.Half() + postWidth.Half(), RealGoalWidth.Half() + postWidth.Half());
            rightPost.localScale = new Vector3(postWidth, RealGoalHeight + postWidth, postWidth);

            crossbar.localPosition = new Vector3(0f, RealGoalHeight + postWidth.Half(), 0f);
            crossbar.localScale = new Vector3(postWidth, postWidth, RealGoalWidth + postWidth * 2f);
        }
    }
}