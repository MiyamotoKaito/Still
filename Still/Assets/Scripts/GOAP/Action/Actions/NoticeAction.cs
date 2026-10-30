using System;
using Still.Common;
using UnityEngine;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///見つけてもすぐには追わず、無言でプレイヤーを見つめる
    ///</summary>
    // 「見つかったかどうか分からない時間」を作るため。見つめ切ったら Noticed を立てて ChaseAction へつなぐ
    [Serializable]
    public class NoticeAction : ActionBase
    {
        private GhostSensorView _sensor;
        private IPlayerAvatar _player;
        private float _remaining;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectDependencies(GhostSensorView sensor, IPlayerAvatar player)
        {
            _sensor = sensor;
            _player = player;
        }

        ///<summary>
        ///見つめている間にプレイヤーが視界から外れたら、追わずにその場で見回す
        ///</summary>
        public override bool Execute(IAgentController agent)
        {
            // PlayerVisible は見失ってからしばらく1のままなので、視線が通っているかは毎フレーム直接確かめる
            if (base.Execute(agent) && _sensor.CanSeePlayer(agent.Config.GhostFovLength)) return true;

            // 「やり過ごせた」体験にするため、追跡ではなく見回し（LookAroundだけのPatrolプラン）に戻す
            WorldStates.ModifyState(WorldStateType.PlayerVisible, 0);
            WorldStates.ModifyState(WorldStateType.EnemyPatrol, 1);
            WorldStates.ModifyState(WorldStateType.LookAround, 0);
            return false;
        }

        ///<summary>
        ///プレイヤーの方へゆっくり向き直り、決めた時間見つめたら完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            agent.LookAt(_player.Transform.position, agent.Config.NoticeTurnSpeed);
            _remaining -= Time.deltaTime;
            return _remaining <= 0f;
        }

        ///<summary>
        ///その場で立ち止まる（音は鳴らさない）
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            agent.StopMove();
            _remaining = agent.Config.NoticeSeconds;
        }
    }
}
