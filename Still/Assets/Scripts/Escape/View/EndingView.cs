using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace Still.Escape
{
    ///<summary>
    ///脱出後のエンディング（Timeline）を流す
    ///</summary>
    public class EndingView : MonoBehaviour
    {
        [SerializeField, Tooltip("エンディングのTimelineを再生するDirector（カメラ・字幕ごと非アクティブで置く）")]
        private PlayableDirector _director;
        [SerializeField, Tooltip("エンディング中は隠すもの（HUDや懐中電灯）")]
        private GameObject[] _hideDuringEnding;

        ///<summary>
        ///エンディングを最後まで流す
        ///</summary>
        public async UniTask PlayAsync(CancellationToken ct)
        {
            foreach (var target in _hideDuringEnding) target.SetActive(false);

            _director.gameObject.SetActive(true);
            // 終わると同時に止めるとTimelineで動かした値が元に戻り、最後の黒画面が一瞬消える。最後の状態で保持させる
            _director.extrapolationMode = DirectorWrapMode.Hold;
            _director.Play();

            await UniTask.WaitUntil(() => _director.time >= _director.duration, cancellationToken: ct);
        }
    }
}
