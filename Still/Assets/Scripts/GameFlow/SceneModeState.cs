using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using UnityEngine;

namespace Still.GameFlow
{
    ///<summary>
    ///1つのシーンをロードしている間だけ続くモード
    ///</summary>
    public sealed class SceneModeState : IState
    {
        public SceneModeState(GameModeType gameModeType, string sceneName, CursorLockMode cursorLockMode, ISceneLoader sceneLoader)
        {
            GameModeType = gameModeType;
            SceneName = sceneName;
            _cursorLockMode = cursorLockMode;
            _sceneLoader = sceneLoader;
        }

        ///<summary>このStateが表すモード</summary>
        public GameModeType GameModeType { get; }
        ///<summary>このモードでロードするシーン</summary>
        public string SceneName { get; }

        private readonly CursorLockMode _cursorLockMode;
        private readonly ISceneLoader _sceneLoader;

        ///<summary>
        ///シーンをロードする
        ///</summary>
        public async UniTask EnterAsync(CancellationToken ct)
        {
            await _sceneLoader.LoadAsync(SceneName, ct);
            Cursor.lockState = _cursorLockMode;
        }

        ///<summary>
        ///シーンをアンロードする
        ///</summary>
        public async UniTask ExitAsync(CancellationToken ct)
        {
            await _sceneLoader.UnloadAsync(SceneName, ct);
        }
    }
}
