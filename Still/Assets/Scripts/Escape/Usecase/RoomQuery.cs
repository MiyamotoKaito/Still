using System.Collections.Generic;
using Still.Common;
using Still.Utility;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///プレイヤーがいる部屋を探す
    ///</summary>
    public sealed class RoomQuery : IRoomQuery
    {
        public RoomQuery(SceneComponentList<Room> rooms)
        {
            _rooms = rooms.Items;
        }

        private readonly IReadOnlyList<Room> _rooms;

        ///<summary>
        ///プレイヤーがいる部屋のドアを取得する
        ///</summary>
        public bool TryGetOccupiedRoomDoor(out Transform door)
        {
            foreach (var room in _rooms)
            {
                if (!room.IsPlayerInside) continue;
                door = room.Door;
                return true;
            }
            door = null;
            return false;
        }
    }
}
