using Still.GameFlow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Still.Container.Editor
{
    ///<summary>
    ///どのシーンを開いていても、再生はRootシーンから始める
    ///</summary>
    // 子シーンのスコープは親（Root）が先にロードされている必要があるため。
    // 開いていたシーンはSessionState経由でGameBootに伝え、そのモードから始めさせる
    [InitializeOnLoad]
    internal static class PlayModeStartScene
    {
        private const string RootScenePath = "Assets/Scenes/Root.unity";

        static PlayModeStartScene()
        {
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(RootScenePath);
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingEditMode) return;

            var activeScene = SceneManager.GetActiveScene();
            var startScene = activeScene.path == RootScenePath ? string.Empty : activeScene.name;
            SessionState.SetString(GameBoot.EditorStartSceneKey, startScene);
        }
    }
}
