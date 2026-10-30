using Still.Common;
using UnityEngine;

namespace Still.Utility
{
    ///<summary>
    ///チェックポイントをPlayerPrefsに保存する
    ///</summary>
    public sealed class PlayerPrefsCheckpoint : ICheckpoint
    {
        private const string KeyX = "LastPositionX";
        private const string KeyY = "LastPositionY";
        private const string KeyZ = "LastPositionZ";

        ///<summary>
        ///位置を保存する
        ///</summary>
        public void Save(Vector3 position)
        {
            PlayerPrefs.SetFloat(KeyX, position.x);
            PlayerPrefs.SetFloat(KeyY, position.y);
            PlayerPrefs.SetFloat(KeyZ, position.z);
            PlayerPrefs.Save();
        }

        ///<summary>
        ///保存した位置を取得する
        ///</summary>
        public Vector3 Load()
        {
            return new Vector3(
                PlayerPrefs.GetFloat(KeyX, 0f),
                PlayerPrefs.GetFloat(KeyY, 0f),
                PlayerPrefs.GetFloat(KeyZ, 0f));
        }
    }
}
