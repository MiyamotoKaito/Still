using System;
using System.Collections.Generic;
using Template.Editor;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///脱出までのステージの並び
    ///</summary>
    [CreateAssetMenu(fileName = "MissionConfig", menuName = "Config/Escape/MissionConfig")]
    public class MissionConfig : ScriptableObject
    {
        ///<summary>上から順に進むステージ</summary>
        public IReadOnlyList<MissionStage> Stages => _stages;
        ///<summary>開けると脱出になるドアのID</summary>
        public string ExitDoorId => _exitDoorId;
        ///<summary>全ステージを終えた後にチェックリストへ出す文</summary>
        public string ClearMessage => _clearMessage;

        [SerializeField] private List<MissionStage> _stages = new();
        [SerializeField] private string _exitDoorId = "Exit";
        [SerializeField] private string _clearMessage = "出口から脱出しろ";
    }

    ///<summary>
    ///1つのステージ。目標をすべて達成すると次へ進む
    ///</summary>
    [Serializable]
    public class MissionStage
    {
        ///<summary>チェックリストの見出し</summary>
        public string Title => _title;
        ///<summary>ステージ開始時の効果</summary>
        public IReadOnlyList<IMissionEffect> OnStart => _onStart;
        ///<summary>達成すべき目標</summary>
        public IReadOnlyList<IObjective> Objectives => _objectives;
        ///<summary>ステージ達成時の効果</summary>
        public IReadOnlyList<IMissionEffect> OnComplete => _onComplete;

        [SerializeField] private string _title;
        [SerializeReference, SubclassSelector] private List<IMissionEffect> _onStart = new();
        [SerializeReference, SubclassSelector] private List<IObjective> _objectives = new();
        [SerializeReference, SubclassSelector] private List<IMissionEffect> _onComplete = new();
    }
}
