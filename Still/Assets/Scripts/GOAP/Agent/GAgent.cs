using Still.Common;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストの体。移動とアニメーションだけを担当し、判断はGoapExecutorが行う
    ///</summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class GAgent : MonoBehaviour, IAgentController, IGhost
    {
        ///<summary>ゴーストの設定</summary>
        public GhostConfig Config => _ghostConfig;
        ///<summary>ゴーストが鳴らす音のAudioSource</summary>
        public AudioSource AudioSource => _audioSource;
        ///<summary>現在のターゲット（無ければnull）</summary>
        public Transform CurrentTarget => _currentTarget;
        ///<summary>現在位置</summary>
        public Vector3 Position => transform.position;
        ///<summary>ターゲットに十分近づいたか</summary>
        public bool IsArrived => _currentTarget != null
            && Vector3.Distance(Position, _currentTarget.position) < _ghostConfig.GhostStopDistance;

        private GhostConfig _ghostConfig;
        private AudioSource _audioSource;
        private NavMeshAgent _navMeshAgent;
        private Transform _currentTarget;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void Construct(GhostConfig ghostConfig)
        {
            _ghostConfig = ghostConfig;
        }

        ///<summary>
        ///ターゲットを設定する
        ///</summary>
        public void SetTarget(Transform target)
        {
            _currentTarget = target;
        }

        ///<summary>
        ///移動速度を設定する
        ///</summary>
        public void SetSpeed(float speed)
        {
            _navMeshAgent.speed = speed;
        }

        ///<summary>
        ///NavMesh上のランダムな地点を取得する（見つからなければ現在位置）
        ///</summary>
        public Vector3 GetRandomPos()
        {
            Vector2 random2D = Random.insideUnitCircle * _ghostConfig.Radius;
            var candidate = transform.position + new Vector3(random2D.x, 0f, random2D.y);

            return NavMesh.SamplePosition(candidate, out NavMeshHit hit, _ghostConfig.Radius / 2, NavMesh.AllAreas)
                ? hit.position
                : Position;
        }

        ///<summary>
        ///目的地へ移動を始める
        ///</summary>
        public void SetMoveDestination(Vector3 destination)
        {
            _navMeshAgent.updateRotation = true;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(destination);
        }

        ///<summary>
        ///瞬間移動する
        ///</summary>
        public void Teleport(Vector3 position)
        {
            _navMeshAgent.Warp(position);
            // 当たり判定は次の物理更新まで元の位置に残るため、同じフレームのプレイヤーの視線が
            // 消えたはずのゴーストに当たり、FocusMeがもう一度発動してしまう。その場で位置を反映させる
            Physics.SyncTransforms();
        }

        ///<summary>
        ///その場で止まる
        ///</summary>
        public void StopMove()
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.velocity = Vector3.zero;
            // ResetPathだけだと次フレームに慣性で少し進むため、次の位置も固定する
            _navMeshAgent.nextPosition = transform.position;
        }

        ///<summary>
        ///指定位置の方へ水平に向き直る（移動を再開するまで自動の向き変更を止める）
        ///</summary>
        public void LookAt(Vector3 position, float degreesPerSecond)
        {
            // NavMeshAgentが向きを上書きしないよう、自分で回している間は止めておく
            _navMeshAgent.updateRotation = false;

            var direction = position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) return;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(direction), degreesPerSecond * Time.deltaTime);
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _navMeshAgent.speed = _ghostConfig.GhostMoveSpeed;
        }
    }
}
