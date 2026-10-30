using System;
using System.Collections.Generic;
using R3;
using Still.Common;
using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///GOAPのワールドステート
    ///</summary>
    public sealed class WorldStates : IWorldStates, IDisposable
    {
        public WorldStates(WorldStateConfig config)
        {
            _states = config.GetStates();
        }

        ///<summary>値が変わったキーを通知する</summary>
        public Observable<WorldStateType> OnStateChanged => _onStateChanged;
        ///<summary>現在の全ステート</summary>
        public IReadOnlyDictionary<WorldStateType, int> CurrentStates => _states;

        private readonly Dictionary<WorldStateType, int> _states;
        private readonly Subject<WorldStateType> _onStateChanged = new();

        ///<summary>
        ///値を取得する（未登録のキーは0）
        ///</summary>
        public int GetStateValue(WorldStateType key)
        {
            return _states.TryGetValue(key, out int value) ? value : 0;
        }

        ///<summary>
        ///値を上書きする（同じ値なら通知しない）
        ///</summary>
        public void ModifyState(WorldStateType key, int value)
        {
            if (_states.TryGetValue(key, out int current) && current == value) return;

            _states[key] = value;
            Debug.Log($"[WorldStates] {key} = {value}");
            _onStateChanged.OnNext(key);
        }

        ///<summary>
        ///値に加算する
        ///</summary>
        public void AddState(WorldStateType key, int value)
        {
            ModifyState(key, GetStateValue(key) + value);
        }

        ///<summary>
        ///破棄する
        ///</summary>
        public void Dispose()
        {
            _onStateChanged.Dispose();
        }
    }
}
