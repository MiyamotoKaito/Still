using Still.Common;
using Still.Utility;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Still.Escape
{
    ///<summary>
    ///脱出要素（ミッション・調べる物・扉・照明・部屋）の登録。InGameスコープで使う
    ///</summary>
    [CreateAssetMenu(fileName = "EscapeInstaller", menuName = "Installer/Escape")]
    public sealed class EscapeInstaller : InstallerObject
    {
        [SerializeField] private MissionConfig _missionConfig;

        ///<summary>
        ///登録する
        ///</summary>
        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_missionConfig);
            builder.Register<MissionEntity>(Lifetime.Singleton);
            builder.Register<MissionUsecase>(Lifetime.Singleton);
            builder.Register<LightSwitchUsecase>(Lifetime.Singleton).AsSelf().As<ILightSwitchUsecase>();
            builder.Register<RoomQuery>(Lifetime.Singleton).As<IRoomQuery>();

            builder.RegisterComponentInHierarchy<EscapeChecklistView>();
            builder.RegisterSceneComponents<MissionTargetView>();
            builder.RegisterSceneComponents<LockedDoor>();
            builder.RegisterSceneComponents<DoorView>();
            builder.RegisterSceneComponents<LightSwitch>();
            builder.RegisterSceneComponents<Room>();

            builder.RegisterEntryPoint<MissionPresenter>();
            builder.RegisterEntryPoint<MissionTicker>();
            builder.RegisterEntryPoint<DoorPresenter>();
            builder.RegisterEntryPoint<RoomPresenter>();
            builder.RegisterEntryPoint<LightSwitchPresenter>();
        }
    }
}
