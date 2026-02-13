using Still.Object.Door.Model;
using Still.Object.Door.View;
using Still.Object.Key;
using Still.Object.Key.Model;
using UnityEngine;

public class LockedDoorPresenter
{
    private LockedDoorModel _model;
    private LockedDoor _door;
    private KeyView _keyView;

    public LockedDoorPresenter(LockedDoorModel model, LockedDoor door, KeyView keyView)
    {
        _model = model;
        _door = door;
        _keyView = keyView;

        _keyView.OnKeyCollected += HandleDoorLocked;
    }

    public void HandleDoorLocked()
    {
        _model.Unlock();
        Debug.Log("鍵が掛かっているドアを開けれるようになった");
        _door.GetKey(_model.isLocked);
    }
    public void Dispose()
    {
        _door.OnDoorLocked -= HandleDoorLocked;
    }
}
