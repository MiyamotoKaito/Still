using System;

namespace Still.Player.Model
{
    /// <summary>
    /// スタミナモデル
    /// </summary>
    public class StaminaModel
    {
        public event Action<float> OnStaminaChanged;
        public float CurrentStamina => _currentStamina;

        private float _maxStamina;
        private float _currentStamina;

        public StaminaModel(float maxStamina)
        {
            _maxStamina = maxStamina;
            _currentStamina = maxStamina;
        }
        /// <summary>
        /// スタミナの値を変更する
        /// </summary>
        /// <param name="amount"></param>
        public void ModifyStamina(float amount)
        {
            _currentStamina = Math.Clamp(_currentStamina + amount, 0, _maxStamina);
            OnStaminaChanged?.Invoke(_currentStamina);
        }
        /// <summary>
        /// スタミナの最大値を設定する
        /// </summary>
        /// <param name="maxStamina"></param>
        public void SetMaxStamina(float maxStamina)
        {
            _maxStamina = maxStamina;
        }
    }
}
