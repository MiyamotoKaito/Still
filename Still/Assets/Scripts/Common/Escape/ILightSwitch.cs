using UnityEngine;

namespace Still.Common
{
    ///<summary>
    ///照明スイッチ
    ///</summary>
    public interface ILightSwitch
    {
        ///<summary>点灯しているか</summary>
        bool IsOn { get; }
        ///<summary>スイッチの位置</summary>
        Transform Transform { get; }
    }
}
