using System.Threading;
using Cysharp.Threading.Tasks;

namespace Still.Common
{
    ///<summary>
    ///Additiveシーンのロード／アンロード
    ///</summary>
    public interface ISceneLoader
    {
        ///<summary>
        ///シーンをAdditiveでロードし、アクティブシーンにする
        ///</summary>
        UniTask LoadAsync(string sceneName, CancellationToken ct);

        ///<summary>
        ///ロード済みのシーンをアンロードする
        ///</summary>
        UniTask UnloadAsync(string sceneName, CancellationToken ct);
    }
}
