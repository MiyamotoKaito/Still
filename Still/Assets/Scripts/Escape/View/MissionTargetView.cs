using System;
using Still.Common;
using Template.Editor;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Still.Escape
{
    ///<summary>
    ///ミッションで調べる物（TV・レコード・本・鍵・写真・絵画・蓄音機など）
    ///</summary>
    // 何をすると目標が進むかはMissionConfig側に書き、ここには調べたときの見た目と音だけを設定する
    public class MissionTargetView : MonoBehaviour, IInteractable
    {
        public event Action<MissionTargetView> OnInteracted;

        ///<summary>ミッションから指定するID</summary>
        public TargetId Id => new TargetId(_targetId);
        ///<summary>使用中か（切り替え式なら現在の状態）</summary>
        public bool IsActive => _isActive;
        ///<summary>切り替え式か</summary>
        public bool IsToggle => _isToggle;
        ///<summary>一度使うと調べられなくなるか</summary>
        public bool IsConsumable => _consumeOnUse;
        ///<summary>使ったときに鳴らすSE（無ければ空）</summary>
        public string UseSe => _useSe;
        ///<summary>使用中にループさせるBGM（無ければ空）</summary>
        public string UseBgm => _useBgm;
        ///<summary>使ったときの演出（無ければnull）</summary>
        public IEvent UseEvent => _useEvent;
        ///<summary>音を鳴らす位置のAudioSource（無ければnull）</summary>
        public AudioSource AudioSource => _audioSource != null ? _audioSource : _audioSource = GetComponent<AudioSource>();

        [SerializeField, Tooltip("ミッションから指定するID")] private string _targetId;
        [SerializeField] private TextMeshProUGUI _guideText;
        [SerializeField, Tooltip("調べられるときに出す操作ガイド")] private string _guideLabel;
        [SerializeField, Tooltip("切り替え式で使用中のときの操作ガイド（空なら通常と同じ）")] private string _activeGuideLabel;

        [Header("使ったとき")]
        [SerializeField, Tooltip("一度使ったら調べられなくなる")] private bool _consumeOnUse;
        [SerializeField, Tooltip("使ったら消える（拾う物）")] private bool _hideOnUse;
        [SerializeField, Tooltip("使うたびにON/OFFが切り替わる")] private bool _isToggle;
        [SerializeField] private string _useSe;
        [SerializeField] private string _useBgm;
        [SerializeField, Tooltip("使用中だけ有効にするもの（TVの光など）")] private Behaviour[] _enableWhileActive = Array.Empty<Behaviour>();
        [SerializeReference, SubclassSelector] private IEvent _useEvent;

        [Header("出現")]
        [SerializeField, Tooltip("ミッションで出現させるまで隠しておく")] private bool _hiddenAtStart;
        [SerializeField, Tooltip("出現時にこの中からランダムな位置へ移動する（空ならその場）")] private Transform[] _spawnPositions = Array.Empty<Transform>();

        private AudioSource _audioSource;
        private bool _isUsed;
        private bool _isActive;

        ///<summary>
        ///調べる
        ///</summary>
        public void Interact()
        {
            if (_isUsed && _consumeOnUse) return;
            OnInteracted?.Invoke(this);
        }

        ///<summary>
        ///操作ガイドを表示する
        ///</summary>
        public void ShowUI()
        {
            if (_guideText == null || (_isUsed && _consumeOnUse)) return;

            bool useActiveLabel = _isToggle && _isActive && !string.IsNullOrEmpty(_activeGuideLabel);
            _guideText.text = useActiveLabel ? _activeGuideLabel : _guideLabel;
            _guideText.gameObject.SetActive(true);
        }

        ///<summary>
        ///操作ガイドを隠す
        ///</summary>
        public void HideUI()
        {
            if (_guideText != null) _guideText.gameObject.SetActive(false);
        }

        ///<summary>
        ///ゲーム開始時の状態にする
        ///</summary>
        public void ResetState()
        {
            _isUsed = false;
            _isActive = false;
            SetActiveVisuals(false);
            if (_hiddenAtStart) gameObject.SetActive(false);
        }

        ///<summary>
        ///使った見た目にする
        ///</summary>
        public void Use()
        {
            _isUsed = true;
            _isActive = !_isToggle || !_isActive;
            SetActiveVisuals(_isActive);

            if (_consumeOnUse) HideUI();
            if (!_hideOnUse) return;

            HideUI();
            gameObject.SetActive(false);
        }

        ///<summary>
        ///出現させる
        ///</summary>
        public void Reveal()
        {
            if (_spawnPositions.Length > 0) transform.position = _spawnPositions[Random.Range(0, _spawnPositions.Length)].position;
            gameObject.SetActive(true);
        }

        private void SetActiveVisuals(bool isActive)
        {
            foreach (var behaviour in _enableWhileActive)
            {
                if (behaviour != null) behaviour.enabled = isActive;
            }
        }
    }
}
