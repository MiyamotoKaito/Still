using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Still.Utility
{
    ///<summary>
    ///シーン上に複数置かれた同じ種類のViewの一覧
    ///</summary>
    // IReadOnlyList<T>のまま登録するとVContainerのコレクション解決（同じ型の複数登録をまとめる機能）と衝突するため包む
    public sealed class SceneComponentList<T> where T : Component
    {
        public SceneComponentList(IReadOnlyList<T> items)
        {
            Items = items;
        }

        ///<summary>見つかったコンポーネント</summary>
        public IReadOnlyList<T> Items { get; }
    }

    ///<summary>
    ///SceneComponentListの登録
    ///</summary>
    public static class SceneComponentListExtensions
    {
        ///<summary>
        ///ロード済みシーンから非アクティブも含めてTを集め、一覧として登録する
        ///</summary>
        // Additive構成ではスコープのシーン以外もロードされているが、Stateの遷移で前のシーンを先にアンロードするので、
        // 対象のViewを持つのはそのスコープのシーンだけになる
        public static void RegisterSceneComponents<T>(this IContainerBuilder builder) where T : Component
        {
            builder.Register(_ => new SceneComponentList<T>(
                Object.FindObjectsByType<T>(FindObjectsInactive.Include)), Lifetime.Singleton);
        }
    }
}
