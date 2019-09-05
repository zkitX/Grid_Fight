using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using System.Linq;
#if UNITY_SWITCH
using Rewired.Platforms.Switch;
#endif

public class VibrationController : MonoBehaviour
{
    public static VibrationController Instance;

    public List<VibrationTypeClass> VibrationType = new List<VibrationTypeClass>();


    private void Awake()
    {
        Instance = this;

        foreach (VibrationTypeClass item in VibrationType)
        {
            item.vibFile = Resources.Load<TextAsset>(item.VibrationT.ToString() + ".bnvib").bytes;
        }
    }

    public void CustomVibration(int playerId, VibrationType vT)
    {
#if UNITY_SWITCH
        VibrationTypeClass vibrationToFire = VibrationType.Where(r => r.VibrationT == vT).First();
        foreach (Joystick joystick in ReInput.players.GetPlayer(playerId).controllers.Joysticks)
        {
            // Get the Switch Gamepad Extension from the Joystick
            ISwitchVibrationDevice ext = joystick.GetExtension<ISwitchVibrationDevice>();
            if (ext != null)
            {
                for (int i = 0; i < ext.vibrationMotorCount; i++)
                {
                    ext.SetVibration(i, vibrationToFire.vibFile); // Pass the BNVIB file as a byte array
                }
            }
        }
#endif
    }

    public void Vibration(int playerId)
    {
#if UNITY_SWITCH
        foreach (Joystick joystick in ReInput.players.GetPlayer(playerId).controllers.Joysticks)
        {
            // Get the Switch Gamepad Extension from the Joystick
            ISwitchVibrationDevice ext = joystick.GetExtension<ISwitchVibrationDevice>();
            if (ext != null)
            {

                // Create a new SwitchVibration
                // You could also use the SetVibration overload that takes 5 float values instead
                SwitchVibration vib = new SwitchVibration()
                {
                    amplitudeLow = 1f,
                    frequencyLow = 50,
                    amplitudeHigh = 0.5f,
                    frequencyHigh = 100
                };

                // Send the vibration to the controller
                for (int i = 0; i < ext.vibrationMotorCount; i++)
                {
                    ext.SetVibration(i, vib); // set vibration in each motor
                }
            }
        }
#endif
    }
}

[System.Serializable]
public class VibrationTypeClass
{
    public string FileName;
    public VibrationType VibrationT;
    public byte[] vibFile;
}

public enum VibrationType
{
    a,
    b
}

