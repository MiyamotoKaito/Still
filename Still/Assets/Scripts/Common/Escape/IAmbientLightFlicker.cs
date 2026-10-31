using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Still.Common
{
    ///<summary>
    ///部屋の照明を一時的にちらつかせる（点灯状態は変えない演出用）
    ///</summary>
    public interface IAmbientLightFlicker
    {
        ///<summary>
        ///指定位置の近くで点いている照明をしばらくちらつかせる
        ///</summary>
        UniTask FlickerNearAsync(Vector3 position, float radius, float duration, CancellationToken ct);
    }
}
