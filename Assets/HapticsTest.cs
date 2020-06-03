using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HapticsTest : MonoBehaviour
{
    public SteamVR_Action_Vibration Haptics;

    public SteamVR_Input_Sources handType;

    public float duration = 0.5f;
    public float frequency = 50;
    public float amplitude = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Duration: use zero for pulse, otherwise use desired duration in seconds
            // Frequency: does not work with Oculus Touch
            // Amplitude: works nicely

            Debug.Log("Haptics triggered");
            Haptics.Execute(0, 0, frequency, amplitude, handType);
        }
    }
}
