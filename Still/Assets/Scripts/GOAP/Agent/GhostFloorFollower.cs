using System.Linq;
using Still.Common;
using UnityEngine;
using VContainer.Unity;

namespace Still.Goap
{
    ///<summary>
    ///プレイヤーが別の階へ移ってしばらくしたら、ゴーストもその階へ移る
    ///</summary>
    // 階どうしはNavMeshでつながっていないので、歩いて追う代わりにテレポートで「ついてきた」ことにする
    public sealed class GhostFloorFollower : ITickable
    {
        public GhostFloorFollower(IAgentController agent, IPlayerAvatar player, GhostConfig config,
                                  TeleportPoints teleportPoints, GoapExecutor executor)
        {
            _agent = agent;
            _player = player;
            _config = config;
            _teleportPoints = teleportPoints;
            _executor = executor;
        }

        private float _apartSeconds;

        private readonly IAgentController _agent;
        private readonly IPlayerAvatar _player;
        private readonly GhostConfig _config;
        private readonly TeleportPoints _teleportPoints;
        private readonly GoapExecutor _executor;

        ///<summary>
        ///違う階にいる時間を数え、超えたらプレイヤーの階へ移る
        ///</summary>
        public void Tick()
        {
            int playerFloor = _config.FloorOf(_player.Transform.position);
            if (_config.FloorOf(_agent.Position) == playerFloor)
            {
                _apartSeconds = 0f;
                return;
            }

            _apartSeconds += Time.deltaTime;
            if (_apartSeconds < _config.FollowFloorSeconds) return;
            _apartSeconds = 0f;

            // 目の前に現れると興ざめなので、その階でプレイヤーから最も遠い地点に出る
            var playerPos = _player.Transform.position;
            var destination = _teleportPoints.Points
                .Where(p => _config.FloorOf(p.position) == playerFloor)
                .OrderByDescending(p => (p.position - playerPos).sqrMagnitude)
                .FirstOrDefault();
            if (destination == null) return;

            _agent.Teleport(destination.position);
            // 前の階で立てた計画（別の階の目的地など）は使えないので立て直させる
            _agent.SetTarget(null);
            _executor.Replan();
        }
    }
}
