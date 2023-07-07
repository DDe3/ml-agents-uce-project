using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudio : MonoBehaviour
{

    public AudioSource runningSound;
    public float runningMaxVolume;
    public float runningMaxPitch;
    public AudioSource iddleSound;
    public float iddleMaxVolume;
    private float speedRatio;
    private CarController carController;
    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        float speedSign;
        if (carController) 
        {
            float speed = carController.GetSpeedRatio();
            speedSign = Mathf.Sign(speed);
            speedRatio = Mathf.Abs(speed);
        }
        /*
        if (speedRatio>limiterEngage) 
        {
            revlimiter = (Mathf.Sin(Time.time * limiterFrecuency) + 1f)*limiterSound*(speedRatio-limiterEngage);
        }
        */
        iddleSound.volume = Mathf.Lerp(0.1f, iddleMaxVolume, speedRatio);
        runningSound.volume = Mathf.Lerp(0.3f, runningMaxVolume, speedRatio);
        runningSound.pitch = Mathf.Lerp(
            runningSound.pitch, 
            Mathf.Lerp(0.3f, runningMaxPitch, speedRatio), 
            Time.deltaTime);

    }
}
