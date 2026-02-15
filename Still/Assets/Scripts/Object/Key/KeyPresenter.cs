using Still.Object.Key.Model;
using UnityEngine;
namespace Still.Object.Key
{
    public class KeyPresenter
    {
        private KeyView _key;
        private KeyModel _keyModel;
        private EventManager _eventManager;
        public KeyPresenter(KeyView key, KeyModel keyModel, EventManager eventManager)
        {
            _key = key;
            _keyModel = keyModel;
            _eventManager = eventManager;

            _eventManager.OnAcheivedCountChanged += CheckKeyCollection;
            _key.OnKeyCollected += UnlockDoor;
        }
        private void CheckKeyCollection(int count)
        {
            if (count == _keyModel.RequiredCount)
            {
                _key.gameObject.SetActive(true);
                _key.Spawn();
                Debug.Log("鍵が出現した");
            }
        }
        private void UnlockDoor()
        {
            _keyModel.Collect();
            Debug.Log("鍵が掛かっているドアを開けれるようになった");
        }
        public void Dispose()
        {
            _key.OnKeyCollected -= UnlockDoor;
        }
    }
}
