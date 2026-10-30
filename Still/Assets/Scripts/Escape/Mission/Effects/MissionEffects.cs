using System;
using Still.Common;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///鍵の掛かったドアを開けられるようにする
    ///</summary>
    [Serializable]
    public class UnlockDoorEffect : IMissionEffect
    {
        [SerializeField, Tooltip("開けられるようにするドアのID")] private string _doorId;

        ///<summary>
        ///ドアを解錠する
        ///</summary>
        public void Apply(IMissionEffectContext context) => context.UnlockDoor(new TargetId(_doorId));
    }

    ///<summary>
    ///隠れていた調べる物を出現させる
    ///</summary>
    [Serializable]
    public class RevealTargetEffect : IMissionEffect
    {
        [SerializeField, Tooltip("出現させる物のID")] private string _targetId;

        ///<summary>
        ///出現させる
        ///</summary>
        public void Apply(IMissionEffectContext context) => context.RevealTarget(new TargetId(_targetId));
    }

    ///<summary>
    ///ワールドステートを書き換える
    ///</summary>
    [Serializable]
    public class SetWorldStateEffect : IMissionEffect
    {
        [SerializeField] private WorldStateType _key;
        [SerializeField] private int _value;

        ///<summary>
        ///書き換える
        ///</summary>
        public void Apply(IMissionEffectContext context) => context.SetWorldState(_key, _value);
    }

    ///<summary>
    ///SEを鳴らす
    ///</summary>
    [Serializable]
    public class PlaySeEffect : IMissionEffect
    {
        [SerializeField] private string _soundName;

        ///<summary>
        ///鳴らす
        ///</summary>
        public void Apply(IMissionEffectContext context) => context.PlaySe(_soundName);
    }
}
