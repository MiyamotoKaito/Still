using System;
using Still.Common;
using TMPro;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///開け閉めできるドア
    ///</summary>
    public class DoorView : MonoBehaviour, IInteractable
    {
        public event Action OnInteracted;

        [SerializeField] protected TextMeshProUGUI _text;

        private Animator _animator;
        private bool _isOpen;

        ///<summary>
        ///調べる
        ///</summary>
        public void Interact()
        {
            OnInteracted?.Invoke();
        }

        ///<summary>
        ///開閉を切り替える
        ///</summary>
        public void Toggle()
        {
            _isOpen = !_isOpen;
            _animator.SetBool("Open", _isOpen);
        }

        ///<summary>
        ///操作ガイドを表示する
        ///</summary>
        public virtual void ShowUI()
        {
            _text.gameObject.SetActive(true);
            _text.text = _isOpen ? "Close Door" : "Open Door";
        }

        ///<summary>
        ///操作ガイドを隠す
        ///</summary>
        public void HideUI()
        {
            _text.gameObject.SetActive(false);
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
    }
}
