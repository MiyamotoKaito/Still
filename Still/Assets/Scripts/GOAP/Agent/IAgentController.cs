using Still.GOAP.Agent.Config;
using UnityEngine;
namespace Still.GOAP.Agent
{
    public interface IAgentController
    {
        GhostConfig Config { get; }
        Vector3 TargetPos { get; }
        void SetTarget(Vector3 targetPos);
        float Speed { get; }
        void SetSpeed(float speed);
        Vector3 GetRandomPos();
        Vector3 Position { get; }
        void SetMoveDestination(Vector3 target);
        bool IsArrived { get; }

        void PlayTriggerAnimation(string param);
        void PlayBoolAnimation(string param, bool flag);
    }
}