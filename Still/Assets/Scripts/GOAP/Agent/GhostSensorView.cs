using Still.Common;
using UnityEngine;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストの目。正面へのRaycastでプレイヤーが見えているかだけを返す
    ///</summary>
    public class GhostSensorView : MonoBehaviour
    {
        private IPlayerAvatar _player;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void Construct(IPlayerAvatar player)
        {
            _player = player;
        }

        ///<summary>
        ///遮蔽物越しではなく、正面にプレイヤーが直接見えているか
        ///</summary>
        public bool CanSeePlayer(float distance)
        {
            if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance)) return false;
            return hit.collider.transform.IsChildOf(_player.Transform);
        }
    }
}
