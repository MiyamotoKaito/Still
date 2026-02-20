using UnityEngine;

public class TeleportPos : MonoBehaviour
{
    public GameObject[] TeleportPosList => _teleportPos;
    [SerializeField, ReadOnly]
    private GameObject[] _teleportPos;
    private void Awake()
    {
        _teleportPos = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _teleportPos[i] = transform.GetChild(i).gameObject;
        }
    }
}
