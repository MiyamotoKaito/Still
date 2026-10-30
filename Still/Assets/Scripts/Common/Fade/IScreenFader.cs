using System.Threading;
using Cysharp.Threading.Tasks;

namespace Still.Common
{
    ///<summary>
    ///画面全体の暗転
    ///</summary>
    public interface IScreenFader
    {
        ///<summary>
        ///即座に真っ暗にする
        ///</summary>
        void SetBlack();

        ///<summary>
        ///指定秒数で暗転する
        ///</summary>
        UniTask FadeOutAsync(float duration, CancellationToken ct);

        ///<summary>
        ///指定秒数で暗転を解除する
        ///</summary>
        UniTask FadeInAsync(float duration, CancellationToken ct);
    }
}
