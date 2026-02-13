using Still.Object.Door.View;
using Still.Object.Key;
using UnityEngine;
[System.Serializable]
public class LockedDoorAndKeyPairs
{
    public LockedDoor LockedDoor => _lockedDoor;
    public KeyView KeyView => _keyView;

    [SerializeField]
    LockedDoor _lockedDoor;
    [SerializeField]
    KeyView _keyView;
}
