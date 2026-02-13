using System;
using System.Collections.Generic;
using Still.GOAP.WorldState.Config;
using UniRx;
using UnityEngine;
namespace Still.GOAP.WorldState
{
    public class WorldStates
    {
        /// <summary>ステートの値が変わった時に発火されるイベント</summary>
        public IObservable<string> OnStateChanged => _onStateChanged;
        public Dictionary<string, int> CurrentStates => _currentWorldStates;

        private readonly Subject<string> _onStateChanged = new();
        private readonly Dictionary<string, int> _currentWorldStates = new();


        public WorldStates(WorldStateConfig config)
        {
            foreach (var state in config.GetStates())
            {
                _currentWorldStates.Add(state.Key, state.Value);
            }
        }
        /// <summary>
        /// ステートの値を書き換える
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ModifyState(string key, int value)
        {
            if (value == _currentWorldStates[key]) return;
            _currentWorldStates[key] = value;
            _onStateChanged.OnNext(key);
            Debug.Log($"{key}の値が変更された : 現在{_currentWorldStates[key]}");
        }
        /// <summary>
        /// ステートの値を加算する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AdditionState(string key, int value)
        {
            _currentWorldStates[key] += value;
            _onStateChanged.OnNext(key);
            Debug.Log($"{key}の値が書き変わった : 現在{_currentWorldStates[key]}");
        }

        public int GetStateValue(string key) => _currentWorldStates[key];
    }
}