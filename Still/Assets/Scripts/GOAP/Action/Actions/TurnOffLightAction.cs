using System;
using Still.Common;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///ターゲットの照明スイッチを消す
    ///</summary>
    [Serializable]
    public class TurnOffLightAction : ActionBase
    {
        private ILightSwitchUsecase _lightSwitches;
        private ILightSwitch _target;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectDependencies(ILightSwitchUsecase lightSwitches)
        {
            _lightSwitches = lightSwitches;
        }

        ///<summary>
        ///点いていれば消して完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            if (_target != null && _target.IsOn) _lightSwitches.Toggle(_target);
            return true;
        }

        ///<summary>
        ///ターゲットのTransformに対応するスイッチを探す
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            _target = null;
            foreach (var lightSwitch in _lightSwitches.LightSwitches)
            {
                if (lightSwitch.Transform != agent.CurrentTarget) continue;
                _target = lightSwitch;
                return;
            }
        }
    }
}
