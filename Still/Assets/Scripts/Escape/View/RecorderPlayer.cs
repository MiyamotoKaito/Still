using System;
using Still.Common;
using TMPro;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///調べると音楽が流れるレコードプレイヤー
    ///</summary>
    public class RecorderPlayer : MonoBehaviour, IInteractable
    {
        public event Action OnInteracted;

        ///<summary>音楽を鳴らすAudioSource</summary>
        public AudioSource AudioSource => _audioSource;

        [SerializeField] private TextMeshProUGUI _text;

        private AudioSource _audioSource;

        ///<summary>
        ///調べる
        ///</summary>
        public void Interact()
        {
            OnInteracted?.Invoke();
        }

        ///<summary>
        ///操作ガイドを表示する
        ///</summary>
        public void ShowUI()
        {
            _text.gameObject.SetActive(true);
            _text.text = "Play Music";
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
            _audioSource = GetComponent<AudioSource>();
        }
    }
}
