using UnityEngine;
namespace Still.Object.Key.Model
{
    public class KeyModel
    {
        public bool IsCollected => _isCollected;

        private bool _isCollected = false;

        public void Collect()
        {
            _isCollected = true;
            Debug.Log("KeyModel: 鍵が収集されました。");
        }
    }
}
