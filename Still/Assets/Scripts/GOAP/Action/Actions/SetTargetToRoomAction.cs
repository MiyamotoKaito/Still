using System;
using Still.Common;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///プレイヤーがいる部屋のドアをターゲットにする
    ///</summary>
    [Serializable]
    public class SetTargetToRoomAction : ActionBase
    {
        private IRoomQuery _rooms;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectDependencies(IRoomQuery rooms)
        {
            _rooms = rooms;
        }

        ///<summary>
        ///即完了する（ターゲットが無ければ次のMoveActionが破綻して再計画される）
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            return true;
        }

        ///<summary>
        ///プレイヤーがいる部屋のドアを探す
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            agent.SetTarget(_rooms.TryGetOccupiedRoomDoor(out var door) ? door : null);
        }
    }
}
