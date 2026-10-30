using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Still.Common;
using UnityEngine;

namespace Still.Utility
{
    ///<summary>
    ///モード遷移を管理する
    ///</summary>
    public sealed class StateMachine : IStateMachine, IDisposable
    {
        ///<summary>現在のモード</summary>
        public ReadOnlyReactiveProperty<GameModeType> CurrentMode => _currentMode;

        private IState _currentState;
        private bool _isTransitioning;

        private readonly Dictionary<GameModeType, IState> _states = new();
        private readonly ReactiveProperty<GameModeType> _currentMode = new();
        private readonly CancellationTokenSource _disposeCts = new();

        ///<summary>
        ///Stateを登録する
        ///</summary>
        public void AddState(IState state)
        {
            _states[state.GameModeType] = state;
        }

        ///<summary>
        ///Stateの登録を解除する（スコープ破棄時に必ず呼ぶ）
        ///</summary>
        public void RemoveState(IState state)
        {
            if (_states.TryGetValue(state.GameModeType, out var registered) && registered == state)
            {
                _states.Remove(state.GameModeType);
            }
            if (_currentState == state) _currentState = null;
        }

        ///<summary>
        ///指定したモードへ遷移する。遷移中の要求は無視する
        ///</summary>
        public async UniTask ChangeStateAsync(GameModeType gameModeType)
        {
            // ゲームオーバーとクリアが同時に起きたときなどに二重遷移させない
            if (_isTransitioning) return;

            if (!_states.TryGetValue(gameModeType, out var next))
            {
                Debug.LogError($"[StateMachine] {gameModeType} のStateが登録されていません");
                return;
            }

            _isTransitioning = true;
            try
            {
                var ct = _disposeCts.Token;
                if (_currentState != null) await _currentState.ExitAsync(ct);
                _currentState = next;
                _currentMode.Value = gameModeType;
                await next.EnterAsync(ct);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        ///<summary>
        ///破棄する
        ///</summary>
        public void Dispose()
        {
            _disposeCts.Cancel();
            _disposeCts.Dispose();
            _currentMode.Dispose();
        }
    }
}
