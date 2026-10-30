using Still.Common;
using Still.Utility;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Still.Escape
{
    ///<summary>
    ///脱出要素（調べるもの・鍵・扉・照明・部屋）の登録。InGameスコープで使う
    ///</summary>
    [CreateAssetMenu(fileName = "EscapeInstaller", menuName = "Installer/Escape")]
    public sealed class EscapeInstaller : InstallerObject
    {
        [SerializeField, Tooltip("鍵が出るまでに調べる数")]
        private int _requiredItemCount = 3;

        ///<summary>
        ///登録する
        ///</summary>
        public override void Install(IContainerBuilder builder)
        {
            builder.Register(_ => new EscapeEntity(_requiredItemCount), Lifetime.Singleton);
            builder.Register<EscapeUsecase>(Lifetime.Singleton);
            builder.Register<LightSwitchUsecase>(Lifetime.Singleton).AsSelf().As<ILightSwitchUsecase>();
            builder.Register<RoomQuery>(Lifetime.Singleton).As<IRoomQuery>();

            builder.RegisterComponentInHierarchy<Tv>();
            builder.RegisterComponentInHierarchy<RecorderPlayer>();
            builder.RegisterComponentInHierarchy<Book>();
            builder.RegisterComponentInHierarchy<KeyView>();
            builder.RegisterComponentInHierarchy<LockedDoor>();
            builder.RegisterComponentInHierarchy<EscapeChecklistView>();
            builder.RegisterSceneComponents<DoorView>();
            builder.RegisterSceneComponents<LightSwitch>();
            builder.RegisterSceneComponents<Room>();

            builder.RegisterEntryPoint<EscapePresenter>();
            builder.RegisterEntryPoint<DoorPresenter>();
            builder.RegisterEntryPoint<RoomPresenter>();
            builder.RegisterEntryPoint<LightSwitchPresenter>();
        }
    }
}
