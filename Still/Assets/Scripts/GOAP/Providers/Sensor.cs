using Still.Enum.WorldStates;
using Still.GOAP.Agent.Config;
using Still.GOAP.WorldState;
using Still.Player.View;
using UnityEngine;
using VContainer;

namespace Still.GOAP.Providers
{
    public class Sensor : MonoBehaviour
    {
        [Inject] private WorldStates _worldStates;
        [Inject] private GhostConfig _ghostConfig;
        [SerializeField] private LayerMask _playerLayer;
        private float _checkInterval = 5;
        private float _timer = 0f;
        private void Update()
        {
            var canseePlayer = Physics.Raycast(
                transform.position,
                transform.forward,
                out RaycastHit hit,
                _ghostConfig.GhostFovLength,
                _playerLayer);

            // プレイヤーが見えている場合
            if (canseePlayer)
            {
                if (_worldStates.GetStateValue(WorldStateType.PlayerVisible.ToString()) == 0)
                {
                    _worldStates.ModifyState(WorldStateType.PlayerVisible.ToString(), 1);
                    Debug.Log("[Sensor] プレイヤーを発見！");
                }
                // タイマーをリセット
                _timer = 0f;
            }
            // プレイヤーが見えていない場合
            else
            {
                // 現在プレイヤーが見えている状態なら、タイマーを開始
                if (_worldStates.GetStateValue(WorldStateType.PlayerVisible.ToString()) == 1)
                {
                    _timer += Time.deltaTime;

                    if (_timer >= _checkInterval)
                    {
                        _worldStates.ModifyState(WorldStateType.PlayerVisible.ToString(), 0);
                        _timer = 0f;
                        Debug.Log("[Sensor] プレイヤーを見失いました");
                    }
                }
            }
        }
    }
}