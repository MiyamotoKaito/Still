using System;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public bool IsOn;
    public event Action<bool> SwitchChanged;

    [ContextMenu("スイッチ切り替え")]
    public void SwitchToggle()
    {
        IsOn = IsOn = true ? false : true;
        SwitchChanged.Invoke(IsOn);
    }
}