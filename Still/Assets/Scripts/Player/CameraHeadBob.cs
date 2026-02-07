using System;
using Still.Player.View;
using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    [Header("Head Bob")]
    [SerializeField] private float _horizontalBob = 0.05f;
    [SerializeField] private float _verticalBob = 0.05f;
    [SerializeField] private float _shakeSpeed = 8f;

    private CameraView _camera;
    private PlayerView _player;
    private Vector3 _defaultPos;
    private float _timer;

    private void Awake()
    {
        _camera = FindAnyObjectByType<CameraView>();
        _player = GetComponent<PlayerView>();
        _defaultPos = _camera.transform.localPosition;
    }

    private void LateUpdate()
    {
        if (_player.CurrentMoveValue.magnitude > 0.1f)
        {
            _timer += Time.deltaTime * _shakeSpeed;

            float x = Mathf.Cos(_timer) * _horizontalBob;
            float y = Mathf.Sin(_timer * 2f) * _verticalBob;

            Vector3 targetPos = _defaultPos + new Vector3(x, y, 0);

            _camera.transform.localPosition = Vector3.Lerp(
                _camera.transform.localPosition,
                targetPos,
                Time.deltaTime * _shakeSpeed
            );
        }
        else
        {
            _timer = 0f;
            _camera.transform.localPosition = Vector3.Lerp(
                _camera.transform.localPosition,
                _defaultPos,
                Time.deltaTime * _shakeSpeed
            );
        }
    }
}