namespace Still.Player.Model
{
    public class PlayerModel
    {
        public float CurrentSpeed => _currentSpeed;
        private float _currentSpeed;
        public void SetMoveSpeed(float dashSpeed)
        {
            _currentSpeed = dashSpeed;
        }
    }
}