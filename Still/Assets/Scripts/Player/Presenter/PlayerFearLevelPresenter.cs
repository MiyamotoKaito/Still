using System;
using R3;
using Still.Common;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///SAN値から求めた恐怖レベルをゴーストのワールドステートへ渡す
    ///</summary>
    // 以前はWorldStatesManagerとこのクラスが別々の閾値で同じキーを書き換えていた。計算はSanValueに一本化した
    public sealed class PlayerFearLevelPresenter : IInitializable, IDisposable
    {
        public PlayerFearLevelPresenter(PlayerStatusEntity status, IWorldStates worldStates)
        {
            _status = status;
            _worldStates = worldStates;
        }

        private IDisposable _subscription;

        private readonly PlayerStatusEntity _status;
        private readonly IWorldStates _worldStates;

        ///<summary>
        ///購読を始める
        ///</summary>
        public void Initialize()
        {
            _subscription = _status.San
                .Select(san => san.ToFearLevel())
                .DistinctUntilChanged()
                .Subscribe(level => _worldStates.ModifyState(WorldStateType.FearLevel, level));
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose() => _subscription?.Dispose();
    }
}
