using UnityEngine;
namespace Still.Player.View
{
    public class PlayerView : MonoBehaviour, IPlayerController
    {
        public void UpdatePosition(Vector3 pos)
        {
            this.transform.position = pos;
        }
    }
}