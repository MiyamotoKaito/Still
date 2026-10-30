using System;
using System.Collections.Generic;
using System.Linq;
using Still.Common;
using Still.Utility;
using VContainer.Unity;

namespace Still.Escape
{
    ///<summary>
    ///普通のドア（出口以外）を調べたら開け閉めする
    ///</summary>
    public sealed class DoorPresenter : IInitializable, IDisposable
    {
        public DoorPresenter(SceneComponentList<DoorView> doors, IAudioService audio)
        {
            // 出口の扉は鍵の判定があるのでEscapePresenterが扱う
            _doors = doors.Items.Where(door => door is not LockedDoor).ToArray();
            _audio = audio;
        }

        private readonly IReadOnlyList<DoorView> _doors;
        private readonly IAudioService _audio;
        private readonly List<(DoorView Door, Action Handler)> _handlers = new();

        ///<summary>
        ///購読を始める
        ///</summary>
        public void Initialize()
        {
            foreach (var door in _doors)
            {
                Action handler = () => Toggle(door);
                door.OnInteracted += handler;
                _handlers.Add((door, handler));
            }
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose()
        {
            foreach (var (door, handler) in _handlers)
            {
                if (door != null) door.OnInteracted -= handler;
            }
            _handlers.Clear();
        }

        private void Toggle(DoorView door)
        {
            door.Toggle();
            _audio.PlaySe("DoorOpen");
        }
    }
}
