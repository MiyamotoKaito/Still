using Unity.Cinemachine;
using UnityEngine;

namespace Still.Title
{
    ///<summary>
    ///タイトル画面のカメラとUI
    ///</summary>
    public class TitleView : MonoBehaviour
    {
        ///<summary>開始演出のカメラ移動を待つ時間</summary>
        public int StartDelayMilliseconds => _startDelay;

        [SerializeField] private CinemachineCamera _startCam;
        [SerializeField] private CinemachineCamera _endCam;
        [SerializeField] private int _startDelay = 800;
        [SerializeField] private Canvas _canvas;

        ///<summary>
        ///タイトル表示の状態にする
        ///</summary>
        public void ShowTitle()
        {
            SetPriority(1, 0);
            _canvas.enabled = true;
        }

        ///<summary>
        ///UIを消して奥へ進むカメラ演出を始める
        ///</summary>
        public void PlayStartTransition()
        {
            _canvas.enabled = false;
            SetPriority(0, 1);
        }

        private void SetPriority(int startCam, int endCam)
        {
            _startCam.Priority = startCam;
            _endCam.Priority = endCam;
        }
    }
}
