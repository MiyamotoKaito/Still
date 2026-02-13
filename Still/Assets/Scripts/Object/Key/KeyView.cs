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
            if (_text == null)
            {
                Debug.LogError($"{gameObject.name} の _text がアサインされていません！");
                return;
            }
            Debug.Log($"KeyView: {gameObject.name} ShowUI called."); // どのオブジェクトか判別
            _text.gameObject.SetActive(true);
            _text.text = "Collect Key";
        }
    }
}