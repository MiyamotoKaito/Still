namespace Still.Player.Model
{
    public class SpotLightModel
    {
        public bool IsOn => _isOn;
        private bool _isOn;
        public void SwitchToggle()
        {
            _isOn = !_isOn;
        }
    }
}
