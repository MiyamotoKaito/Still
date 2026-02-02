using Still.Enum.WorldStates;
using Still.GOAP.WorldState;
using System;
using UnityEngine;
using VContainer;

public class LightSwitch : MonoBehaviour
{
    public bool IsOn;
    public event Action<bool> SwitchChanged;
    [Inject] private readonly WorldStates _worldStates;
    [ContextMenu("スイッチ切り替え")]
    public void SwitchToggle()
    {
        IsOn = IsOn == true ? false : true;
        if (IsOn)
        {
            _worldStates.AdditionState(WorldStateType.OnLighting.ToString(), 1);
        }
        else
        {
            _worldStates.AdditionState(WorldStateType.OnLighting.ToString(), -1);
        }
        //SwitchChanged.Invoke(IsOn);
    }

    public void Initialize()
    {
        // 初期状態をWorldStatesに反映
        if (IsOn)
        {
            _worldStates.AdditionState(WorldStateType.OnLighting.ToString(), 1);
        }
    }
}