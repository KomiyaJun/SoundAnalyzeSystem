using MyGame.AudioSetting;
using UnityEngine;

public class NandemoDebug : MonoBehaviour
{
    float oldSpeed = 1.0f;
    float speed = 1.0f;

    public void DebugFloat(float value)
    {
        Debug.Log($"Nandemo Debugger float value : {value} ");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            speed -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            speed += 0.1f;
        }

        if (oldSpeed != speed)
        {
            oldSpeed = speed;
            SoundService.Instance.SetBgmPlaybackSpeed(speed);

        }
    }
}
