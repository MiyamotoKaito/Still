using System;
using Still.Common;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///プレイヤーを追いかける
    ///</summary>
    [Serializable]
    public class ChaseAction : ActionBase
    {
        private IAudioService _audio;
        private IPlayerAvatar _player;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectDependencies(IAudioService audio, IPlayerAvatar player)
        {
            _audio = audio;
            _player = player;
        }

        ///<summary>
        ///見失ったら追跡をやめて通常速度に戻す
        ///</summary>
        public override bool Execute(IAgentController agent)
        {
            if (base.Execute(agent)) return true;

            agent.SetSpeed(agent.Config.GhostMoveSpeed);
            _audio.Stop(agent.AudioSource);
            WorldStates.ModifyState(WorldStateType.IsChasing, 0);
            // 次に見つけたときも、まず見つめるところから始める
            WorldStates.ModifyState(WorldStateType.Noticed, 0);
            return false;
        }

        ///<summary>
        ///プレイヤーに追いついたら完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            if (agent.IsArrived)
            {
                _audio.Stop(agent.AudioSource);
                return true;
            }

            // 開始時の位置ではなく、逃げるプレイヤーの今の位置を追う
            agent.SetMoveDestination(_player.Transform.position);
            return false;
        }

        ///<summary>
        ///プレイヤーをターゲットにしてダッシュする
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            _audio.PlaySe("Chase", agent.AudioSource);
            agent.SetTarget(_player.Transform);
            agent.SetSpeed(agent.Config.GhostDashSpeed);
            agent.SetMoveDestination(_player.Transform.position);
        }
    }
}
