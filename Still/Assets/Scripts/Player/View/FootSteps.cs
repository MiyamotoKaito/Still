using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Still.Player
{
    ///<summary>
    ///床のマテリアルに応じた足音を鳴らす
    ///</summary>
    public class FootSteps : MonoBehaviour
    {
        [Serializable]
        public class FootStepSound
        {
            ///<summary>対象の床マテリアル</summary>
            public Material Material => _material;
            ///<summary>ランダムに鳴らす足音</summary>
            public List<AudioClip> Clip => _clip;

            [SerializeField] private Material _material;
            [SerializeField] private List<AudioClip> _clip;
        }

        [SerializeField] private FootStepSound[] _footStepSounds;
        [SerializeField] private float _stepInterval = 0.5f;

        private AudioSource _audioSource;
        private float _stepTimer;
        private PlayerView _player;

        private void Awake()
        {
            _player = GetComponent<PlayerView>();
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void Update()
        {
            if (_player.CurrentMoveValue == Vector2.zero)
            {
                _audioSource.Stop();
                return;
            }

            _stepTimer -= Time.deltaTime;
            if (_stepTimer > 0) return;

            PlayFootStep();
            _stepTimer = _stepInterval;
        }

        private void PlayFootStep()
        {
            if (_audioSource.isPlaying) return;
            if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f)) return;

            _audioSource.pitch = _player.IsDash ? 2.0f : 1.0f;
            var clips = FindClips(hit.collider.GetComponent<Renderer>());
            if (clips == null || clips.Count == 0) return;

            _audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)]);
        }

        // 床のマテリアルに対応する足音。無ければ先頭をデフォルトとして使う
        private List<AudioClip> FindClips(Renderer floor)
        {
            if (floor != null && floor.sharedMaterial != null)
            {
                foreach (var sound in _footStepSounds)
                {
                    if (sound.Material == floor.sharedMaterial) return sound.Clip;
                }
            }
            return _footStepSounds.Length > 0 ? _footStepSounds[0].Clip : null;
        }
    }
}
