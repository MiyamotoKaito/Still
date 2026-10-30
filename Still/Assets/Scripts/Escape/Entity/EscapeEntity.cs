using System;
using R3;

namespace Still.Escape
{
    ///<summary>
    ///脱出の進み具合と鍵の所持
    ///</summary>
    public sealed class EscapeEntity : IDisposable
    {
        public EscapeEntity(int requiredItemCount)
        {
            _progress = new ReactiveProperty<EscapeProgress>(new EscapeProgress(EscapeItemType.None, requiredItemCount));
        }

        ///<summary>調べたものの進み具合</summary>
        public ReadOnlyReactiveProperty<EscapeProgress> Progress => _progress;
        ///<summary>鍵を持っているか</summary>
        public ReadOnlyReactiveProperty<bool> HasKey => _hasKey;

        private readonly ReactiveProperty<EscapeProgress> _progress;
        private readonly ReactiveProperty<bool> _hasKey = new(false);

        ///<summary>
        ///進み具合を差し替える
        ///</summary>
        public void SetProgress(EscapeProgress progress) => _progress.Value = progress;

        ///<summary>
        ///鍵の所持を差し替える
        ///</summary>
        public void SetHasKey(bool hasKey) => _hasKey.Value = hasKey;

        ///<summary>
        ///破棄する
        ///</summary>
        public void Dispose()
        {
            _progress.Dispose();
            _hasKey.Dispose();
        }
    }
}
