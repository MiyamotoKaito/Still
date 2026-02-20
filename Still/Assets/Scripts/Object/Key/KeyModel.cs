using UnityEngine;
namespace Still.Object.Key.Model
{
    public class KeyModel
    {
        public bool IsCollected => _isCollected;
        public int RequiredCount => _requiredCount;
        private bool _isCollected = false;
        private int _requiredCount;
        public KeyModel(int requiredCount)
        {
            _requiredCount = requiredCount;
        }
        public void Collect()
        {
            _isCollected = true;
            Debug.Log("KeyModel: 鍵が収集されました。");
        }
    }
}
