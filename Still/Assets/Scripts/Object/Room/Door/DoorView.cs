using TMPro;
using UnityEngine;
namespace Still.Object.Door
{
    public class DoorView : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private TextMeshProUGUI _text;
        private Animator _animator;
        private bool _isOpen = false;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        private void ToggleIsOpen()
        {
            _isOpen = !_isOpen;
            _animator.SetBool("Open", _isOpen);
        }

        public void Interact()
        {
            ToggleIsOpen();
        }

        public void ShowUI()
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