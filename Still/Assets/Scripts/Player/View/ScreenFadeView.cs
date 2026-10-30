using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Still.Player
{
    ///<summary>
    ///画面全体を覆う黒いImageで暗転する（フェード用のPanelに付ける）
    ///</summary>
    [RequireComponent(typeof(Image))]
    public class ScreenFadeView : MonoBehaviour, IScreenFader
    {
        // Panelは非アクティブで置かれるためAwakeを待たずに取得する
        private Image Image => _image != null ? _image : _image = GetComponent<Image>();

        private Image _image;

        ///<summary>
        ///即座に真っ暗にする
        ///</summary>
        public void SetBlack()
        {
            SetAlpha(1f);
        }

        ///<summary>
        ///指定秒数で暗転する
        ///</summary>
        public UniTask FadeOutAsync(float duration, CancellationToken ct)
        {
            return FadeAsync(1f, duration, ct);
        }

        ///<summary>
        ///指定秒数で暗転を解除する
        ///</summary>
        public async UniTask FadeInAsync(float duration, CancellationToken ct)
        {
            await FadeAsync(0f, duration, ct);
            gameObject.SetActive(false);
        }

        private async UniTask FadeAsync(float to, float duration, CancellationToken ct)
        {
            // 呼び出し側がトークンを持たない場合でも、シーン破棄で止まるようにする
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);

            float from = gameObject.activeSelf ? Image.color.a : 0f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
                await UniTask.Yield(linked.Token);
            }
            SetAlpha(to);
        }

        private void SetAlpha(float alpha)
        {
            gameObject.SetActive(true);
            var color = Image.color;
            color.a = alpha;
            Image.color = color;
        }
    }
}
