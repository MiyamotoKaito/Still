using Still.Common;
using Still.Utility;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Still.Title
{
    ///<summary>
    ///タイトル画面の登録。Titleスコープで使う
    ///</summary>
    [CreateAssetMenu(fileName = "TitleInstaller", menuName = "Installer/Title")]
    public sealed class TitleInstaller : InstallerObject
    {
        ///<summary>
        ///登録する
        ///</summary>
        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<TitleView>();
            builder.RegisterSceneComponents<OpendDoor>();
            builder.RegisterEntryPoint<TitleController>();
        }
    }
}
