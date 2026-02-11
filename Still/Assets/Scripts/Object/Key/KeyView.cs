using System;
using TMPro;
using UnityEngine;
namespace Still.Object.Key
{
    public class KeyView : MonoBehaviour, IInteractable
    {
        public event Action OnKeyCollected;

        [SerializeField]
        private TextMeshProUGUI _text;
        public void HideUI()
        {
            _text.gameObject.SetActive(false);
        }

        public void Interact()
        {
            OnKeyCollected?.Invoke();
            this.gameObject.SetActive(false);
        }

        public void ShowUI()
        {
            _text.gameObject.SetActive(true);
        }
    }
}