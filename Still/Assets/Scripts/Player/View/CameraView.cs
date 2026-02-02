using UnityEngine;
namespace Still.Player.View
{
    public class CameraView : MonoBehaviour
    {
        public bool IsObjectHit => _isObjectHit;

        private bool _isObjectHit;

        private void CheckObjectHit(float FovLength)
        {
            if (Physics.Raycast(this.transform.position, this.transform.forward, FovLength))
            {
                _isObjectHit = true;
            }
            else
            {
                _isObjectHit = false;
            }
        }
    }
}