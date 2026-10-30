using System;
using System.Collections.Generic;
using Still.Common;
using Still.Utility;
using VContainer.Unity;

namespace Still.Escape
{
    ///<summary>
    ///プレイヤーが部屋にいるかをワールドステートへ伝える
    ///</summary>
    public sealed class RoomPresenter : IInitializable, IDisposable
    {
        public RoomPresenter(SceneComponentList<Room> rooms, IWorldStates worldStates)
        {
            _rooms = rooms.Items;
            _worldStates = worldStates;
        }

        private readonly IReadOnlyList<Room> _rooms;
        private readonly IWorldStates _worldStates;

        ///<summary>
        ///購読を始める
        ///</summary>
        public void Initialize()
        {
            foreach (var room in _rooms)
            {
                room.OnPlayerEntered += HandleEntered;
                room.OnPlayerExited += HandleExited;
            }
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose()
        {
            foreach (var room in _rooms)
            {
                if (room == null) continue;
                room.OnPlayerEntered -= HandleEntered;
                room.OnPlayerExited -= HandleExited;
            }
        }

        private void HandleEntered() => _worldStates.ModifyState(WorldStateType.InTheRoom, 1);

        private void HandleExited() => _worldStates.ModifyState(WorldStateType.InTheRoom, 0);
    }
}
