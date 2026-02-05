using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class TurnOffLightAction : ActionBase
    {
        public override bool Perform(IAgentController agent)
        {
            if (_lightSwith == null)
            {
                _lightSwith = GameObject.FindAnyObjectByType<LightSwitch>();
                return false; // ターゲットがなければ即完了
            }

            // スイッチを切り替える（物理的な操作）
            if (_lightSwith.IsOn) // 電気がついている場合のみオフにする
            {
                _lightSwith.SwitchToggle(); // スイッチをオフにする操作
                Debug.Log($"[Action] {nameof(TurnOffLightAction)}: 電気スイッチをオフにしました。");
            }
            return true; // アクションは完了
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
            _lightSwith = agent.CurrentTarget.GetComponent<LightSwitch>();
        }
        private LightSwitch _lightSwith;
    }
}
