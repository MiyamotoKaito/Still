using Still.Enum.WorldStates;
using Still.GOAP.WorldState;
using Still.Player.Model;
namespace Still.Player.Presenter
{
    public class PlayerFearLevelPresenter
    {
        private SANValueModel _sanModel;
        private WorldStates _worldStates;
        public PlayerFearLevelPresenter(SANValueModel sanModel, WorldStates worldStates)
        {
            _sanModel = sanModel;
            _worldStates = worldStates;

            _sanModel.OnSANChanged += LevelUpFearLevel;
        }

        public void LevelUpFearLevel(float value)
        {
            int fearLevel = 0;
            if (value < 10) fearLevel = 4;
            else if (value < 30) fearLevel = 3;
            else if (value < 60) fearLevel = 2;
            else if (value < 80) fearLevel = 1;
            else fearLevel = 0;

            _worldStates.ModifyState(WorldStateType.FearLevel.ToString(), fearLevel);

        }
    }
}
