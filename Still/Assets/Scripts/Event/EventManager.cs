using Still.Object.Key;
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
    private KeyView _keyView;

    private void Awake()
    {
        _tvObject = FindAnyObjectByType<Tv>();
        _recorderPlayerObject = FindAnyObjectByType<RecorderPlayer>();
        _bookObject = FindAnyObjectByType<Book>();
        _keyView = FindAnyObjectByType<KeyView>();
        SetText();
    }
    private void OnEnable()
    {
        RegisterEvents();
    }
    private void SetText()
    {
        _tv.text = $"□:Tv";
        _recordPlayer.text = $"□:PlayMusic";
        _book.text = $"□:Book";
    }
    private void RegisterEvents()
    {
        _tvObject.OnInteract += TvCondition;
        _recorderPlayerObject.OnInteract += RecorderCondition;
        _bookObject.OnInteract += BookCondition;
        _keyView.OnKeySpawned += SpawnKey;
    }
    private void TvCondition()
    {
        _tv.text = $"■:Tv";
        _achievedCount++;
        OnAcheivedCountChanged?.Invoke(_achievedCount);
    }
    private void RecorderCondition()
    {
        _recordPlayer.text = $"■:PlayMusic";
        _achievedCount++;
        OnAcheivedCountChanged?.Invoke(_achievedCount);
    }
    private void BookCondition()
    {
        _book.text = $"■:Book";
        _achievedCount++;
        OnAcheivedCountChanged?.Invoke(_achievedCount);
    }
    private void OnDisable()
    {
        _tvObject.OnInteract -= TvCondition;
        _recorderPlayerObject.OnInteract -= RecorderCondition;
        _bookObject.OnInteract -= BookCondition;
        _keyView.OnKeySpawned -= SpawnKey;
    }

    private void SpawnKey()
    {
        _tv.gameObject.SetActive(false);
        _recordPlayer.text = $"□:Find the key!!";
        _book.gameObject.SetActive(false);
    }
}
