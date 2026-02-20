using Still.Enum.WorldStates;
using Still.GOAP.WorldState;
using System;
using TMPro;
using UnityEngine;
using VContainer;

public class LightSwitch : MonoBehaviour,IInteractable
{
    public bool IsOn => _isOn;
    public event Action<bool> SwitchChanged;
    [Inject] private readonly WorldStates _worldStates;
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private Light _light;
    private bool _isOn;

    [ContextMenu("スイッチ切り替え")]
    public void SwitchToggle()
    {
        AudioManager.Instance.PlaySE("Light1");
        _isOn = _isOn == true ? false : true;
        if (IsOn)
        {
            _light.enabled = true;
            _worldStates.AdditionState(WorldStateType.OnLighting.ToString(), 1);
        }
        else
        {
            _light.enabled = false;
            _worldStates.AdditionState(WorldStateType.OnLighting.ToString(), -1);
        }
        
    }
    public void Init()
    {
        _light.enabled = false;
    }
    public void Interact()
    {
        SwitchToggle();
    }

    public void ShowUI()
    {
        _text.text = IsOn ? "Turn Off Light" : "Turn On Light";
        _text.gameObject.SetActive(true);
    }

    public void HideUI()
    {
       _text.gameObject.SetActive(false);
    }
}