using Still.Enum.WorldStates;
using Still.GOAP.WorldState;
using Still.Player.Model;
using UniRx;
using UnityEngine;
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
        }

        public void LevelUpFearLevel()
        {
            if (_worldStates == null)
            Debug.Log($"{_worldStates}入っていない");
            _worldStates.OnStateChanged.Where(x => x == WorldStateType.FearLevel.ToString())
                .Subscribe(_ => 
                {
                    _sanModel.ModifySAN(-10);
                    Debug.Log($"恐怖レベル上昇！ SAN値減少: {_sanModel.CurrentSAN}");
                });
        }
    }
}
