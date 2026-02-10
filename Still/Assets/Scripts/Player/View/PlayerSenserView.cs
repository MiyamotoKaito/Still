using System;
using UnityEngine;

public class PlayerSensorView : MonoBehaviour
{
    public event Action<RaycastHit> OnHitDetected;
    public event Action OnHitLost;
    [SerializeField] private PlayerConfig config;
    [SerializeField] private LayerMask _layerMask;
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, config.RayDistance, _layerMask))
        {
            OnHitDetected?.Invoke(hit);
        }
        else
        {
            OnHitLost?.Invoke();
        }
    }
}