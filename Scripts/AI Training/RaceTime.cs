using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceTime : MonoBehaviour
{
    private float raceTime;


    private void Update() 
    {
        raceTime += Time.deltaTime;
    }

    private void Awake() 
    {
        raceTime = 0f;
    }


    public void ResetTime() 
    {
        raceTime = 0f;
    }

    public float GetTime() {
        return raceTime;
    }
}
