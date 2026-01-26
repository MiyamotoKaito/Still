using System.Collections.Generic;
using UnityEngine;
namespace Still.GOAP.Action.Config
{
    [CreateAssetMenu(fileName = "EnemyActionConfig", menuName = "Config/GOAP/Actions")]
    public class GhostActionData : ScriptableObject
    {
        public List<IAction> GhostActions => _ghostActions;
        [SerializeReference, SubclassSelector]
        private List<IAction> _ghostActions;
    }
}