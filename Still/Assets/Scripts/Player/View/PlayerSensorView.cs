using System;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーの視線。正面へのRaycastの結果を通知する
    ///</summary>
    public class PlayerSensorView : MonoBehaviour
    {
        ///<summary>調べられる距離で何かに当たった</summary>
        public event Action<RaycastHit> OnHitPlayerDetected;
        ///<summary>見える距離で何かに当たった</summary>
        public event Action<RaycastHit> OnHitDetected;
        ///<summary>調べられる距離に何もない</summary>
        public event Action OnHitPlayerLost;
        ///<summary>見える距離に何もない</summary>
        public event Action OnHitLost;

        ///<summary>カメラに付いている懐中電灯</summary>
        public Light SpotLight => _spotLight != null ? _spotLight : _spotLight = GetComponentInChildren<Light>();

        [SerializeField] private PlayerConfig config;
        [SerializeField] private LayerMask _layerMask;

        private Light _spotLight;

        private void Update()
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit interactHit, config.InteractiveRayDistance, _layerMask))
                OnHitPlayerDetected?.Invoke(interactHit);
            else
                OnHitPlayerLost?.Invoke();

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, config.RayDistance))
                OnHitDetected?.Invoke(hit);
            else
                OnHitLost?.Invoke();
        }

        private void OnDrawGizmos()
        {
            if (config == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * config.RayDistance);
        }
    }
}
