using System.Linq;
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
        [SerializeField] private FearConfig _fearConfig;

        ///<summary>
        ///登録する
        ///</summary>
        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_config);
            builder.RegisterInstance(_fearConfig);

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
            // 2階用・恐怖演出用のVolumeもシーンにあるので、被写界深度を持つ全体用のVolumeを明示して選ぶ
            builder.Register(_ => Object.FindObjectsByType<Volume>(FindObjectsInactive.Include)
                .First(v => v.isGlobal && v.GetComponent<FearPostProcessView>() == null), Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<FearPostProcessView>();
            builder.RegisterComponentInHierarchy<FearAudioView>();
            builder.RegisterComponentInHierarchy<PhantomView>();
            builder.Register<FearEffectContext>(Lifetime.Singleton);

            builder.RegisterEntryPoint<PlayerMoveController>();
            builder.RegisterEntryPoint<GameOverController>();
            builder.RegisterEntryPoint<PlayerStatusPresenter>();
            builder.RegisterEntryPoint<PlayerFearLevelPresenter>();
            builder.RegisterEntryPoint<PlayerSensorPresenter>();
            builder.RegisterEntryPoint<DepthOfFieldPresenter>();
            builder.RegisterEntryPoint<PlayerFovPresenter>();
            builder.RegisterEntryPoint<PlayerGhostDetectedPresenter>();
            builder.RegisterEntryPoint<PlayerSpotLightPresenter>();
            builder.RegisterEntryPoint<FearController>();
        }
    }
}
