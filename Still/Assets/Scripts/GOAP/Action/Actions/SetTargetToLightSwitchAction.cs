using System;
using Still.Common;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///点いている照明スイッチをターゲットにする
    ///</summary>
    [Serializable]
    public class SetTargetToLightSwitchAction : ActionBase
    {
        // スイッチの位置からNavMeshを探す範囲（スイッチは壁ぎわにあるので少し広めに取る）
        private const float SampleRadius = 1.5f;

        private ILightSwitchUsecase _lightSwitches;
        private bool _hasTarget;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectDependencies(ILightSwitchUsecase lightSwitches)
        {
            _lightSwitches = lightSwitches;
        }

        ///<summary>
        ///歩いて行けるスイッチが見つからなければ、計画を破棄させる
        ///</summary>
        public override bool Execute(IAgentController agent)
        {
            return _hasTarget && base.Execute(agent);
        }

        ///<summary>
        ///ターゲットが決まったら完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            return agent.CurrentTarget != null;
        }

        ///<summary>
        ///自分と同じ階で点いていて、歩いて行けるスイッチを探す
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            agent.SetTarget(null);
            _hasTarget = false;
            int floor = agent.Config.FloorOf(agent.Position);
            foreach (var lightSwitch in _lightSwitches.LightSwitches)
            {
                // 別の階のスイッチへはNavMeshがつながっておらず歩いて行けない
                if (!lightSwitch.IsOn || agent.Config.FloorOf(lightSwitch.Transform.position) != floor) continue;
                // 2階の部屋のようにNavMeshを張っていない場所のスイッチへは行けないので狙わない
                if (!IsReachable(agent.Position, lightSwitch.Transform.position)) continue;

                agent.SetTarget(lightSwitch.Transform);
                _hasTarget = true;
                return;
            }
        }

        private static bool IsReachable(Vector3 from, Vector3 to)
        {
            if (!NavMesh.SamplePosition(from, out var start, SampleRadius, NavMesh.AllAreas)) return false;
            if (!NavMesh.SamplePosition(to, out var goal, SampleRadius, NavMesh.AllAreas)) return false;

            var path = new NavMeshPath();
            return NavMesh.CalculatePath(start.position, goal.position, NavMesh.AllAreas, path)
                   && path.status == NavMeshPathStatus.PathComplete;
        }
    }
}
