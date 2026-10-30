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
        ///点いているスイッチを探す
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            agent.SetTarget(null);
            foreach (var lightSwitch in _lightSwitches.LightSwitches)
            {
                if (!lightSwitch.IsOn) continue;
                agent.SetTarget(lightSwitch.Transform);
                return;
            }
        }
    }
}
