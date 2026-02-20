using UnityEngine;

public class LastPosition
{
    public Vector3 GetLastPosition()
    {
        float x = PlayerPrefs.GetFloat("LastPositionX", 0f);
        float y = PlayerPrefs.GetFloat("LastPositionY", 0f);
        float z = PlayerPrefs.GetFloat("LastPositionZ", 0f);
        return new Vector3(x, y, z);
    }
    public void SavePosition(Vector3 position)
    {
        PlayerPrefs.SetFloat("LastPositionX", position.x);
        PlayerPrefs.SetFloat("LastPositionY", position.y);
        PlayerPrefs.SetFloat("LastPositionZ", position.z);
        PlayerPrefs.Save();
    }
}