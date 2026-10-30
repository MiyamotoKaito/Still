using System;
using R3;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーのSAN値とスタミナ
    ///</summary>
    public sealed class PlayerStatusEntity : IDisposable
    {
        public PlayerStatusEntity(PlayerConfig config)
        {
            _san = new ReactiveProperty<SanValue>(new SanValue(config.DefaultSANValue, config.DefaultSANValue));
            _stamina = new ReactiveProperty<StaminaValue>(new StaminaValue(config.DefaultStaminaValue, config.DefaultStaminaValue));
        }

        ///<summary>SAN値</summary>
        public ReadOnlyReactiveProperty<SanValue> San => _san;
        ///<summary>スタミナ</summary>
        public ReadOnlyReactiveProperty<StaminaValue> Stamina => _stamina;

        private readonly ReactiveProperty<SanValue> _san;
        private readonly ReactiveProperty<StaminaValue> _stamina;

        ///<summary>
        ///SAN値を差し替える
        ///</summary>
        public void SetSan(SanValue san) => _san.Value = san;

        ///<summary>
        ///スタミナを差し替える
        ///</summary>
        public void SetStamina(StaminaValue stamina) => _stamina.Value = stamina;

        ///<summary>
        ///破棄する
        ///</summary>
        public void Dispose()
        {
            _san.Dispose();
            _stamina.Dispose();
        }
    }
}
