using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーの設定
    ///</summary>
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Player/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        public float PlayerDefaultSpeed;
        public float PlayerMaxSpeed;
        public float DefaultStaminaValue;
        public float DefaultSANValue;
        public float DefaultFOV;
        public float RayDistance;
        public float InteractiveRayDistance;

        [Header("ダッシュ")]
        [Tooltip("ダッシュ中に1秒あたり減るスタミナ")]
        public float DashStaminaCost = 10f;
        [Tooltip("ダッシュしていないときに1秒あたり回復するスタミナ")]
        public float StaminaRecovery = 5f;
        [Tooltip("ダッシュ中に1秒あたり減るSAN値")]
        public float DashSanCost = 0.1f;
        [Tooltip("ダッシュ中の視野角")]
        public float DashFOV = 80f;
    }
}
