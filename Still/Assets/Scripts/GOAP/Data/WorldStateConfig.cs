using System;
using System.Collections.Generic;
using Still.Common;
using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///ワールドステートの初期値
    ///</summary>
    [CreateAssetMenu(fileName = "WorldStateConfig", menuName = "Config/GOAP/InitialWorldState")]
    public class WorldStateConfig : ScriptableObject
    {
        [Serializable]
        public struct StateEntry
        {
            public WorldStateType Key;
            public int Value;
        }

        public List<StateEntry> InitialStates;

        ///<summary>
        ///初期値を辞書で取得する
        ///</summary>
        public Dictionary<WorldStateType, int> GetStates()
        {
            var states = new Dictionary<WorldStateType, int>();
            foreach (var state in InitialStates)
            {
                states[state.Key] = state.Value;
            }
            return states;
        }

        ///<summary>
        ///未登録のワールドステートを値0で追加する
        ///</summary>
        [ContextMenu("全てのワールドステートを設定")]
        public void SetAllState()
        {
            foreach (WorldStateType type in Enum.GetValues(typeof(WorldStateType)))
            {
                if (InitialStates.Exists(s => s.Key == type)) continue;
                InitialStates.Add(new StateEntry { Key = type, Value = 0 });
            }
        }
    }
}
