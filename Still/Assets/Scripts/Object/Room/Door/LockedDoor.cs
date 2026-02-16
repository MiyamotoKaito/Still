using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Still.Player.View;

namespace Still.Object.Door.View
{
    public class LockedDoor : DoorView
    {
        public event Action OnDoorLocked;
        [SerializeField]
        private Image _image;
        private bool _isLocked = true;
        private PlayerView _playerView;

        public override void Interact()
        {
            OnDoorLocked?.Invoke();

            if (_isLocked)
            {
                AudioManager.Instance.PlaySE("KeyLocked");
                return;
            }
            ToggleIsOpen();
            GameClear().Forget();
        }

        public override void ShowUI()
        {
            if (_isLocked)
            {
                _text.gameObject.SetActive(true);
                _text.text = "Locked\nCollect The Key!!";
                return;
            }
            base.ShowUI();
        }

        public void GetKey(bool isLocked)
        {
            _isLocked = isLocked;
        }

        private async UniTask GameClear()
        {
            _playerView = GameObject.FindAnyObjectByType<PlayerView>();
            _image.gameObject.SetActive(true);
            _image.material.DOFade(1, 2f).SetEase(Ease.InOutQuad);
            _playerView.DisablePlayerInput();
            await UniTask.Delay(TimeSpan.FromSeconds(4));
            SceneManager.LoadScene("Title");
        }
    }
}
