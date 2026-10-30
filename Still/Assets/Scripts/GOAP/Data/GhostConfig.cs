using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストの設定
    ///</summary>
    [CreateAssetMenu(fileName = "GhostConfig", menuName = "Config/GOAP/GhostConfig")]
    public class GhostConfig : ScriptableObject
    {
        [Header("デフォルトの設定")]
        public float GhostMoveSpeed;
        public float GhostDashSpeed;
        public float GhostStopDistance;
        public float GhostFovLength;

        [Header("<color=cyan>ここから下はアクションごとの設定</color>")]

        [Header("<color=yellow>PatrolAction</color>")]
        public float Radius;

        [Header("<color=yellow>LookAroundAction</color>")]
        public float Duration;

        [Header("<color=yellow>視界</color>")]
        [Tooltip("プレイヤーを見失ってから PlayerVisible を0に戻すまでの秒数")]
        public float LoseSightSeconds = 5f;

        [Header("<color=yellow>ノック</color>")]
        [Tooltip("プレイヤーが部屋に居続けてからノックするまでの秒数")]
        public float KnockAfterSeconds = 15f;
    }
}
