using Still.Enum.CompareType;
using Still.Enum.WorldStates;
using UnityEngine;
namespace Still.GOAP.Action.Setting
{
    [System.Serializable]
    public class HasConditionActions
    {
        public string WorldStateKey => _key.ToString();
        public int Value => _value;
        public CompareType CompareType => _compareType;

        [SerializeField] private WorldStateType _key;
        [SerializeField] private int _value;
        [SerializeField] private CompareType _compareType;
    }
    [System.Serializable]
    public class NoConditionActions
    {
        public string WorldStateKey => _key.ToString();
        public int Value => _value;

        [SerializeField] private WorldStateType _key;
        [SerializeField] private int _value;
    }
}
