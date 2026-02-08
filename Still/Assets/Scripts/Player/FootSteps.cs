using Still.Player.View;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] private FootStepSound[] _footStepSounds;
    [SerializeField] private float _stepInterval = 0.5f;

    private AudioSource _audioSource;
    private float _stepTimer;
    private PlayerView _player;

    [System.Serializable]
    public class FootStepSound
    {
        public Material Material => _material;
        public AudioClip Clip => _clip;

        [SerializeField] private Material _material;
        [SerializeField] private AudioClip _clip;
    }

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerView>();
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        // プレイヤーが移動中
        if (_player.CurrentMoveValue != Vector2.zero)
        {
            _stepTimer -= Time.deltaTime;

            if (_stepTimer <= 0)
            {
                PlayFootStep();
                _stepTimer = _stepInterval;
            }
        }
        else
        {
            // 停止時は音を止める
            _audioSource.Stop();
        }
    }

    private void PlayFootStep()
    {
        // 既に再生中なら何もしない
        if (_audioSource.isPlaying) return;

        // 下方向にRaycast
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer != null && renderer.sharedMaterial != null)
            {
                // マテリアルに対応する足音を探す
                foreach (var footStepSound in _footStepSounds)
                {
                    if (footStepSound.Material == renderer.sharedMaterial)
                    {
                        _audioSource.PlayOneShot(footStepSound.Clip);
                        return;
                    }
                }
            }

            // マテリアルが一致しない場合、デフォルトの足音を再生
            if (_footStepSounds.Length > 0 && _footStepSounds[0].Clip != null)
            {
                _audioSource.PlayOneShot(_footStepSounds[0].Clip);
            }
        }
    }
}