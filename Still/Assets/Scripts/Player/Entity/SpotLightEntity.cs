using System;
using R3;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーの懐中電灯
    ///</summary>
    public sealed class SpotLightEntity : IDisposable
    {
        ///<summary>点いているか</summary>
        public ReadOnlyReactiveProperty<bool> IsOn => _isOn;

        private readonly ReactiveProperty<bool> _isOn = new(false);

        ///<summary>
        ///点灯／消灯を切り替える
        ///</summary>
        public void Toggle() => _isOn.Value = !_isOn.Value;

        ///<summary>
        ///破棄する
        ///</summary>
        public void Dispose() => _isOn.Dispose();
    }
}
