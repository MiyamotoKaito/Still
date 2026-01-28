using Still.Enum.WorldStates;
using Still.GOAP.WorldState;
using UnityEngine;
using VContainer;
namespace Still.GOAP.Provider
{
    public class LightStateProvider : MonoBehaviour
    {
        [Inject] private readonly WorldStates _worldStates;
        [SerializeField] private readonly LightSwitch _lightSwich;

        private void Start()
        {
            UpdateWorldStates(_lightSwich.IsOn);
        }
        private void OnEnable()
        {
            _lightSwich.SwitchChanged += UpdateWorldStates;
        }
        private void OnDisable()
        {
            _lightSwich.SwitchChanged -= UpdateWorldStates;
        }
        /// <summary>
        /// スイッチが押されていたらステートを書き換える
        /// </summary>
        /// <param name="isOn"></param>
        private void UpdateWorldStates(bool isOn)
        {
            _worldStates.ModifyState(WorldStateType.OnLighting.ToString(), isOn ? 1 : 0);
            Debug.Log($"[Provider] WorldStateを更新しました: IsLightOn = {(isOn ? 1 : 0)}");
        }
    }
}
