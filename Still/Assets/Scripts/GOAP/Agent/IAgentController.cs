using Still.GOAP.Agent.Config;
using UnityEngine;
namespace Still.GOAP.Agent
{
    public interface IAgentController
    {
        GhostConfig Config { get; }
        AudioSource AudioSource { get; }
        GameObject CurrentTarget { get; }
        void SetTarget(GameObject targetPos);
        float Speed { get; }
        void SetSpeed(float speed);
        Vector3 GetRandomPos();
        Vector3 Position { get; }
        void SetMoveDestination(Vector3 target);
        bool IsArrived { get; }
        void Teleport(Vector3 pos);
        void StopMove();
        void PlayTriggerAnimation(string param);
        void PlayBoolAnimation(string param, bool flag);
    }
}