using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using Still.Utility;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///ミッションのルール（ステージの進行・目標の判定・効果の実行・脱出と階移動）
    ///</summary>
    public sealed class MissionUsecase : IMissionEffectContext, IMissionContext
    {
        private const float EscapeFadeSeconds = 2f;
        private const int WaitAfterEscapeFadeMilliseconds = 2000;
        private const float TravelFadeSeconds = 0.6f;

        public MissionUsecase(MissionConfig config, MissionEntity entity, SceneComponentList<LightSwitch> lightSwitches,
                              IWorldStates worldStates, IAudioService audio, ISanUsecase san,
                              IPlayerAvatar player, IScreenFader fader, IStateMachine stateMachine)
        {
            _config = config;
            _entity = entity;
            _lightSwitches = lightSwitches;
            _worldStates = worldStates;
            _audio = audio;
            _san = san;
            _player = player;
            _fader = fader;
            _stateMachine = stateMachine;
        }

        ///<summary>全ステージを終えたか</summary>
        public bool IsCleared => _entity.StageIndex.CurrentValue >= _config.Stages.Count;
        ///<summary>いまのステージの見出し（全ステージ達成後はクリア後の文）</summary>
        public string CurrentTitle => IsCleared ? _config.ClearMessage : _config.Stages[_entity.StageIndex.CurrentValue].Title;

        private readonly MissionConfig _config;
        private readonly MissionEntity _entity;
        private readonly SceneComponentList<LightSwitch> _lightSwitches;
        private readonly IWorldStates _worldStates;
        private readonly IAudioService _audio;
        private readonly ISanUsecase _san;
        private readonly IPlayerAvatar _player;
        private readonly IScreenFader _fader;
        private readonly IStateMachine _stateMachine;

        ///<summary>
        ///最初のステージから始める
        ///</summary>
        public void Begin()
        {
            EnterStage(0);
        }

        ///<summary>
        ///調べたことをいまのステージの目標へ渡す。間違えたらペナルティを与える
        ///</summary>
        public InteractOutcomeType HandleInteract(TargetId target)
        {
            if (IsCleared) return InteractOutcomeType.Ignored;

            var result = InteractOutcomeType.Ignored;
            foreach (var tracker in _entity.Trackers)
            {
                var outcome = tracker.HandleInteract(target);
                if (outcome.OutcomeType == InteractOutcomeType.Ignored) continue;

                if (outcome.OutcomeType == InteractOutcomeType.Mistake)
                {
                    _san.Damage(outcome.SanDamage);
                    if (!string.IsNullOrEmpty(outcome.MistakeSe)) _audio.PlaySe(outcome.MistakeSe);
                }
                // 同じ物が複数の目標に関わる場合は「進んだ」を優先して返す
                if (result != InteractOutcomeType.Progressed) result = outcome.OutcomeType;
            }

            if (result == InteractOutcomeType.Ignored) return result;

            _entity.NotifyProgressChanged();
            CompleteStageIfDone();
            return result;
        }

        ///<summary>
        ///時間で進む目標（儀式）を更新する
        ///</summary>
        public void Tick(float deltaTime)
        {
            if (IsCleared) return;

            bool changed = false;
            foreach (var tracker in _entity.Trackers) changed |= tracker.Tick(deltaTime, this);
            UpdateRitualState();

            if (!changed) return;
            _entity.NotifyProgressChanged();
            CompleteStageIfDone();
        }

        ///<summary>
        ///切り替え式の物が動いているかを記録する
        ///</summary>
        public void ReportTargetActive(TargetId target, bool isActive) => _entity.SetTargetActive(target, isActive);

        ///<summary>
        ///ドアを開けられるか
        ///</summary>
        public bool IsDoorUnlocked(TargetId door) => _entity.IsDoorUnlocked(door);

        ///<summary>
        ///開けると脱出になるドアか
        ///</summary>
        public bool IsExit(TargetId door) => door.Value == _config.ExitDoorId;

        ///<summary>
        ///暗転してタイトルへ戻る
        ///</summary>
        public async UniTask EscapeAsync(CancellationToken ct)
        {
            _player.LockInput();
            await _fader.FadeOutAsync(EscapeFadeSeconds, ct);
            await UniTask.Delay(WaitAfterEscapeFadeMilliseconds, cancellationToken: ct);
            await _stateMachine.ChangeStateAsync(GameModeType.Title);
        }

        ///<summary>
        ///暗転して別の階へ移動する
        ///</summary>
        public async UniTask TravelAsync(Transform destination, CancellationToken ct)
        {
            _player.LockInput();
            await _fader.FadeOutAsync(TravelFadeSeconds, ct);
            _player.Warp(destination.position);
            await _fader.FadeInAsync(TravelFadeSeconds, ct);
            _player.UnlockInput();
        }

        ///<summary>
        ///ドアを開けられるようにする
        ///</summary>
        public void UnlockDoor(TargetId door) => _entity.UnlockDoor(door);

        ///<summary>
        ///隠れていた調べる物を出現させる
        ///</summary>
        public void RevealTarget(TargetId target) => _entity.RevealTarget(target);

        ///<summary>
        ///ワールドステートを書き換える
        ///</summary>
        public void SetWorldState(WorldStateType key, int value) => _worldStates.ModifyState(key, value);

        ///<summary>
        ///SEを鳴らす
        ///</summary>
        public void PlaySe(string soundName) => _audio.PlaySe(soundName);

        ///<summary>
        ///切り替え式の物が動いているか
        ///</summary>
        public bool IsTargetActive(TargetId target) => _entity.IsTargetActive(target);

        ///<summary>
        ///指定した照明スイッチが点いているか
        ///</summary>
        public bool IsLightOn(TargetId lightSwitch) => _lightSwitches.Items.Any(s => s.SwitchId == lightSwitch.Value && s.IsOn);

        private void EnterStage(int stageIndex)
        {
            if (stageIndex >= _config.Stages.Count)
            {
                _entity.SetStage(stageIndex, System.Array.Empty<IObjectiveTracker>());
                UpdateRitualState();
                return;
            }

            var stage = _config.Stages[stageIndex];
            _entity.SetStage(stageIndex, stage.Objectives.Select(objective => objective.CreateTracker()).ToArray());
            foreach (var effect in stage.OnStart) effect.Apply(this);
            // 目標の無いステージ（演出だけのステージ）はすぐ次へ進む
            CompleteStageIfDone();
        }

        private void CompleteStageIfDone()
        {
            if (IsCleared || !_entity.Trackers.All(tracker => tracker.IsCompleted)) return;

            var stageIndex = _entity.StageIndex.CurrentValue;
            foreach (var effect in _config.Stages[stageIndex].OnComplete) effect.Apply(this);
            EnterStage(stageIndex + 1);
        }

        // 儀式が進行中の間だけ RitualActive を立て、ゴーストに照明を消しに来させる
        private void UpdateRitualState()
        {
            bool isRunning = _entity.Trackers.OfType<RitualTracker>().Any(ritual => ritual.IsRunning);
            _worldStates.ModifyState(WorldStateType.RitualActive, isRunning ? 1 : 0);
        }
    }
}
