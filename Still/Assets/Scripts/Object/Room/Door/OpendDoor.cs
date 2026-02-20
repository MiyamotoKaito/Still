using UnityEngine;

public class OpendDoor : MonoBehaviour
{
    private Animator _anim;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Camera>(out var camera))
        {
            _anim.SetTrigger("Open");
            AudioManager.Instance.PlaySE("DoorOpen");
        }
    }
}
