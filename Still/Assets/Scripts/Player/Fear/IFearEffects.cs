using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///恐怖の度合いに応じて毎フレーム効く効果（FearConfigにSerializeReferenceで並べる）
    ///</summary>
    public interface ISanEffect
    {
        ///<summary>
        ///恐怖の度合い（0=平常、1=SAN値0）に応じて効果を反映する
        ///</summary>
        void Apply(float fear, FearEffectContext context);

        ///<summary>
        ///効果を消して元に戻す
        ///</summary>
        void Reset(FearEffectContext context);
    }

    ///<summary>
    ///一度きりの恐怖演出（敵を見たときの反応や幻覚。FearConfigにSerializeReferenceで並べる）
    ///</summary>
    public interface IFearAction
    {
        ///<summary>
        ///演出を再生する
        ///</summary>
        UniTask PlayAsync(FearEffectContext context, CancellationToken ct);
    }

    ///<summary>
    ///開始を遅らせられる恐怖演出の基底
    ///</summary>
    [Serializable]
    public abstract class FearActionBase : IFearAction
    {
        [SerializeField, Tooltip("同じ反応の中で、開始を遅らせる秒数")] private float _delay;

        ///<summary>
        ///遅らせてから演出を再生する
        ///</summary>
        public async UniTask PlayAsync(FearEffectContext context, CancellationToken ct)
        {
            if (_delay > 0f) await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: ct);
            await PlayCoreAsync(context, ct);
        }

        protected abstract UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct);

        // 経過時間の割合（0→1）を毎フレーム渡す
        protected static async UniTask RunForAsync(float duration, Action<float> onFrame, CancellationToken ct)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                onFrame(elapsed / duration);
                await UniTask.Yield(ct);
                elapsed += Time.deltaTime;
            }
        }
    }
}
