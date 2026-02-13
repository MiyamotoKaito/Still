using Still.Enum.WorldStates;
using Still.Player;
using UniRx;
using UnityEngine;
using VContainer;

namespace Still.GOAP.WorldState
{
    public class WorldStatesManager : MonoBehaviour
    {
        [Inject] private WorldStates _worldStates;
        private GameInstaller _installer;
        private bool _isInTheRoom;
        private float _timeInTheRoom;
        private void Awake()
        {
            _installer = FindAnyObjectByType<GameInstaller>();
        }
        private void Start()
        {
            _worldStates.OnStateChanged.Subscribe(_ => OnStateChangedHandler());
        }
        private void Update()
        {
            CanKnock();
        }
        /// <summary>
        /// WorldStateが変更されたときの処理
        /// </summary>
        private void OnStateChangedHandler()
        {
            ModifyFearLevel();
            StayInTheRoom();
        }
        /// <summary>
        /// プレイヤーのSAN値に応じて恐怖レベルを変更する
        /// </summary>
        private void ModifyFearLevel()
        {
            var fearLevel = _installer.SANValueModel.CurrentSAN;

            if (fearLevel > 80)
                _worldStates.ModifyState(WorldStateType.FearLevel.ToString(), 0);
            else if (fearLevel > 60)
                _worldStates.ModifyState(WorldStateType.FearLevel.ToString(), 1);
            else if (fearLevel > 40)
                _worldStates.ModifyState(WorldStateType.FearLevel.ToString(), 2);
            else if (fearLevel > 20)
                _worldStates.ModifyState(WorldStateType.FearLevel.ToString(), 3);
            else
                _worldStates.ModifyState(WorldStateType.FearLevel.ToString(), 4);
        }
        /// <summary>
        /// 部屋にいるかどうかで状態を変更する
        /// </summary>
        private void StayInTheRoom()
        {
            if (_worldStates.GetStateValue(WorldStateType.InTheRoom.ToString()) == 1)
            {
                _isInTheRoom = true;
            }
            else
            {
                _isInTheRoom = false;
            }
        }
        /// <summary>
        /// 部屋の滞在時間に応じてノック可能状態にする
        /// </summary>
        private void CanKnock()
        {
            if (!_isInTheRoom)
                return;

            _timeInTheRoom += Time.deltaTime;
            if (_timeInTheRoom >= 15f)
            {
                _worldStates.ModifyState(WorldStateType.Knock.ToString(), 1);
                _worldStates.ModifyState(WorldStateType.InTheRoom.ToString(), 0);
                _timeInTheRoom = 0f;
            }
        }
    }
}