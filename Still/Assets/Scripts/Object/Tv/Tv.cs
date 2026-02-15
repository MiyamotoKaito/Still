using System;
using TMPro;
using UnityEngine;

public class Tv : MonoBehaviour, IInteractable
{
    public event Action OnInteract;
    [SerializeField]
    private TextMeshProUGUI _text;
    private Light _tvLight;
    private bool _isOn = false;
    private AudioSource _audiosource;
    private void Awake()
    {
        _tvLight = GetComponentInChildren<Light>();
        _tvLight.enabled = false;
    }
    public void HideUI()
    {
        _text.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (!_isOn)
        {
            _tvLight.enabled = true;
            _isOn = true;
            AudioManager.Instance.PlayBGM("Tv", _audiosource);
            OnInteract?.Invoke();
            _text.gameObject.SetActive(false);
        }
    }

    public void ShowUI()
    {
        if (!_isOn)
        {
            _text.text = "Watch TV";
            _text.gameObject.SetActive(true);
        }
    }
}
