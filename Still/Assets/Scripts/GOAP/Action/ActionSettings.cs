using Still.Enum.WorldStates;
using UnityEngine;
namespace Still.GOAP.Action.Setting
{
    [System.Serializable]
    public class HasConditionActions
    {
        public string WorldStateKey => _key.ToString();
        public Condition Value => _value;

        [SerializeField] private WorldStateType _key;
        [SerializeField] private Condition _value;
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
