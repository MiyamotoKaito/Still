using UnityEngine;
namespace Still.Object.Door
{
    public class DoorView : MonoBehaviour
    {
        private Animator _animator;
        private bool _isOpen = false;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        public void IsOpen()
        {
            _isOpen = !_isOpen;
            _animator.SetBool("Open", _isOpen);
        }
    }
}