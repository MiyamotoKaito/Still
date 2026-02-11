using Still.Object.Key.Model;
using UnityEngine;
namespace Still.Object.Key
{
    public class KeyPresenter
    {
        private KeyView _key;
        private KeyModel _keyModel;
        public KeyPresenter(KeyView key, KeyModel keyModel)
        {
            _key = key;
            _keyModel = keyModel;

            _key.OnKeyCollected += UnlockDoor;
        }

        private void UnlockDoor()
        {
            _keyModel.Collect();
            Debug.Log("鍵が掛かっているドアを開けれるようになった");
        }
    }
}
