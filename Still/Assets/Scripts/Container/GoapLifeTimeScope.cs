using System.Collections.Generic;
using Still.GOAP.Action;
using Still.GOAP.Action.Config;
using Still.GOAP.Agent.Config;
using Still.GOAP.Goal.Config;
using Still.GOAP.Planner.Executor;
using Still.GOAP.WorldState;
using Still.GOAP.WorldState.Config;
using Still.GOAP.WorldState.Observer;
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
        builder.RegisterInstance(_agentConfig);
        builder.RegisterInstance<List<IAction>>(_actionData.GhostActions);
        builder.RegisterInstance<List<SubGoalConfig>>(_subgoals);
        
        builder.Register<WorldStates>(Lifetime.Singleton)
            .WithParameter("initializeStates", _worldStatesConfig.GetStates());
        builder.Register<GoapExecutor>(Lifetime.Singleton);
        builder.Register<WorldStatesObserver>(Lifetime.Singleton);
    }
}
