
using UnityEngine;

public class CustomCrashCrate : MonoBehaviour
{
    [Header("Whole Crate")]
    public MeshRenderer wholeCrate;
    public BoxCollider boxCollider;
    [Header("Fractured Crate")]
    public GameObject fracturedCrate;
    [Header("Audio")]
    public AudioSource crashAudioClip;
    [Header("Collision")]
    public float requiredForceToBreak;


    public void RunCollision(float hitForce, float amplitude)
    {
        ExperimentData.DataFile.AddData(gameObject.name, requiredForceToBreak, hitForce, amplitude);
        if (hitForce >= requiredForceToBreak)
        {
            wholeCrate.enabled = false;
            boxCollider.enabled = false;
            fracturedCrate.SetActive(true);
            crashAudioClip.Play();
        }
    }
}
