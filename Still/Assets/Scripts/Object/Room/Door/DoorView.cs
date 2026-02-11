using TMPro;
using UnityEngine;
namespace Still.Object.Door
{
    public class DoorView : MonoBehaviour, IInteractable
    {
        [SerializeField]
        protected TextMeshProUGUI _text;
        protected Animator _animator;
        protected bool _isOpen = false;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        protected void ToggleIsOpen()
        {
            _isOpen = !_isOpen;
            _animator.SetBool("Open", _isOpen);
        }

        public virtual void Interact()
        {
            ToggleIsOpen();
        }

        public virtual void ShowUI()
        {
            _text.gameObject.SetActive(true);
            _text.text = _isOpen ? "Close Door" : "Open Door";
        }

        public void HideUI()
        {
            _text.gameObject.SetActive(false);
        }
    }
}