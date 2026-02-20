using Cysharp.Threading.Tasks;
using Still.Player.View;
using Unity.Cinemachine;
using UnityEngine;
[System.Serializable]
public class BookShellLightOffEvent : IEvent
{
    public bool IsFinished => _isFinished;

    [SerializeField]
    private CinemachineCamera _playerCamera;
    [SerializeField]
    private CinemachineCamera _bookCam;
    [SerializeField]
    private Animator _bookShellAnimator;
    private PlayerView _playerView;
    private bool _isFinished = false;
    public void Initialize()
    {
        _bookCam.Priority = 0;
        _isFinished = false;
        _playerView = GameObject.FindAnyObjectByType<PlayerView>();
    }

    public void OnEvent()
    {
        PlayAnimation().Forget();
    }
    private async UniTask PlayAnimation()
    {
        _bookCam.transform.position = _playerCamera.transform.position;
        _bookCam.LookAt = _bookShellAnimator.transform;
        _playerView.DisablePlayerInput();
        _bookCam.Priority = 10;
        await UniTask.Delay(700); // カメラ切り替えのために待機
        _bookShellAnimator.SetTrigger("LightOff");
        await UniTask.Delay(2000); // アニメーションの長さに合わせて待機
        _playerView.EnablePlayerInput();
        _bookCam.Priority = 0;
        _isFinished = true;
    }
}
