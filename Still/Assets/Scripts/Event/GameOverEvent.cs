using Cysharp.Threading.Tasks;
using DG.Tweening;
using Still.Player;
using Still.Player.View;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameOverEvent : IEvent
{
    private Canvas _gameOverCanvas;
    private Image _fadeImage;
    private LastPosition _lastPosition;
    private PlayerView _player;
    private bool _isFinished;

    public bool IsFinished => _isFinished;

    public void Initialize()
    {
        var gameInstaller = GameObject.FindAnyObjectByType<GameInstaller>();
        gameInstaller.SANValueModel.ModifySAN(-10);
        if (gameInstaller == null)
        {
            Debug.LogError("GameInstaller not found!");
            return;
        }

        // GameInstallerのプロパティから取得
        _lastPosition = gameInstaller.LastPosition;
        _gameOverCanvas = gameInstaller.UICanvas;
        _fadeImage = gameInstaller.FadeImage;
        _player = GameObject.FindAnyObjectByType<PlayerView>();

        Debug.Log($"Initialize完了: Canvas={_gameOverCanvas != null}, Image={_fadeImage != null}, Player={_player != null}, LastPosition={_lastPosition != null}");
    }

    public void OnEvent()
    {
        ShowGameOver().Forget();
    }

    private async UniTask ShowGameOver()
    {
        if (_player == null || _fadeImage == null || _lastPosition == null)
        {
            Debug.LogError("GameOverEvent: 必要なオブジェクトがnullです");
            _isFinished = true;
            return;
        }

        // プレイヤー入力を無効化
        _player.DisablePlayerInput();

        // 画像を表示し、完全に不透明にする（暗転）
        _fadeImage.gameObject.SetActive(true);
        var color = _fadeImage.color;
        color.a = 1f;
        _fadeImage.color = color;

        // 暗転中にプレイヤーをテレポート（見えない）
        _player.transform.position = _lastPosition.GetLastPosition();

        // 2秒かけてフェードアウト（透明にする）
        await DOTween.ToAlpha(
            () => _fadeImage.color,
            x => _fadeImage.color = x,
            1f,
            2f
            ).AsyncWaitForCompletion();

        // 画像を非表示
        _fadeImage.gameObject.SetActive(false);

        // プレイヤー入力を有効化
        _player.EnablePlayerInput();

        _isFinished = true;
    }
}