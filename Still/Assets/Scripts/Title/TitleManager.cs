using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera _startCam;
    [SerializeField]
    private CinemachineCamera _endCam;
    [SerializeField]
    private Animator[] _doors;
    [SerializeField]
    private int _startDelay = 800;
    private PlayerInputActions _inputActions;
    private void Awake()
    {
        SetPriority(1, 0);
        _inputActions = new PlayerInputActions();
    }
    private void SetPriority(int startCam, int endCam)
    {
        _startCam.Priority = startCam;
        _endCam.Priority = endCam;
    }
    private void Start()
    {
        AudioManager.Instance.PlayBGM("Title");
    }
    private void OnEnable()
    {
        _inputActions.Title.Enable();
        _inputActions.Title.Click.performed += GameStart;
    }

    private void GameStart(InputAction.CallbackContext context)
    {
        OpenDoors().Forget();
    }
    private async UniTask OpenDoors()
    {
        AudioManager.Instance.FadeBGM(0.5f);
        SetPriority(0, 1);
        await UniTask.Delay(_startDelay);
        SceneManager.LoadScene("MasterScene");
    }
    private void OnDisable()
    {
        _inputActions.Title.Click.performed -= GameStart;
        _inputActions.Title.Disable();
    }
}
