using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadManager : MonoBehaviour
{
    public static GamepadManager instance;

    private Gamepad gamepad;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;

        gamepad = Gamepad.current;
    }

    public void Rumble(float lowFreq, float highFreq, float duration)
    {
        gamepad.SetMotorSpeeds(lowFreq, highFreq);
        Invoke("StopRumble", duration);
    }

    private void StopRumble()
    {
        gamepad.SetMotorSpeeds(0f, 0f);
    }
}
