using System.Collections.Generic;

namespace Still.Common
{
    ///<summary>
    ///照明スイッチの操作（プレイヤーとゴーストの両方が使う）
    ///</summary>
    public interface ILightSwitchUsecase
    {
        ///<summary>シーン内の全スイッチ</summary>
        IReadOnlyList<ILightSwitch> LightSwitches { get; }

        ///<summary>
        ///スイッチを切り替える
        ///</summary>
        void Toggle(ILightSwitch lightSwitch);
    }
}
