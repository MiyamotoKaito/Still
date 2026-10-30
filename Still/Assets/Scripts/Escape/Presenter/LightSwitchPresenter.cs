using System;
using System.Collections.Generic;
using Still.Utility;
using VContainer.Unity;

namespace Still.Escape
{
    ///<summary>
    ///プレイヤーが調べた照明スイッチを切り替える
    ///</summary>
    public sealed class LightSwitchPresenter : IInitializable, IDisposable
    {
        public LightSwitchPresenter(SceneComponentList<LightSwitch> lightSwitches, LightSwitchUsecase usecase)
        {
            _lightSwitches = lightSwitches.Items;
            _usecase = usecase;
        }

        private readonly IReadOnlyList<LightSwitch> _lightSwitches;
        private readonly LightSwitchUsecase _usecase;
        private readonly List<(LightSwitch LightSwitch, Action Handler)> _handlers = new();

        ///<summary>
        ///全スイッチを消灯して購読を始める
        ///</summary>
        public void Initialize()
        {
            _usecase.TurnOffAll();
            foreach (var lightSwitch in _lightSwitches)
            {
                Action handler = () => _usecase.Toggle(lightSwitch);
                lightSwitch.OnInteracted += handler;
                _handlers.Add((lightSwitch, handler));
            }
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose()
        {
            foreach (var (lightSwitch, handler) in _handlers)
            {
                if (lightSwitch != null) lightSwitch.OnInteracted -= handler;
            }
            _handlers.Clear();
        }
    }
}
