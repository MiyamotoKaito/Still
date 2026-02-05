using System.Collections.Generic;
using Still.GOAP.Action.Setting;
using Still.GOAP.Agent;
using Still.GOAP.Planner;
using Still.GOAP.WorldState;
using Still.Player;
using UnityEngine;
using VContainer;
namespace Still.GOAP.Action
{
    public abstract class ActionBase : IAction
    {
        public Dictionary<string, Condition> Preconditions
        {
            get
            {
                if (_pDic == null)
                    _pDic ??= ToDictionary(_preconditions);

                return _pDic;
            }
        }

        public Dictionary<string, int> Effects
        {
            get
            {
                if (_eDic == null)
                    _eDic ??= ToDictionary(_effects);

                return _eDic;
            }
        }

        public int ActionCost => _actionCost;

        public virtual bool Execute(IAgentController agent)
        {
            if (Evalution.IsSatisfied(_worldStates.CurrentStates, Preconditions))
            {
                return true;
            }
            return false;
        }

        public abstract bool Perform(IAgentController agent);

        public abstract void SetTarget(IAgentController agent);
        [Header("設定")]
        [SerializeField] private List<HasConditionActions> _preconditions;
        [SerializeField] private List<NoConditionActions> _effects;
        [SerializeField] private int _actionCost;

        private Dictionary<string, Condition> _pDic;
        private Dictionary<string, int> _eDic;
        protected WorldStates _worldStates;
        protected PlayerManager _playerManager;

        [Inject]
        public void Init(WorldStates worldState)
        {
            _worldStates = worldState;
            _playerManager = GameObject.FindAnyObjectByType<PlayerManager>();
        }
        public Dictionary<string, Condition> ToDictionary(List<HasConditionActions> list)
        {
            var dic = new Dictionary<string, Condition>();
            foreach (var item in list)
            {
                if (!dic.ContainsKey(item.WorldStateKey))
                {
                    dic.Add(item.WorldStateKey, item.Value);
                }
            }
            return dic;
        }
        public Dictionary<string, int> ToDictionary(List<NoConditionActions> list)
        {
            var dic = new Dictionary<string, int>();
            foreach (var item in list)
            {
                if (!dic.ContainsKey(item.WorldStateKey))
                {
                    dic.Add(item.WorldStateKey, item.Value);
                }
            }
            return dic;
        }
    }
}