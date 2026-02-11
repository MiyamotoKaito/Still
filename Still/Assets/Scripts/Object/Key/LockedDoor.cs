using System;

namespace Still.Object.Door.View
{
    public class LockedDoor : DoorView
    {
        public event Action OnDoorLocked;
        private bool _isLocked = true;

        public override void Interact()
        {
            OnDoorLocked?.Invoke();

            if (_isLocked) return;

            ToggleIsOpen();
        }

        public override void ShowUI()
        {
            if (_isLocked)
            {
                _text.gameObject.SetActive(true);
                _text.text = "鍵が掛かっている";
                return;
            }
            base.ShowUI();
        }

        public void GetKey(bool isLocked)
        {
            _isLocked = isLocked;
        }
    }
}
