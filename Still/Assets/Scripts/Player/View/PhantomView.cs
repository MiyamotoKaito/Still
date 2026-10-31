using System.Linq;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///幻覚のゴースト。見た目だけの複製で、当たり判定もAIも持たない
    ///</summary>
    public class PhantomView : MonoBehaviour
    {
        ///<summary>表示中か</summary>
        public bool IsVisible => _phantom != null && _phantom.activeSelf;
        ///<summary>幻覚の位置</summary>
        public Vector3 Position => _phantom != null ? _phantom.transform.position : transform.position;

        [SerializeField, Tooltip("見た目に使うゴーストのプレハブ（AIや当たり判定は起動時に取り除く）")]
        private GameObject _phantomPrefab;

        private GameObject _phantom;

        ///<summary>
        ///指定位置に、指定方向を向いて現れる
        ///</summary>
        public void Show(Vector3 position, Vector3 lookAt)
        {
            if (_phantom == null) return;

            var direction = lookAt - position;
            direction.y = 0f;
            _phantom.transform.SetPositionAndRotation(position, direction.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(direction) : Quaternion.identity);
            _phantom.SetActive(true);
        }

        ///<summary>
        ///消える
        ///</summary>
        public void Hide()
        {
            if (_phantom != null) _phantom.SetActive(false);
        }

        private void Awake()
        {
            if (_phantomPrefab == null) return;

            // 本物のゴーストのスクリプトが起動しないよう、非アクティブな親の下で複製してから見た目以外を取り除く
            var holder = new GameObject("PhantomHolder");
            holder.SetActive(false);
            _phantom = Instantiate(_phantomPrefab, holder.transform);
            StripToVisualOnly(_phantom);
            _phantom.transform.SetParent(transform, false);
            _phantom.name = "Phantom";
            _phantom.SetActive(false);
            Destroy(holder);
        }

        private static void StripToVisualOnly(GameObject go)
        {
            // RequireComponentの依存があるので、スクリプト → 物理・AI の順に消す
            foreach (var behaviour in go.GetComponentsInChildren<MonoBehaviour>(true)) DestroyImmediate(behaviour);
            var removable = go.GetComponentsInChildren<Component>(true)
                .Where(c => !(c is Transform) && !(c is Renderer) && !(c is MeshFilter) && !(c is Animator))
                .ToList();
            foreach (var component in removable) DestroyImmediate(component);
        }
    }
}
