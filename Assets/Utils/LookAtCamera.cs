using UnityEngine;

namespace Fulbo
{
    public class LookAtCamera : MonoBehaviour
    {
        private void Update()
        {
            float angleToCamera = Vector3.SignedAngle(Vector3.forward, transform.position.HorizontalDirectionTo(Camera.main.transform.position), Vector3.up);
            transform.rotation = Quaternion.AngleAxis(angleToCamera, Vector3.up);
        }
    }
}