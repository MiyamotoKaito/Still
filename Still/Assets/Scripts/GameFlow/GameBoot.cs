using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Still.Common;
using UnityEngine;
using VContainer.Unity;

namespace Still.GameFlow
{
    ///<summary>
    ///Rootスコープ生成後にStateを登録し、最初のモードへ遷移する
    ///</summary>
    public sealed class GameBoot : IStartable, IDisposable
    {
        ///<summary>エディタで再生を始めたシーン名を受け渡すSessionStateのキー</summary>
        public const string EditorStartSceneKey = "Still.EditorStartScene";

        public GameBoot(IStateMachine stateMachine, ISceneLoader sceneLoader, string titleSceneName, string inGameSceneName)
        {
            _stateMachine = stateMachine;
            _states = new[]
            {
                new SceneModeState(GameModeType.Title, titleSceneName, CursorLockMode.None, sceneLoader),
                new SceneModeState(GameModeType.InGame, inGameSceneName, CursorLockMode.Locked, sceneLoader),
            };
        }

        private readonly IStateMachine _stateMachine;
        private readonly IReadOnlyList<SceneModeState> _states;

        ///<summary>
        ///Stateを登録して最初のモードへ遷移する
        ///</summary>
        public void Start()
        {
            foreach (var state in _states) _stateMachine.AddState(state);
            _stateMachine.ChangeStateAsync(ResolveInitialMode()).Forget();
        }

        ///<summary>
        ///Stateの登録を解除する
        ///</summary>
        public void Dispose()
        {
            foreach (var state in _states) _stateMachine.RemoveState(state);
        }

        private GameModeType ResolveInitialMode()
        {
#if UNITY_EDITOR
            // エディタではRootから起動し直すので、開いていたシーンのモードから始める
            var startScene = UnityEditor.SessionState.GetString(EditorStartSceneKey, string.Empty);
            var state = _states.FirstOrDefault(s => s.SceneName == startScene);
            if (state != null) return state.GameModeType;
#endif
            return GameModeType.Title;
        }
    }
}
