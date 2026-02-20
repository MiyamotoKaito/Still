using System;
using UnityEngine;

public class PlayerSensorView : MonoBehaviour
{
    public event Action<RaycastHit> OnHitPlayerDetected;
    public event Action<RaycastHit> OnHitDetected;
    public event Action OnHitPlayerLost;
    public event Action OnHitLost;
    [SerializeField] private PlayerConfig config;
    [SerializeField] private LayerMask _layerMask;
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit playerHit, config.InteractiveRayDistance, _layerMask))
        {
            OnHitPlayerDetected?.Invoke(playerHit);
        }
        else
        {
            OnHitPlayerLost?.Invoke();
        }
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, config.RayDistance))
        {
            OnHitDetected?.Invoke(hit);
        }
        else
        {
            OnHitLost?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * config.RayDistance);
    }
}