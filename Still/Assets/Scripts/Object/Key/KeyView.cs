using System;
using TMPro;
using UnityEngine;
namespace Still.Object.Key
{
    public class KeyView : MonoBehaviour, IInteractable
    {
        public event Action OnKeyCollected;
        public event Action OnKeySpawned;
        public GameObject[] SpawnPositions => _spawnPositions;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private GameObject[] _spawnPositions;
        public void Init()
        {
            this.gameObject.SetActive(false);
        }
        public void HideUI()
        {
            _text.gameObject.SetActive(false);
        }

        public void Interact()
        {
            OnKeyCollected?.Invoke();
            this.gameObject.SetActive(false);
        }

        public void ShowUI()
        {
            if (_text == null)
            {
                Debug.LogError($"{gameObject.name} の _text がアサインされていません！");
                return;
            }
            Debug.Log($"KeyView: {gameObject.name} ShowUI called."); // どのオブジェクトか判別
            _text.gameObject.SetActive(true);
            _text.text = "Collect Key";
        }
        public void Spawn()
        {
            if (_spawnPositions == null || _spawnPositions.Length == 0)
            {
                Debug.LogError($"{gameObject.name} の SpawnPositions が設定されていません！");
                return;
            }
            int randomIndex = UnityEngine.Random.Range(0, _spawnPositions.Length);
            this.transform.position = _spawnPositions[randomIndex].transform.position;
            this.gameObject.SetActive(true);
            OnKeySpawned?.Invoke();
        }
    }
}