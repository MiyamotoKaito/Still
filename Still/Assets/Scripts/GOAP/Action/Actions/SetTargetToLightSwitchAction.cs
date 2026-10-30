using System;
using Still.Common;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///点いている照明スイッチをターゲットにする
    ///</summary>
    [Serializable]
    public class SetTargetToLightSwitchAction : ActionBase
    {
        private ILightSwitchUsecase _lightSwitches;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectDependencies(ILightSwitchUsecase lightSwitches)
        {
            _lightSwitches = lightSwitches;
        }

        ///<summary>
        ///ターゲットが決まったら完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            return agent.CurrentTarget != null;
        }

        ///<summary>
        ///自分と同じ階で点いているスイッチを探す
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            agent.SetTarget(null);
            int floor = agent.Config.FloorOf(agent.Position);
            foreach (var lightSwitch in _lightSwitches.LightSwitches)
            {
                // 別の階のスイッチへはNavMeshがつながっておらず歩いて行けない
                if (!lightSwitch.IsOn || agent.Config.FloorOf(lightSwitch.Transform.position) != floor) continue;
                agent.SetTarget(lightSwitch.Transform);
                return;
            }
        }
    }
}
