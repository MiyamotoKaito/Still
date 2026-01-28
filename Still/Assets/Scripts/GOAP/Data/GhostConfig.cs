using UnityEngine;
namespace Still.GOAP.Agent.Config
{
    /// <summary>
    /// Ghostの設定
    /// </summary>
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

        [Header("<color=yellow>SearchAction</color>")]
        public float Duration;
    }
}