using System.Collections.Generic;
using Still.Common;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストAI（GOAP）の登録。InGameスコープで使う
    ///</summary>
    [CreateAssetMenu(fileName = "GoapInstaller", menuName = "Installer/Goap")]
    public sealed class GoapInstaller : InstallerObject
    {
        [SerializeField] private GhostConfig _ghostConfig;
        [SerializeField] private GhostActionData _actionData;
        [SerializeField] private WorldStateConfig _worldStateConfig;
        [SerializeField] private List<SubGoalConfig> _subGoals = new();

        ///<summary>
        ///登録する
        ///</summary>
        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_ghostConfig);
            builder.RegisterInstance(_worldStateConfig);
            builder.RegisterInstance(_actionData);
            // IReadOnlyList<T>で登録するとVContainerのコレクション解決と衝突するので具象のListで登録する
            builder.RegisterInstance(_subGoals);

            builder.Register<WorldStates>(Lifetime.Singleton).As<IWorldStates>();
            builder.RegisterEntryPoint<GoapExecutor>().AsSelf();
            builder.RegisterEntryPoint<WorldStatesObserver>();
            builder.RegisterEntryPoint<GhostSightTicker>();
            builder.RegisterEntryPoint<KnockTimer>();
            builder.RegisterEntryPoint<GhostFloorFollower>();

            builder.RegisterComponentInHierarchy<GAgent>().As<IAgentController>();
            builder.RegisterComponentInHierarchy<GhostSensorView>();
            builder.RegisterComponentInHierarchy<TeleportPoints>();

            // アクションはSOが保持するSerializeReferenceの値なので、コンテナ生成後に依存を注入する
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var action in _actionData.GhostActions) resolver.Inject(action);
            });
        }
    }
}
