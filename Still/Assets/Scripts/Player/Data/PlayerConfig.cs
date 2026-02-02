using UnityEngine;
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Player/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public float PlayerDefaultSpeed;
    public float PlayerMaxSpeed;
    public float DefaultStaminaValue;
    public float DefaultSANValue;
}
