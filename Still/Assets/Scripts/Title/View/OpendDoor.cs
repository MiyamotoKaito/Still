using System;
using UnityEngine;

namespace Still.Title
{
    ///<summary>
    ///カメラが近づくと勝手に開くドア（タイトル演出用）
    ///</summary>
    public class OpendDoor : MonoBehaviour
    {
        public event Action OnOpened;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Camera>(out _)) return;

            _animator.SetTrigger("Open");
            OnOpened?.Invoke();
        }
    }
}
