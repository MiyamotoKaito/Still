using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using UnityEngine.SceneManagement;

namespace Still.Utility
{
    ///<summary>
    ///Additiveシーンのロード／アンロード
    ///</summary>
    public sealed class SceneLoader : ISceneLoader
    {
        ///<summary>
        ///シーンをAdditiveでロードし、アクティブシーンにする
        ///</summary>
        public async UniTask LoadAsync(string sceneName, CancellationToken ct)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask(cancellationToken: ct);
            // ライティングや環境設定はアクティブシーンのものが使われるため
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        ///<summary>
        ///ロード済みのシーンをアンロードする
        ///</summary>
        public async UniTask UnloadAsync(string sceneName, CancellationToken ct)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded) return;

            await SceneManager.UnloadSceneAsync(scene).ToUniTask(cancellationToken: ct);
        }
    }
}
