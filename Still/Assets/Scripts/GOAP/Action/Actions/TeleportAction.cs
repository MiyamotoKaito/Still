using System;
using System.Linq;
using Still.Common;
using UnityEngine;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///プレイヤーと同じ階で、いちばん遠いテレポート地点へ瞬間移動する
    ///</summary>
    [Serializable]
    public class TeleportAction : ActionBase
    {
        private TeleportPoints _teleportPoints;
        private IPlayerAvatar _player;
        private Vector3 _farthestPos;
        private bool _hasDestination;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectDependencies(TeleportPoints teleportPoints, IPlayerAvatar player)
        {
            _teleportPoints = teleportPoints;
            _player = player;
        }

        ///<summary>
        ///移動先があればテレポートして完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            if (!_hasDestination) return true;

            agent.Teleport(_farthestPos);
            return true;
        }

        ///<summary>
        ///XZ平面のマンハッタン距離で最も遠い地点を選ぶ
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            // 別の階へ飛ぶとプレイヤーとの関わりが切れるので、プレイヤーと同じ階の地点から選ぶ
            int playerFloor = agent.Config.FloorOf(_player.Transform.position);
            var candidates = _teleportPoints.Points.Where(p => agent.Config.FloorOf(p.position) == playerFloor).ToList();
            if (candidates.Count == 0) candidates = _teleportPoints.Points.ToList();

            _hasDestination = false;
            float farthest = -1f;
            foreach (var point in candidates)
            {
                var pos = point.position;
                float distance = Mathf.Abs(agent.Position.x - pos.x) + Mathf.Abs(agent.Position.z - pos.z);
                if (distance <= farthest) continue;

                farthest = distance;
                _farthestPos = pos;
                _hasDestination = true;
            }
        }
    }
}
