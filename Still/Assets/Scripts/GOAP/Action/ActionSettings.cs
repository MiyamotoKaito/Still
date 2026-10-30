using System;
using System.Collections.Generic;
using Still.Common;
using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///条件付きのワールドステート（前提条件・ゴール条件）
    ///</summary>
    [Serializable]
    public class HasConditionActions
    {
        ///<summary>対象のワールドステート</summary>
        public WorldStateType WorldStateKey => _key;
        ///<summary>条件</summary>
        public Condition Value => _value;

        [SerializeField] private WorldStateType _key;
        [SerializeField] private Condition _value;
    }

    ///<summary>
    ///書き込む値（効果）
    ///</summary>
    [Serializable]
    public class NoConditionActions
    {
        ///<summary>対象のワールドステート</summary>
        public WorldStateType WorldStateKey => _key;
        ///<summary>書き込む値</summary>
        public int Value => _value;

        [SerializeField] private WorldStateType _key;
        [SerializeField] private int _value;
    }

    ///<summary>
    ///インスペクタ用のリストを辞書に変換する（同じキーは後勝ち）
    ///</summary>
    public static class ActionSettingConverter
    {
        ///<summary>
        ///条件のリストを辞書にする
        ///</summary>
        public static Dictionary<WorldStateType, Condition> ToDictionary(List<HasConditionActions> list)
        {
            var dict = new Dictionary<WorldStateType, Condition>();
            foreach (var item in list) dict[item.WorldStateKey] = item.Value;
            return dict;
        }

        ///<summary>
        ///効果のリストを辞書にする
        ///</summary>
        public static Dictionary<WorldStateType, int> ToDictionary(List<NoConditionActions> list)
        {
            var dict = new Dictionary<WorldStateType, int>();
            foreach (var item in list) dict[item.WorldStateKey] = item.Value;
            return dict;
        }
    }
}
