using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Still.Common;
using Still.Utility;
using VContainer;
using VContainer.Unity;

namespace Still.Escape
{
    ///<summary>
    ///調べる物と鍵付きドアの入力をUsecaseへ渡し、ミッションの進み具合を画面へ反映する
    ///</summary>
    public sealed class MissionPresenter : IInitializable, IStartable, IDisposable
    {
        public MissionPresenter(MissionEntity entity, MissionUsecase usecase,
                                SceneComponentList<MissionTargetView> targets, SceneComponentList<LockedDoor> doors,
                                EscapeChecklistView checklist, IPlayerAvatar player, ICheckpoint checkpoint,
                                IAudioService audio, IObjectResolver resolver)
        {
            _entity = entity;
            _usecase = usecase;
            _targets = targets.Items;
            _doors = doors.Items;
            _checklist = checklist;
            _player = player;
            _checkpoint = checkpoint;
            _audio = audio;
            _resolver = resolver;
        }

        private readonly MissionEntity _entity;
        private readonly MissionUsecase _usecase;
        private readonly IReadOnlyList<MissionTargetView> _targets;
        private readonly IReadOnlyList<LockedDoor> _doors;
        private readonly EscapeChecklistView _checklist;
        private readonly IPlayerAvatar _player;
        private readonly ICheckpoint _checkpoint;
        private readonly IAudioService _audio;
        private readonly IObjectResolver _resolver;
        private readonly List<(LockedDoor Door, Action Handler)> _doorHandlers = new();
        private readonly CompositeDisposable _disposables = new();
        private readonly CancellationTokenSource _cts = new();

        ///<summary>
        ///入力と状態の購読を始める
        ///</summary>
        public void Initialize()
        {
            foreach (var target in _targets)
            {
                // 演出はSerializeReferenceの値なのでコンテナが生成できず、ここで注入する
                if (target.UseEvent != null) _resolver.Inject(target.UseEvent);
                target.ResetState();
                target.OnInteracted += HandleTarget;
            }

            foreach (var door in _doors)
            {
                Action handler = () => HandleDoor(door);
                door.OnInteracted += handler;
                _doorHandlers.Add((door, handler));
                door.SetLocked(!_entity.IsDoorUnlocked(door.Id));
            }

            _entity.OnProgressChanged.Subscribe(_ => RefreshChecklist()).AddTo(_disposables);
            _entity.OnDoorUnlocked.Subscribe(UnlockDoorViews).AddTo(_disposables);
            _entity.OnTargetRevealed.Subscribe(RevealTargetViews).AddTo(_disposables);
        }

        ///<summary>
        ///開始位置を最初のチェックポイントにしてミッションを始める
        ///</summary>
        public void Start()
        {
            _checkpoint.Save(_player.Transform.position);
            _usecase.Begin();
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose()
        {
            foreach (var target in _targets)
            {
                if (target != null) target.OnInteracted -= HandleTarget;
            }
            foreach (var (door, handler) in _doorHandlers)
            {
                if (door != null) door.OnInteracted -= handler;
            }
            _disposables.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }

        private void HandleTarget(MissionTargetView target)
        {
            var outcome = _usecase.HandleInteract(target.Id);
            // 拾う物などは、いまの目標に関係ないときは使わずに残しておく
            if (target.IsConsumable && outcome != InteractOutcomeType.Progressed) return;

            target.Use();
            PlayUseFeedback(target);
            _usecase.ReportTargetActive(target.Id, target.IsToggle && target.IsActive);

            if (outcome == InteractOutcomeType.Progressed) _checkpoint.Save(_player.Transform.position);
        }

        private void PlayUseFeedback(MissionTargetView target)
        {
            if (!string.IsNullOrEmpty(target.UseSe))
            {
                if (target.AudioSource != null) _audio.PlaySe(target.UseSe, target.AudioSource);
                else _audio.PlaySe(target.UseSe);
            }

            if (!string.IsNullOrEmpty(target.UseBgm) && target.AudioSource != null)
            {
                if (target.IsActive) _audio.PlayBgm(target.UseBgm, target.AudioSource);
                else _audio.Stop(target.AudioSource);
            }

            if (target.IsActive) target.UseEvent?.Play();
        }

        private void HandleDoor(LockedDoor door)
        {
            if (!_usecase.IsDoorUnlocked(door.Id))
            {
                _audio.PlaySe("KeyLocked");
                return;
            }

            _audio.PlaySe("DoorOpen");
            if (door.WarpDestination != null)
            {
                _usecase.TravelAsync(door.WarpDestination, _cts.Token).Forget();
                return;
            }

            door.Toggle();
            if (_usecase.IsExit(door.Id)) _usecase.EscapeAsync(_cts.Token).Forget();
        }

        private void UnlockDoorViews(TargetId id)
        {
            foreach (var door in _doors.Where(d => d.Id == id)) door.SetLocked(false);
        }

        private void RevealTargetViews(TargetId id)
        {
            foreach (var target in _targets.Where(t => t.Id == id)) target.Reveal();
        }

        private void RefreshChecklist()
        {
            _checklist.Show(_usecase.CurrentTitle, _entity.Trackers.Select(tracker => tracker.Describe()).ToArray());
        }
    }
}
