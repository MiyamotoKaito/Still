using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Still.Common;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Still.Escape
{
    ///<summary>
    ///調べるもの・鍵・出口の扉の入力をUsecaseへ渡し、進み具合を画面へ反映する
    ///</summary>
    public sealed class EscapePresenter : IInitializable, IStartable, IDisposable
    {
        public EscapePresenter(EscapeEntity entity, EscapeUsecase usecase, Tv tv, RecorderPlayer recorder, Book book,
                               KeyView key, LockedDoor exitDoor, EscapeChecklistView checklist,
                               IPlayerAvatar player, ICheckpoint checkpoint, IAudioService audio, IObjectResolver resolver)
        {
            _entity = entity;
            _usecase = usecase;
            _tv = tv;
            _recorder = recorder;
            _book = book;
            _key = key;
            _exitDoor = exitDoor;
            _checklist = checklist;
            _player = player;
            _checkpoint = checkpoint;
            _audio = audio;
            _resolver = resolver;
        }

        private readonly EscapeEntity _entity;
        private readonly EscapeUsecase _usecase;
        private readonly Tv _tv;
        private readonly RecorderPlayer _recorder;
        private readonly Book _book;
        private readonly KeyView _key;
        private readonly LockedDoor _exitDoor;
        private readonly EscapeChecklistView _checklist;
        private readonly IPlayerAvatar _player;
        private readonly ICheckpoint _checkpoint;
        private readonly IAudioService _audio;
        private readonly IObjectResolver _resolver;
        private readonly CompositeDisposable _disposables = new();
        private readonly CancellationTokenSource _cts = new();

        ///<summary>
        ///入力と状態の購読を始める
        ///</summary>
        public void Initialize()
        {
            // 本の演出はSerializeReferenceの値なのでコンテナが生成できず、ここで注入する
            if (_book.PickUpEvent != null) _resolver.Inject(_book.PickUpEvent);
            _key.Hide();

            _tv.OnInteracted += HandleTv;
            _recorder.OnInteracted += HandleRecorder;
            _book.OnInteracted += HandleBook;
            _key.OnInteracted += HandleKey;
            _exitDoor.OnInteracted += HandleExitDoor;

            _entity.Progress.Subscribe(_checklist.ShowProgress).AddTo(_disposables);
            _entity.Progress
                .Select(progress => progress.IsKeyUnlocked)
                .DistinctUntilChanged()
                .Where(isUnlocked => isUnlocked)
                .Subscribe(_ => SpawnKey())
                .AddTo(_disposables);
            _entity.HasKey.Subscribe(hasKey => _exitDoor.SetLocked(!hasKey)).AddTo(_disposables);
        }

        ///<summary>
        ///開始位置を最初のチェックポイントにする
        ///</summary>
        public void Start()
        {
            _checkpoint.Save(_player.Transform.position);
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose()
        {
            _tv.OnInteracted -= HandleTv;
            _recorder.OnInteracted -= HandleRecorder;
            _book.OnInteracted -= HandleBook;
            _key.OnInteracted -= HandleKey;
            _exitDoor.OnInteracted -= HandleExitDoor;
            _disposables.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }

        private void HandleTv()
        {
            SaveCheckpoint();
            _tv.TurnOn();
            _audio.PlaySe("Noise", _tv.AudioSource);
            _usecase.Achieve(EscapeItemType.Tv);
        }

        private void HandleRecorder()
        {
            SaveCheckpoint();
            _recorder.HideUI();
            _audio.PlayBgm("Recorder1", _recorder.AudioSource);
            _usecase.Achieve(EscapeItemType.Music);
        }

        private void HandleBook()
        {
            SaveCheckpoint();
            _book.PickUp();
            _book.PickUpEvent?.Play();
            _usecase.Achieve(EscapeItemType.Book);
        }

        private void HandleKey()
        {
            _key.Hide();
            _usecase.CollectKey();
        }

        private void HandleExitDoor()
        {
            if (!_usecase.CanOpenExit)
            {
                _audio.PlaySe("KeyLocked");
                return;
            }

            _exitDoor.Toggle();
            _audio.PlaySe("DoorOpen");
            _usecase.EscapeAsync(_cts.Token).Forget();
        }

        private void SpawnKey()
        {
            var positions = _key.SpawnPositions;
            if (positions.Count == 0)
            {
                Debug.LogError("[EscapePresenter] 鍵の出現位置が設定されていません");
                return;
            }

            _key.Show(positions[Random.Range(0, positions.Count)].transform.position);
            _checklist.ShowFindKey();
        }

        private void SaveCheckpoint()
        {
            _checkpoint.Save(_player.Transform.position);
        }
    }
}
