using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class SoundData
    {
        public AudioClip Clip => _clip;
        public string Name => _name;
        public float Volume => _volume;

        [SerializeField] private AudioClip _clip;
        [SerializeField] private string _name;
        [SerializeField, Range(0, 1)] private float _volume;
    }
    [Header("プレイヤー")]
    [ReadOnly, SerializeField] private AudioSource _bgmPlayer;

    [Header("SEリスト")]
    [SerializeField] private List<SoundData> _seList;
    [Header("BGMリスト")]
    [SerializeField] private List<SoundData> _bgmList;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        _bgmPlayer = GetComponentInChildren<AudioSource>();
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volume"></param>
    public void PlaySE(string name)
    {
        foreach (var se in _seList)
        {
            if (se.Name == name)
            {
                GameObject sePlayer = new GameObject("SEPlayer");
                sePlayer.transform.SetParent(transform);

                var source = sePlayer.AddComponent<AudioSource>();
                source.volume = se.Volume;
                source.spatialBlend = 0f;
                source.clip = se.Clip;
                source.Play();
                Destroy(sePlayer, se.Clip.length);
            }
        }
    }
    /// <summary>
    /// AudioSourceを指定してSE再生
    /// </summary>
    /// <param name="name"></param>
    /// <param name="source"></param>
    public void PlaySE(string name, AudioSource source)
    {
        foreach (var se in _seList)
        {
            if (se.Name == name)
            {
                GameObject sePlayer = new GameObject("SEPlayer");
                sePlayer.transform.SetParent(transform);

                source.volume = se.Volume;
                source.spatialBlend = 1f;
                source.clip = se.Clip;
                source.Play();
                Destroy(sePlayer, se.Clip.length);
            }
        }
    }
    /// <summary>
    /// BGM再生(ループ)
    /// </summary>
    /// <param name="name"></param>
    public void PlayBGM(string name)
    {
        foreach (var bgm in _bgmList)
        {
            if (bgm.Name == name)
            {
                _bgmPlayer.loop = true;
                _bgmPlayer.volume = bgm.Volume;
                _bgmPlayer.resource = bgm.Clip;
                _bgmPlayer.Play();
            }
        }
    }
    /// <summary>
    /// AudioSourceを指定してBGM再生(ループ)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="source"></param>
    public void PlayBGM(string name, AudioSource source)
    {
        foreach (var bgm in _bgmList)
        {
            if (bgm.Name == name)
            {
                source.loop = true;
                source.volume = bgm.Volume;
                source.spatialBlend = 1f;
                source.resource = bgm.Clip;
                source.Play();
            }
        }
    }
    /// <summary>
    /// 音楽のフェード
    /// </summary>
    /// <param name="fadeTime"></param>
    public void FadeBGM(float fadeTime)
    {
        DOTween.To(() => _bgmPlayer.volume, x => _bgmPlayer.volume = x, 0f, fadeTime);
    }
    /// <summary>
    /// BGMのストップ
    /// </summary>
    public void StopBGM()
    {
        _bgmPlayer.Stop();
    }
    /// <summary>
    /// AudioSourceを指定してBGMのストップ
    /// </summary>
    /// <param name="source"></param>
    public void StopBGM(AudioSource source)
    {
        source.Stop();
    }
}