using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using UnityEngine;

namespace Still.Utility
{
    ///<summary>
    ///名前でSE/BGMを再生する。Rootシーンに1つだけ置く
    ///</summary>
    public class AudioManager : MonoBehaviour, IAudioService
    {
        [Serializable]
        public class SoundData
        {
            ///<summary>再生するクリップ</summary>
            public AudioClip Clip => _clip;
            ///<summary>呼び出し名</summary>
            public string Name => _name;
            ///<summary>音量</summary>
            public float Volume => _volume;

            [SerializeField] private AudioClip _clip;
            [SerializeField] private string _name;
            [SerializeField, Range(0, 1)] private float _volume;
        }

        [Header("SEリスト")]
        [SerializeField] private List<SoundData> _seList;
        [Header("BGMリスト")]
        [SerializeField] private List<SoundData> _bgmList;

        private AudioSource _bgmPlayer;
        private CancellationTokenSource _fadeCts;

        ///<summary>
        ///2DでSEを再生する
        ///</summary>
        public void PlaySe(string soundName)
        {
            var se = Find(_seList, soundName);
            if (se == null) return;

            var sePlayer = new GameObject($"SE_{soundName}");
            sePlayer.transform.SetParent(transform);
            var source = sePlayer.AddComponent<AudioSource>();
            source.volume = se.Volume;
            source.spatialBlend = 0f;
            source.clip = se.Clip;
            source.Play();
            Destroy(sePlayer, se.Clip.length);
        }

        ///<summary>
        ///指定したAudioSourceから3DでSEを再生する
        ///</summary>
        public void PlaySe(string soundName, AudioSource source)
        {
            var se = Find(_seList, soundName);
            if (se == null) return;

            source.loop = false;
            source.volume = se.Volume;
            source.spatialBlend = 1f;
            source.clip = se.Clip;
            source.Play();
        }

        ///<summary>
        ///BGMをループ再生する
        ///</summary>
        public void PlayBgm(string soundName)
        {
            var bgm = Find(_bgmList, soundName);
            if (bgm == null) return;

            CancelFade();
            _bgmPlayer.loop = true;
            _bgmPlayer.volume = bgm.Volume;
            _bgmPlayer.resource = bgm.Clip;
            _bgmPlayer.Play();
        }

        ///<summary>
        ///指定したAudioSourceから3DでBGMをループ再生する
        ///</summary>
        public void PlayBgm(string soundName, AudioSource source)
        {
            var bgm = Find(_bgmList, soundName);
            if (bgm == null) return;

            source.loop = true;
            source.volume = bgm.Volume;
            source.spatialBlend = 1f;
            source.resource = bgm.Clip;
            source.Play();
        }

        ///<summary>
        ///BGMを指定秒数でフェードアウトする
        ///</summary>
        public void FadeOutBgm(float duration)
        {
            CancelFade();
            _fadeCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            FadeOutAsync(duration, _fadeCts.Token).Forget();
        }

        ///<summary>
        ///BGMを止める
        ///</summary>
        public void StopBgm()
        {
            CancelFade();
            _bgmPlayer.Stop();
        }

        ///<summary>
        ///指定したAudioSourceの再生を止める
        ///</summary>
        public void Stop(AudioSource source)
        {
            source.Stop();
        }

        private void Awake()
        {
            _bgmPlayer = GetComponentInChildren<AudioSource>();
        }

        private void OnDestroy()
        {
            CancelFade();
        }

        private SoundData Find(List<SoundData> list, string soundName)
        {
            var data = list.Find(s => s.Name == soundName);
            if (data == null) Debug.LogWarning($"[AudioManager] {soundName} が登録されていません");
            return data;
        }

        private async UniTaskVoid FadeOutAsync(float duration, CancellationToken ct)
        {
            float from = _bgmPlayer.volume;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _bgmPlayer.volume = Mathf.Lerp(from, 0f, elapsed / duration);
                await UniTask.Yield(ct);
            }
            _bgmPlayer.volume = 0f;
        }

        private void CancelFade()
        {
            _fadeCts?.Cancel();
            _fadeCts?.Dispose();
            _fadeCts = null;
        }
    }
}
