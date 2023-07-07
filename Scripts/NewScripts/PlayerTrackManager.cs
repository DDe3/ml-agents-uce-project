using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackManager : TrackManager
{

    public static PlayerTrackManager instance;

    void Start()
    {
        if (instance) Destroy(this.gameObject);
        else instance = this;
        InitialDistributeKarts();
    }

    public void InitialDistributeKarts() 
    {
        foreach (Transform car in carTransformList)
        {
            SetNewTrackForCar(tracksList[0], car);
        }
        tracksList[0].ResetCheckpoints();
    }

    public override void RandomDistributeKart(Transform car, TrackCheckpoints track)
    {
        //tracksList[0].AddKartToThisTrack(car);
    }


    private void SetNewTrackForCar(TrackCheckpoints newTrack, Transform car) {
        newTrack.AddKartToThisTrack(car);
        if (car.TryGetComponent<CarController>(out CarController controller)) {
            AssignTrackToKart(controller, newTrack);
        }
    }


}
