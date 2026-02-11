using Still.Enum.WorldStates;
using Still.GOAP.WorldState;
using Still.Player.View;
using UnityEngine;
using VContainer;
namespace Still.Object
{
    public class Room : MonoBehaviour
    {
        public bool InTheRoom => _inTheRoom;
        public GameObject Door => _door;

        [SerializeField] private GameObject _door;
        [Inject] WorldStates _worldStates;
        private bool _inTheRoom;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerView>(out var playerView))
            {
                _worldStates.ModifyState(WorldStateType.InTheRoom.ToString(), 1);
                _inTheRoom = true;
            }

        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerView>(out var playerView))
            {
                _worldStates.ModifyState(WorldStateType.InTheRoom.ToString(), 0);
                _inTheRoom = false;
            }
        }
    }
}
