using UnityEngine;

namespace Still.Common
{
    ///<summary>
    ///SE/BGMの再生
    ///</summary>
    public interface IAudioService
    {
        ///<summary>
        ///2DでSEを再生する
        ///</summary>
        void PlaySe(string soundName);

        ///<summary>
        ///指定したAudioSourceから3DでSEを再生する
        ///</summary>
        void PlaySe(string soundName, AudioSource source);

        ///<summary>
        ///BGMをループ再生する
        ///</summary>
        void PlayBgm(string soundName);

        ///<summary>
        ///指定したAudioSourceから3DでBGMをループ再生する
        ///</summary>
        void PlayBgm(string soundName, AudioSource source);

        ///<summary>
        ///BGMを指定秒数でフェードアウトする
        ///</summary>
        void FadeOutBgm(float duration);

        ///<summary>
        ///BGMを止める
        ///</summary>
        void StopBgm();

        ///<summary>
        ///指定したAudioSourceの再生を止める
        ///</summary>
        void Stop(AudioSource source);
    }
}
