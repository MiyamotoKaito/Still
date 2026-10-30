using Still.Common;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーの登録。InGameスコープで使う
    ///</summary>
    [CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installer/Player")]
    public sealed class PlayerInstaller : InstallerObject
    {
        [SerializeField] private PlayerConfig _config;

        ///<summary>
        ///登録する
        ///</summary>
        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_config);

            builder.Register<PlayerStatusEntity>(Lifetime.Singleton);
            builder.Register<FieldOfViewEntity>(Lifetime.Singleton);
            builder.Register<SpotLightEntity>(Lifetime.Singleton);
            builder.Register<PlayerStatusUsecase>(Lifetime.Singleton).AsSelf().As<ISanUsecase>();

            builder.RegisterComponentInHierarchy<PlayerView>().AsSelf().As<IPlayerController>().As<IPlayerAvatar>();
            builder.RegisterComponentInHierarchy<CameraView>();
            builder.RegisterComponentInHierarchy<PlayerSensorView>();
            builder.RegisterComponentInHierarchy<SANView>();
            builder.RegisterComponentInHierarchy<StaminaView>();
            builder.RegisterComponentInHierarchy<ScreenFadeView>().As<IScreenFader>();
            builder.RegisterComponentInHierarchy<Volume>();

            builder.RegisterEntryPoint<PlayerMoveController>();
            builder.RegisterEntryPoint<GameOverController>();
            builder.RegisterEntryPoint<PlayerStatusPresenter>();
            builder.RegisterEntryPoint<PlayerFearLevelPresenter>();
            builder.RegisterEntryPoint<PlayerSensorPresenter>();
            builder.RegisterEntryPoint<DepthOfFieldPresenter>();
            builder.RegisterEntryPoint<PlayerFovPresenter>();
            builder.RegisterEntryPoint<PlayerGhostDetectedPresenter>();
            builder.RegisterEntryPoint<PlayerSpotLightPresenter>();
        }
    }
}
