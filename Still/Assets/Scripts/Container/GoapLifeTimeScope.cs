using Still.GOAP.Action;
using Still.GOAP.Action.Config;
using Still.GOAP.Agent;
using Still.GOAP.Agent.Config;
using Still.GOAP.Goal.Config;
using Still.GOAP.Planner.Executor;
using Still.GOAP.WorldState;
using Still.GOAP.WorldState.Config;
using Still.GOAP.WorldState.Observer;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GoapLifeTimeScope : LifetimeScope
{
    [SerializeField] private GhostConfig _agentConfig;
    [SerializeField] private GhostActionData _actionData;
    [SerializeField] private WorldStateConfig _worldStatesConfig;
    [SerializeField] private List<SubGoalConfig> _subgoals;

    protected override void Configure(IContainerBuilder builder)
    {
        // 1. 各種データの登録
        builder.RegisterInstance(_agentConfig);
        builder.RegisterInstance<List<IAction>>(_actionData.GhostActions);
        builder.RegisterInstance<List<SubGoalConfig>>(_subgoals);
        builder.RegisterInstance(_worldStatesConfig).AsSelf();

        // 2. ロジッククラスの登録
        builder.Register<WorldStates>(Lifetime.Singleton).AsSelf();
        builder.Register<WorldStatesObserver>(Lifetime.Singleton).AsSelf();
        builder.Register<GoapExecutor>(Lifetime.Singleton).AsSelf();
        // 3. シーン上のAgentの登録
        builder.RegisterComponentInHierarchy<GAgent>();

        // 4. ビルド直後のコールバック（ここが重要！）
        builder.RegisterBuildCallback(container =>
        {
            // 【追加】ここで強制的に生成（コンストラクタを呼ぶ）
            container.Resolve<WorldStatesObserver>();
            var switches = GameObject.FindObjectsByType<LightSwitch>(FindObjectsSortMode.None);
            foreach (var s in switches)
            {
                // VContainerに「このインスタンスに[Inject]して！」と直接命令する
                container.Inject(s);
                // IInitializableの代わりに手動で初期化を呼ぶ
                s.Init();
            }

            // Actionへの注入
            foreach (var action in _actionData.GhostActions)
            {
                container.Inject(action);
            }
            var executor = container.Resolve<GoapExecutor>();
            executor.RefreshGoal();
            Debug.Log("VContainer Build完了");
        });
    }
}