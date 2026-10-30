using System;
using R3;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーカメラの視野角
    ///</summary>
    public sealed class FieldOfViewEntity : IDisposable
    {
        public FieldOfViewEntity(PlayerConfig config)
        {
            DefaultFov = config.DefaultFOV;
            _fov = new ReactiveProperty<float>(config.DefaultFOV);
        }

        ///<summary>通常時の視野角</summary>
        public float DefaultFov { get; }
        ///<summary>現在の視野角</summary>
        public ReadOnlyReactiveProperty<float> Fov => _fov;

        private readonly ReactiveProperty<float> _fov;

        ///<summary>
        ///視野角を変える
        ///</summary>
        public void SetFov(float fov) => _fov.Value = fov;

        ///<summary>
        ///破棄する
        ///</summary>
        public void Dispose() => _fov.Dispose();
    }
}
