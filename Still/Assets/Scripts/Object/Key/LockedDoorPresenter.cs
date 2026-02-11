using Still.Object.Door.Model;
using Still.Object.Door.View;
using Still.Object.Key.Model;
using UnityEngine;

public class LockedDoorPresenter
{
    private LockedDoorModel _model;
    private LockedDoor _door;
    private KeyModel _keyModel;

    public LockedDoorPresenter(LockedDoorModel model, LockedDoor door, KeyModel keyModel)
    {
        _model = model;
        _door = door;
        _keyModel = keyModel;

        _door.OnDoorLocked += HandleDoorLocked;
    }

    public void HandleDoorLocked()
    {
        if(_keyModel.IsCollected)
        {
            _model.Unlock();
            Debug.Log("鍵が掛かっているドアを開けれるようになった");
        }

        _door.GetKey(_model.isLocked);
    }
}
