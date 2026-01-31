using UnityEngine;
namespace Still.Player.Mode
{
    public class PlayerModel
    {
        public Vector3 Position { get; set; }
        public float Speed { get; set; } = 5f;

        public void Move(Vector3 direction, float deltaTime)
        {
            Position += direction * Speed * deltaTime;
        }
    }
}