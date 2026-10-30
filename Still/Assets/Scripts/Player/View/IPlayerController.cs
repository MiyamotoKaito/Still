using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///移動処理から見たプレイヤーの入力と体
    ///</summary>
    public interface IPlayerController
    {
        ///<summary>移動入力</summary>
        Vector2 CurrentMoveValue { get; }
        ///<summary>ダッシュボタンを押しているか</summary>
        bool IsDash { get; }

        ///<summary>
        ///指定方向へ移動させる
        ///</summary>
        void Move(Vector3 direction, float speed);
    }
}
