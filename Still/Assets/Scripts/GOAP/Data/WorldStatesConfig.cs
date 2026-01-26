using Still.Enum.WorldStates;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Still.GOAP.WorldState.Config
{
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

        //Dictionaryに変換して取り出す
        public Dictionary<string, int> GetStates()
        {
            var initializedWorldStatesDic = new Dictionary<string, int>();
            foreach (var state in InitialStates)
            {
                initializedWorldStatesDic.Add(state.Key.ToString(), state.Value);
            }
            return initializedWorldStatesDic;
        }
        [ContextMenu("全てのワールドステートを設定")]
        public void SetAllState()
        {
            foreach (var type in System.Enum.GetValues(typeof(WorldStateType)))
            {
                var worldStateType = (WorldStateType)type;
                if (InitialStates.Exists(s => s.Key == worldStateType)) continue;
                InitialStates.Add(new StateEntry { Key = worldStateType, Value = 0 });
            }
        }
    }
}