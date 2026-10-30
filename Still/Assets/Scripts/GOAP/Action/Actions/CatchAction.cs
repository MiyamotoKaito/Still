using System;
using Still.Common;
using Template.Editor;
using UnityEngine;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///プレイヤーを捕まえる
    ///</summary>
    [Serializable]
    public class CatchAction : ActionBase
    {
        [SerializeReference, SubclassSelector]
        private IEvent _catchEvent;

        ///<summary>
        ///演出に依存を注入する
        ///</summary>
        [Inject]
        public void InjectEvent(IObjectResolver resolver)
        {
            // 演出はSerializeReferenceの値なのでコンテナが生成できず、ここで注入する
            if (_catchEvent != null) resolver.Inject(_catchEvent);
        }

        ///<summary>
        ///捕まえた演出を開始して即完了する
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            _catchEvent?.Play();
            return true;
        }

        ///<summary>
        ///開始時の処理はない
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
        }
    }
}
