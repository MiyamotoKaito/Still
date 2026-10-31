using System.Collections.Generic;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///恐怖演出の音を鳴らすAudioSourceを用意する
    ///</summary>
    public class FearAudioView : MonoBehaviour
    {
        private const float OneShotMaxDistance = 15f;

        private readonly Dictionary<string, AudioSource> _loops = new();

        ///<summary>
        ///心音などのループ用AudioSource（音名ごとに1つ。耳元で鳴る2D）
        ///</summary>
        public AudioSource GetLoopSource(string soundName)
        {
            if (_loops.TryGetValue(soundName, out var source)) return source;

            var go = new GameObject("FearLoop_" + soundName);
            go.transform.SetParent(transform, false);
            source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0f;
            _loops[soundName] = source;
            return source;
        }

        ///<summary>
        ///指定位置で一度だけ鳴らす3DのAudioSource（一定時間後に消える）
        ///</summary>
        public AudioSource CreateOneShotSourceAt(Vector3 position, float lifetime)
        {
            var go = new GameObject("FearOneShot");
            go.transform.SetParent(transform, false);
            go.transform.position = position;
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            source.maxDistance = OneShotMaxDistance;
            Destroy(go, lifetime);
            return source;
        }
    }
}
