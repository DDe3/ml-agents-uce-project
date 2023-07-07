using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSoundController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    private CarController carController;
    public float minimumPitch = 1f;
    public float maximumPitch = 2f;
    private void Start() 
    {
        carController = GetComponent<CarController>();
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = minimumPitch;
    }

    private void Update() 
    {
        float engineSpeed = Mathf.Abs(carController.GetSpeed());
        if (engineSpeed < minimumPitch) 
        {
            audioSource.pitch = minimumPitch;
        } 
        else
        {
            audioSource.pitch = maximumPitch;
        }
    }
}
