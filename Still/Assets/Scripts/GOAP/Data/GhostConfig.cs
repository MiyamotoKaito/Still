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

        [Header("<color=yellow>NoticeAction</color>")]
        [Tooltip("見つけてから追い始めるまで、無言で見つめる秒数")]
        public float NoticeSeconds = 1.75f;
        [Tooltip("見つめている間にプレイヤーの方へ向き直る速さ（度/秒）")]
        public float NoticeTurnSpeed = 90f;

        [Header("<color=yellow>視界</color>")]
        [Tooltip("プレイヤーを見失ってから PlayerVisible を0に戻すまでの秒数")]
        public float LoseSightSeconds = 5f;

        [Header("<color=yellow>ノック</color>")]
        [Tooltip("プレイヤーが部屋に居続けてからノックするまでの秒数")]
        public float KnockAfterSeconds = 15f;

        [Header("<color=yellow>階</color>")]
        [Tooltip("1階分の高さ。y座標をこれで割って何階かを判断する")]
        public float FloorHeight = 10f;
        [Tooltip("プレイヤーと違う階にいる時間がこれを超えたら、プレイヤーの階へ移る")]
        public float FollowFloorSeconds = 20f;

        ///<summary>
        ///位置が何階か（1階=0）
        ///</summary>
        public int FloorOf(Vector3 position) => Mathf.RoundToInt(position.y / FloorHeight);
    }
}
