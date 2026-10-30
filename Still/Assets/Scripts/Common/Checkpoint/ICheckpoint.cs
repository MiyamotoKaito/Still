using UnityEngine;

namespace Still.Common
{
    ///<summary>
    ///捕まったときに戻る位置の保存
    ///</summary>
    public interface ICheckpoint
    {
        ///<summary>
        ///位置を保存する
        ///</summary>
        void Save(Vector3 position);

        ///<summary>
        ///保存した位置を取得する
        ///</summary>
        Vector3 Load();
    }
}
