using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VirtualHandBehavior : MonoBehaviour
{
    [Header("Experiment Parameters")]
    public string experimentName;
    public bool enableAudio = true;
    public bool enableHaptics = true;

    [Header("Physics")]
    public Transform parent;
    public float touchThreshold = 1;
    public float impactThreshold = 10;

    [Header("SteamVR")]
    public SteamVR_Action_Vibration Haptics;
    public SteamVR_Input_Sources handType;

    AudioClip touchClip;
    AudioClip impactClip;
    AudioClip heavyImpactClip;

    bool impactCooldown = false;

    new Rigidbody rigidbody;
    AudioSource audioSrc;

    private ExperimentData data;

    // Start is called before the first frame update
    void Start()
    {
        data = new ExperimentData(experimentName); 

        rigidbody = GetComponent<Rigidbody>();
        audioSrc = GetComponent<AudioSource>();

        touchClip = Resources.Load<AudioClip>("Audio/touch");
        impactClip = Resources.Load<AudioClip>("Audio/impact");
        heavyImpactClip = Resources.Load<AudioClip>("Audio/heavy_impact");

        transform.position = parent.position;
        transform.rotation = parent.rotation;

        // Start on cooldown because otherwise the controller might warp inside a box on the first frame, instantly breaking it
        StartCoroutine(ImpactCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExperimentData.DataFile.WriteToFile();
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
        rigidbody.MovePosition(parent.position);
        rigidbody.MoveRotation(parent.rotation);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!impactCooldown)
        {
            CustomCrashCrate crate = collision.gameObject.GetComponent<CustomCrashCrate>();
            if (crate == null) return;

            float impact = collision.relativeVelocity.magnitude;
            float amplitude = 1;
            float hapticDuration = 0;

            if (impact < touchThreshold)
            {
                // Amplitude normalized between zero and touchThreshold
                amplitude = impact / touchThreshold;

                // Collision considered as "touch"
                audioSrc.clip = touchClip;

                // Haptic pulse
            }
            else if (impact < impactThreshold)
            {
                // Amplitude normalized between touchThreshold and impactThreshold (min 0.5)
                amplitude = 0.5f + ((impact - touchThreshold) / (impactThreshold - touchThreshold) / 2);

                // Collision considered as "impact"
                audioSrc.clip = impactClip;

                // Short haptic burst
                hapticDuration = 0.15f;
            }
            else
            {
                // Amplitude maxed out at 1

                // Collision considered as "heavy impact"
                audioSrc.clip = heavyImpactClip;

                // Longer haptic burst
                hapticDuration = 0.3f;  
            }

            if (enableHaptics)
            {
                Haptics.Execute(0, hapticDuration, 50, amplitude, handType);
            }
            if (enableAudio)
            {
                audioSrc.volume = amplitude;
                audioSrc.Play();
            }

            Debug.Log("Impact: " + impact + "; Amplitude: " + amplitude);
            crate.RunCollision(impact, amplitude);
            StartCoroutine(ImpactCooldown());
        }
    }

    IEnumerator ImpactCooldown()
    {
        impactCooldown = true;

        yield return new WaitForSeconds(0.2f);

        impactCooldown = false;
    }
}
