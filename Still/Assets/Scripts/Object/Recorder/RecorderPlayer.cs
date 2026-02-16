using Still.Player;
using Still.Player.View;
using System;
using TMPro;
using UnityEngine;

public class RecorderPlayer : MonoBehaviour, IInteractable
{
    public event Action OnInteract;
    [SerializeField]
    private TextMeshProUGUI _text;
    private AudioSource _audiosource;
    private PlayerView _playerview;
    private GameInstaller _gameInstaller;
    public void HideUI()
    {
        _text.gameObject.SetActive(false);
    }

    public void Interact()
    {
        _playerview = FindAnyObjectByType<PlayerView>();
        _gameInstaller = FindAnyObjectByType<GameInstaller>();
        _gameInstaller.LastPosition.SavePosition(_playerview.transform.position);
        OnInteract?.Invoke();
        PlayMusic();
        _text.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        _text.gameObject.SetActive(true);
        _text.text = "Play Music";
    }
    private void Awake()
    {
        _audiosource = GetComponent<AudioSource>();

    }
    private void PlayMusic()
    {
        AudioManager.Instance.PlayBGM("Recorder1", _audiosource);
    }
}
