using System;
using UnityEngine;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///いちばん遠いテレポート地点へ瞬間移動する
    ///</summary>
    [Serializable]
    public class TeleportAction : ActionBase
    {
        private TeleportPoints _teleportPoints;
        private Vector3 _farthestPos;
        private bool _hasDestination;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectDependencies(TeleportPoints teleportPoints)
        {
            _teleportPoints = teleportPoints;
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
            _hasDestination = false;
            float farthest = -1f;
            foreach (var point in _teleportPoints.Points)
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
