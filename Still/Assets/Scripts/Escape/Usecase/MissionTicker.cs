using UnityEngine;
using VContainer.Unity;

namespace Still.Escape
{
    ///<summary>
    ///時間で進む目標を毎フレーム更新する
    ///</summary>
    public sealed class MissionTicker : ITickable
    {
        public MissionTicker(MissionUsecase usecase)
        {
            _usecase = usecase;
        }

        private readonly MissionUsecase _usecase;

        ///<summary>
        ///1フレーム分進める
        ///</summary>
        public void Tick() => _usecase.Tick(Time.deltaTime);
    }
}
