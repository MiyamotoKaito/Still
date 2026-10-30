using System;
using Still.Common;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///プレイヤーが入ったかを検知する部屋のトリガー
    ///</summary>
    public class Room : MonoBehaviour
    {
        public event Action OnPlayerEntered;
        public event Action OnPlayerExited;

        ///<summary>プレイヤーが中にいるか</summary>
        public bool IsPlayerInside => _isPlayerInside;
        ///<summary>この部屋のドア（ゴーストがノックしに来る場所）</summary>
        public Transform Door => _door.transform;

        [SerializeField] private GameObject _door;

        private bool _isPlayerInside;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<IPlayerAvatar>() == null) return;

            _isPlayerInside = true;
            OnPlayerEntered?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<IPlayerAvatar>() == null) return;

            _isPlayerInside = false;
            OnPlayerExited?.Invoke();
        }
    }
}
