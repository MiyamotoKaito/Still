using Still.Common;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Still.Utility
{
    ///<summary>
    ///横断インフラ（音・シーン・StateMachine・チェックポイント）の登録。Rootスコープで使う
    ///</summary>
    [CreateAssetMenu(fileName = "UtilityInstaller", menuName = "Installer/Utility")]
    public sealed class UtilityInstaller : InstallerObject
    {
        ///<summary>
        ///登録する
        ///</summary>
        public override void Install(IContainerBuilder builder)
        {
            builder.Register<SceneLoader>(Lifetime.Singleton).As<ISceneLoader>();
            builder.Register<StateMachine>(Lifetime.Singleton).As<IStateMachine>();
            builder.Register<PlayerPrefsCheckpoint>(Lifetime.Singleton).As<ICheckpoint>();
            builder.RegisterComponentInHierarchy<AudioManager>().As<IAudioService>();
        }
    }
}
