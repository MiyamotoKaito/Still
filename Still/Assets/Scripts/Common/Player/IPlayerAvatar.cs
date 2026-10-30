using UnityEngine;

namespace Still.Common
{
    ///<summary>
    ///他モジュールから見たプレイヤー
    ///</summary>
    public interface IPlayerAvatar
    {
        ///<summary>プレイヤーのTransform</summary>
        Transform Transform { get; }

        ///<summary>
        ///指定位置へ瞬間移動する
        ///</summary>
        void Warp(Vector3 position);

        ///<summary>
        ///プレイヤーの入力を止める
        ///</summary>
        void LockInput();

        ///<summary>
        ///プレイヤーの入力を再開する
        ///</summary>
        void UnlockInput();
    }
}
