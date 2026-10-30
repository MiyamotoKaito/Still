using System.Collections.Generic;
using Template.Editor;
using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストが使えるアクションの一覧
    ///</summary>
    [CreateAssetMenu(fileName = "ActionConfig", menuName = "Config/GOAP/Actions")]
    public class GhostActionData : ScriptableObject
    {
        ///<summary>アクションの一覧</summary>
        public IReadOnlyList<IAction> GhostActions => _ghostActions;

        [SerializeReference, SubclassSelector]
        private List<IAction> _ghostActions;
    }
}
