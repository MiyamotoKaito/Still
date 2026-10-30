using System.Collections.Generic;
using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストのテレポート先。子オブジェクトの位置をそのまま使う
    ///</summary>
    public class TeleportPoints : MonoBehaviour
    {
        ///<summary>テレポート先の一覧</summary>
        public IReadOnlyList<Transform> Points => _points;

        private Transform[] _points;

        private void Awake()
        {
            _points = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                _points[i] = transform.GetChild(i);
            }
        }
    }
}
