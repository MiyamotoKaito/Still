using UnityEngine;

public interface IPlayerController
{
    Vector2 CurrentMoveValue { get; }
    bool IsDash { get; }
    void Move(Vector3 direction, float speed);
    void EnablePlayerInput();
    void DisablePlayerInput();
}