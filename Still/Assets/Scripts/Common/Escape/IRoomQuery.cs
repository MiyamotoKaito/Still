using UnityEngine;

namespace Still.Common
{
    ///<summary>
    ///プレイヤーがいる部屋の問い合わせ
    ///</summary>
    public interface IRoomQuery
    {
        ///<summary>
        ///プレイヤーがいる部屋のドアを取得する
        ///</summary>
        bool TryGetOccupiedRoomDoor(out Transform door);
    }
}
