using System.Collections.Generic;
using Still.Common;
using UnityEngine;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///アクションの基底。前提条件と効果はインスペクタで設定する
    ///</summary>
    [System.Serializable]
    public abstract class ActionBase : IAction
    {
        ///<summary>実行に必要な条件</summary>
        public IReadOnlyDictionary<WorldStateType, Condition> Preconditions => _preconditionDict ??= ActionSettingConverter.ToDictionary(_preconditions);
        ///<summary>完了時にワールドステートへ書き込む値</summary>
        public IReadOnlyDictionary<WorldStateType, int> Effects => _effectDict ??= ActionSettingConverter.ToDictionary(_effects);
        ///<summary>プランニング時のコスト</summary>
        public int ActionCost => _actionCost;

        protected IWorldStates WorldStates;

        [Header("設定")]
        [SerializeField] private List<HasConditionActions> _preconditions;
        [SerializeField] private List<NoConditionActions> _effects;
        [SerializeField] private int _actionCost;

        private Dictionary<WorldStateType, Condition> _preconditionDict;
        private Dictionary<WorldStateType, int> _effectDict;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectWorldStates(IWorldStates worldStates)
        {
            WorldStates = worldStates;
        }

        ///<summary>
        ///実行中に前提条件が維持されているか確認する
        ///</summary>
        public virtual bool Execute(IAgentController agent)
        {
            return Evaluation.IsSatisfied(WorldStates.CurrentStates, Preconditions);
        }

        ///<summary>
        ///毎フレーム呼ばれる。完了したら true を返す
        ///</summary>
        public abstract bool Perform(IAgentController agent);

        ///<summary>
        ///アクション開始時に一度だけ呼ばれる
        ///</summary>
        public abstract void SetTarget(IAgentController agent);

        public override string ToString() => GetType().Name;
    }
}
