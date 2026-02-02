using Still.GOAP.Agent.Config;
using Still.GOAP.Planner.Executor;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
namespace Still.GOAP.Agent
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class GAgent : MonoBehaviour, IAgentController
    {
        public GhostConfig Config => _ghostConfig;

        public float Speed => _speed;

        public Vector3 Position => this.gameObject.transform.position;

        public bool IsArrived => !_navmeshAgent.pathPending && _navmeshAgent.remainingDistance < _ghostConfig.GhostStopDistance;

        public GameObject CurrentTarget => _currentTarget;

        public void SetTarget(GameObject targetPos)
        {
            _currentTarget = targetPos;
        }

        public Vector3 GetRandomPos()
        {
            Vector2 random2D = Random.insideUnitCircle * _ghostConfig.Radius;
            Vector3 randomDirection = new Vector3(random2D.x, 0f, random2D.y);
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _ghostConfig.Radius / 2, NavMesh.AllAreas))
                return hit.position;
            else return Vector3.zero;
        }

        public void PlayBoolAnimation(string param, bool flag) => _animator.SetBool(param, flag);
        public void PlayTriggerAnimation(string param) => _animator.SetTrigger(param);

        public void SetMoveDestination(Vector3 target) => _navmeshAgent.SetDestination(target);

        public void SetSpeed(float speed) => _navmeshAgent.speed = speed;

        private GhostConfig _ghostConfig;
        private GoapExecutor _executor;
        private float _speed;
        private Animator _animator;
        private NavMeshAgent _navmeshAgent;
        private GameObject _currentTarget;

        [Inject]
        public void Construct(GhostConfig ghostConfig, GoapExecutor executor)
        {
            _ghostConfig = ghostConfig;
            _executor = executor;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _navmeshAgent = GetComponent<NavMeshAgent>();
        }
        private void Start()
        {
            _speed = _ghostConfig.GhostMoveSpeed;
            _navmeshAgent.speed = _speed;
        }
        private void Update()
        {
            _executor?.SetAction(this);
        }

    }
}
