namespace Still.Object.Door.Model
{
    public class LockedDoorModel
    {
        public bool isLocked => _isLocked;
        private bool _isLocked = true;

        public void Unlock()
        {
            _isLocked = false;
        }
    }
}