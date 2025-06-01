using UnityEngine;

namespace Fulbo.Match
{
    public class MPAnimator : MonoBehaviour
    {
        #region Enumerators
        public enum Poses
        {
            Idle, 
            Run
        }
        #endregion

        [SerializeField] private Animator animator;

        private string GetPoseStateName(Poses pose) => pose.ToString();

        public void PlayPose(Poses pose) => animator.Play(GetPoseStateName(pose));

        public void ResetPose() => PlayPose(Poses.Idle);
    }
}