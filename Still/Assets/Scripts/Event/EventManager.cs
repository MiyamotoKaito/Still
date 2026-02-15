using System;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public event Action<int> OnAcheivedCountChanged;
    [SerializeField] private TextMeshProUGUI _tv;
    [SerializeField] private TextMeshProUGUI _recordPlayer;
    [SerializeField] private TextMeshProUGUI _book;
    private Tv _tvObject;
    private RecorderPlayer _recorderPlayerObject;
    private Book _bookObject;
    private int _achievedCount = 0;

    private void Awake()
    {
        _tvObject = FindAnyObjectByType<Tv>();
        _recorderPlayerObject = FindAnyObjectByType<RecorderPlayer>();
        _bookObject = FindAnyObjectByType<Book>();
        SetText();
    }
    private void OnEnable()
    {
        RegisterEvents();
    }
    private void SetText()
    {
        _tv.text = $"□:tv";
        _recordPlayer.text = $"□:recordPlayer";
        _book.text = $"□:book";
    }
    private void RegisterEvents()
    {
        _tvObject.OnInteract += TvCondition;
        _recorderPlayerObject.OnInteract += RecorderCondition;
        _bookObject.OnInteract += BookCondition;
    }
    private void TvCondition()
    {
        _tv.text = $"■:tv";
        _achievedCount++;
        OnAcheivedCountChanged?.Invoke(_achievedCount);
    }
    private void RecorderCondition()
    {
        _recordPlayer.text = $"■:recordPlayer";
        _achievedCount++;
        OnAcheivedCountChanged?.Invoke(_achievedCount);
    }
    private void BookCondition()
    {
        _book.text = $"■:book";
        _achievedCount++;
        OnAcheivedCountChanged?.Invoke(_achievedCount);
    }
    private void OnDisable()
    {
        _tvObject.OnInteract -= TvCondition;
        _recorderPlayerObject.OnInteract -= RecorderCondition;
        _bookObject.OnInteract -= BookCondition;
    }
}
