using System;

namespace Still.Player.Model
{
    /// <summary>
    /// SAN値モデル
    /// </summary>
    public class SANValueModel
    {
        public event Action<float> OnSANChanged;
        public float CurrentSAN => _currentSan;

        private float _maxSan;
        private float _currentSan;
        public SANValueModel(float maxSan)
        {
            _maxSan = maxSan;
            _currentSan = maxSan;
        }
        /// <summary>
        /// SAN値を変更する
        /// </summary>
        /// <param name="amount"></param>
        public void ModifySAN(float amount)
        {
            _currentSan = Math.Clamp(_currentSan - amount, 0, _maxSan);
            OnSANChanged?.Invoke(_currentSan);
        }
        /// <summary>
        /// SAN値の最大値を設定する
        /// </summary>
        /// <param name="maxSan"></param>
        public void SetMaxSAN(float maxSan)
        {
            _maxSan = maxSan;
        }
    }
}
