using System;
using Still.Common;
using Template.Editor;
using TMPro;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///拾うと演出が起きる本
    ///</summary>
    public class Book : MonoBehaviour, IInteractable
    {
        public event Action OnInteracted;

        ///<summary>拾ったときの演出（無ければnull）</summary>
        public IEvent PickUpEvent => _event;

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeReference, SubclassSelector]
        private IEvent _event;

        ///<summary>
        ///調べる
        ///</summary>
        public void Interact()
        {
            OnInteracted?.Invoke();
        }

        ///<summary>
        ///拾われて消える
        ///</summary>
        public void PickUp()
        {
            HideUI();
            gameObject.SetActive(false);
        }

        ///<summary>
        ///操作ガイドを表示する
        ///</summary>
        public void ShowUI()
        {
            _text.gameObject.SetActive(true);
            _text.text = "Collect The Book!";
        }

        ///<summary>
        ///操作ガイドを隠す
        ///</summary>
        public void HideUI()
        {
            _text.gameObject.SetActive(false);
        }
    }
}
