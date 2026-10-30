using System;
using R3;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///SAN値とスタミナを画面に表示する
    ///</summary>
    public sealed class PlayerStatusPresenter : IInitializable, IDisposable
    {
        public PlayerStatusPresenter(PlayerStatusEntity status, SANView sanView, StaminaView staminaView)
        {
            _status = status;
            _sanView = sanView;
            _staminaView = staminaView;
        }

        private readonly PlayerStatusEntity _status;
        private readonly SANView _sanView;
        private readonly StaminaView _staminaView;
        private readonly CompositeDisposable _disposables = new();

        ///<summary>
        ///購読を始める
        ///</summary>
        public void Initialize()
        {
            _status.San.Subscribe(san => _sanView.UpdateSAN(san.Value)).AddTo(_disposables);
            _status.Stamina.Subscribe(stamina => _staminaView.UpdateStamina(stamina.Ratio)).AddTo(_disposables);
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose() => _disposables.Dispose();
    }
}
