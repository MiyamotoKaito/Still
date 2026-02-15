using System;
using TMPro;
using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    public event Action OnInteract;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeReference, SubclassSelector]
    private IEvent _event;
    public void HideUI()
    {
        _text.gameObject.SetActive(false);
    }

    public void Interact()
    {
        _text.gameObject.SetActive(false);
        _event.Initialize();
        OnInteract?.Invoke();
        _event.OnEvent();
        this.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        _text.gameObject.SetActive(true);
        _text.text = "本を回収";
    }
}
