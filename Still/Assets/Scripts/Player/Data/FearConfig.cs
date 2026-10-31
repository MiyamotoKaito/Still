using System;
using System.Collections.Generic;
using Template.Editor;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///SAN値の低下と敵の視認で起きる恐怖演出の設定
    ///</summary>
    [CreateAssetMenu(fileName = "FearConfig", menuName = "Config/Player/FearConfig")]
    public class FearConfig : ScriptableObject
    {
        ///<summary>恐怖の度合いに応じて常に効く効果</summary>
        public IReadOnlyList<ISanEffect> SanEffects => _sanEffects;
        ///<summary>敵を見たときの反応の候補</summary>
        public IReadOnlyList<FearReaction> GhostSeenReactions => _ghostSeenReactions;
        ///<summary>敵を見たときの反応を次に出せるまでの秒数</summary>
        public float GhostSeenCooldown => _ghostSeenCooldown;
        ///<summary>幻覚の候補</summary>
        public IReadOnlyList<FearReaction> Hallucinations => _hallucinations;
        ///<summary>幻覚が起き始める恐怖レベル</summary>
        public int MinHallucinationFearLevel => _minHallucinationFearLevel;
        ///<summary>平常に近いときの幻覚の間隔（秒、最小〜最大）</summary>
        public Vector2 HallucinationIntervalCalm => _hallucinationIntervalCalm;
        ///<summary>SAN値0に近いときの幻覚の間隔（秒、最小〜最大）</summary>
        public Vector2 HallucinationIntervalInsane => _hallucinationIntervalInsane;
        ///<summary>恐怖の度合いが実際のSAN値に追いつく速さ（1秒あたり）</summary>
        public float FearFollowSpeed => _fearFollowSpeed;

        [Header("常時の効果")]
        [SerializeReference, SubclassSelector] private List<ISanEffect> _sanEffects = new();

        [Header("敵を見たときの反応")]
        [SerializeField] private List<FearReaction> _ghostSeenReactions = new();
        [SerializeField] private float _ghostSeenCooldown = 8f;

        [Header("幻覚")]
        [SerializeField] private List<FearReaction> _hallucinations = new();
        [SerializeField, Range(0, 4)] private int _minHallucinationFearLevel = 1;
        [SerializeField] private Vector2 _hallucinationIntervalCalm = new(45f, 70f);
        [SerializeField] private Vector2 _hallucinationIntervalInsane = new(8f, 15f);

        [Header("その他")]
        [SerializeField, Tooltip("SAN値が急に減っても画面が一気に変わらないよう、ゆっくり追いつかせる")] private float _fearFollowSpeed = 0.3f;
    }

    ///<summary>
    ///恐怖レベルの範囲と重みを持つ、演出のまとまり（中の演出は同時に始まる）
    ///</summary>
    [Serializable]
    public class FearReaction
    {
        ///<summary>インスペクタで見分けるための名前</summary>
        public string Name => _name;
        ///<summary>この恐怖レベル以上で出る</summary>
        public int MinFearLevel => _minFearLevel;
        ///<summary>この恐怖レベル以下で出る</summary>
        public int MaxFearLevel => _maxFearLevel;
        ///<summary>選ばれやすさ</summary>
        public float Weight => _weight;
        ///<summary>同時に再生する演出</summary>
        public IReadOnlyList<IFearAction> Actions => _actions;

        [SerializeField] private string _name;
        [SerializeField, Range(0, 4)] private int _minFearLevel;
        [SerializeField, Range(0, 4)] private int _maxFearLevel = 4;
        [SerializeField, Min(0f)] private float _weight = 1f;
        [SerializeReference, SubclassSelector] private List<IFearAction> _actions = new();

        ///<summary>
        ///指定した恐怖レベルで出せるか
        ///</summary>
        public bool IsAvailable(int fearLevel) => fearLevel >= _minFearLevel && fearLevel <= _maxFearLevel && _weight > 0f;
    }
}
