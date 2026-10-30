using System.Collections.Generic;
using Still.Common;
using Still.Utility;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///照明スイッチの切り替え。点灯数をワールドステートへ反映する
    ///</summary>
    // 点灯状態はスイッチごとの見た目と常に一致するので、Entityを挟まずViewの状態を正としている
    public sealed class LightSwitchUsecase : ILightSwitchUsecase
    {
        public LightSwitchUsecase(SceneComponentList<LightSwitch> lightSwitches, IWorldStates worldStates, IAudioService audio)
        {
            _lightSwitches = lightSwitches.Items;
            _worldStates = worldStates;
            _audio = audio;
        }

        ///<summary>シーン内の全スイッチ</summary>
        public IReadOnlyList<ILightSwitch> LightSwitches => _lightSwitches;

        private readonly IReadOnlyList<LightSwitch> _lightSwitches;
        private readonly IWorldStates _worldStates;
        private readonly IAudioService _audio;

        ///<summary>
        ///全スイッチを消灯状態から始める
        ///</summary>
        public void TurnOffAll()
        {
            foreach (var lightSwitch in _lightSwitches) lightSwitch.SetOn(false);
        }

        ///<summary>
        ///スイッチを切り替える
        ///</summary>
        public void Toggle(ILightSwitch lightSwitch)
        {
            if (lightSwitch is not LightSwitch view)
            {
                Debug.LogError($"[LightSwitchUsecase] 未知のスイッチです: {lightSwitch}");
                return;
            }

            _audio.PlaySe("Light1");
            view.SetOn(!view.IsOn);
            _worldStates.AddState(WorldStateType.OnLighting, view.IsOn ? 1 : -1);
        }
    }
}
