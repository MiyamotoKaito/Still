using Still.Enum.WorldStates;
using Still.GOAP.WorldState;
using Still.Player.View;
using UnityEngine;
using VContainer;

namespace Still.GOAP.Providers
{
    public class Sensor : MonoBehaviour
    {
        [Inject] private WorldStates _worldStates;
        [SerializeField]
        private float _checkInterval = 5;
        private void Update()
        {
            Physics.Raycast(transform.position, transform.forward, out RaycastHit hit);
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent<PlayerView>(out var player))
                {
                    if (_worldStates.GetStateValue(WorldStateType.PlayerVisible.ToString()) == 0)
                    {
                        _worldStates.ModifyState(WorldStateType.PlayerVisible.ToString(), 1);
                    }
                }
            }
            else if (hit.collider == null)  
            {
                _checkInterval -= Time.deltaTime;
                if (_checkInterval < 0)
                {
                    _worldStates.ModifyState(WorldStateType.PlayerVisible.ToString(), 0);
                    _checkInterval = 5f;
                }
            }
        }
    }
}
