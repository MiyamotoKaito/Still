using Still.Player;
using Still.Player.View;
using System;
using TMPro;
using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    public event Action OnInteract;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeReference, SubclassSelector]
    private IEvent _event;
    private GameInstaller _gameInstaller;
    private PlayerView _playerview;
    public void HideUI()
    {
        _text.gameObject.SetActive(false);
    }

    public void Interact()
    {
        _text.gameObject.SetActive(false);
        _playerview = FindAnyObjectByType<PlayerView>();
        _gameInstaller = FindAnyObjectByType<GameInstaller>();
        _gameInstaller.LastPosition.SavePosition(_playerview.transform.position);
        _event.Initialize();
        OnInteract?.Invoke();
        _event.OnEvent();
        this.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        _text.gameObject.SetActive(true);
        _text.text = "Collect The Book!";
    }
}
