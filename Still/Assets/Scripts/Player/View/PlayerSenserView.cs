using System;
using UnityEngine;

public class PlayerSensorView : MonoBehaviour
{
    public event Action<RaycastHit> OnHitDetected;
    public RaycastHit CurrentHit => _hit;
    [SerializeField] private PlayerConfig config;
    private RaycastHit _hit;
    private GameObject _lastHitObject;
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, config.RayDistance))
        {
            if (hit.collider.gameObject != _lastHitObject)
            {
                Debug.Log($"Hit detected: {hit.collider.gameObject.name}");
                _hit = hit;
                _lastHitObject = hit.collider.gameObject;
                OnHitDetected?.Invoke(hit);
            }
        }
    }
}