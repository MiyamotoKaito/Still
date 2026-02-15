using System;
using TMPro;
using UnityEngine;

public class RecorderPlayer : MonoBehaviour,IInteractable
{
    public event Action OnInteract;
    [SerializeField]
    private TextMeshProUGUI _text;
    private AudioSource _audiosource;
    public void HideUI()
    {
        _text.gameObject.SetActive(false);
    }

    public void Interact()
    {
        OnInteract?.Invoke();
        PlayMusic();
        _text.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        _text.gameObject.SetActive(true);
        _text.text = "Play Music";
    }

    private void PlayMusic()
    {
        AudioManager.Instance.PlayBGM("Recorder", _audiosource);
    }
}
