using Still.Common;

namespace Still.Player
{
    ///<summary>
    ///SAN値とスタミナのルール
    ///</summary>
    public sealed class PlayerStatusUsecase : ISanUsecase
    {
        public PlayerStatusUsecase(PlayerStatusEntity entity, PlayerConfig config)
        {
            _entity = entity;
            _config = config;
        }

        ///<summary>SAN値が尽きてゲームオーバーになるか</summary>
        public bool IsDepleted => _entity.San.CurrentValue.IsDepleted;
        ///<summary>ダッシュできるか</summary>
        public bool CanDash => !_entity.Stamina.CurrentValue.IsEmpty;
        ///<summary>スタミナ回復の倍率（恐怖で息が上がると下がる）</summary>
        public float RecoveryMultiplier { get; set; } = 1f;

        private readonly PlayerStatusEntity _entity;
        private readonly PlayerConfig _config;

        ///<summary>
        ///SAN値を減らす
        ///</summary>
        public void Damage(float amount)
        {
            _entity.SetSan(_entity.San.CurrentValue.Add(-amount));
        }

        ///<summary>
        ///経過時間分のスタミナ消費／回復とSAN値の消耗を反映する
        ///</summary>
        public void Tick(bool isDashing, float deltaTime)
        {
            var stamina = _entity.Stamina.CurrentValue;
            // スタミナが尽きてもダッシュボタンを押している間は回復させない
            if (isDashing)
            {
                _entity.SetStamina(stamina.Add(-_config.DashStaminaCost * deltaTime));
                // 走り回るほど正気を失う
                Damage(_config.DashSanCost * deltaTime);
                return;
            }

            if (!stamina.IsFull) _entity.SetStamina(stamina.Add(_config.StaminaRecovery * RecoveryMultiplier * deltaTime));
        }
    }
}
