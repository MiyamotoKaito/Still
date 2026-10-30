using System;
using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///その場で一定時間見回す
    ///</summary>
    [Serializable]
    public class LookAroundAction : ActionBase
    {
        private float _remaining;

        ///<summary>
        ///時間が経ったら完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            _remaining -= Time.deltaTime;
            return _remaining <= 0;
        }

        ///<summary>
        ///見回す時間を設定する
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            _remaining = agent.Config.Duration;
        }
    }
}
