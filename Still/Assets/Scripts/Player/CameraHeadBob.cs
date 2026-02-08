using System;
using Still.Player.View;
using Unity.Cinemachine;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    [Header("Head Bob")]
    [SerializeField] private float _amplitude = 1;
    [SerializeField] private float _frequency = 1;
    private PlayerView _playerview;
    private CinemachineBasicMultiChannelPerlin _noise;
    private float _defaultAmplitude;
    private float _defaultFrequency;
    private void Awake()
    {
        _playerview = GetComponent<PlayerView>();
        _noise = FindAnyObjectByType<CinemachineBasicMultiChannelPerlin>();
        _defaultAmplitude = _amplitude;
        _defaultFrequency = _frequency;
        _noise.AmplitudeGain = _amplitude;
        _noise.FrequencyGain = _frequency;
    }

    private void LateUpdate()
    {
        if (_playerview.CurrentMoveValue.magnitude > 0.1f)
        {
            _amplitude = _defaultAmplitude;
            _frequency = _defaultFrequency;
            _noise.AmplitudeGain = _amplitude;
            _noise.FrequencyGain = _frequency;
        }
        else
        {
            _noise.AmplitudeGain = 0;
            _noise.FrequencyGain = 0;
        }
    }
}