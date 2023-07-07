using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTrackManager : TrackManager
{
    public static TrainTrackManager instance;

    private void Start() {
        if (instance) Destroy(this.gameObject);
        else instance = this;
        InitialDistributeCars();
    }


    public void InitialDistributeCars() {
        //Debug.Log("InitialDistributeCars");
        System.Random rnd = new();
        foreach (Transform car in carTransformList) {
            int randomIndex = rnd.Next(0, tracksList.Count);
            TrackCheckpoints track = tracksList[randomIndex];
            SetInitialTrackForCar(track, car);
        }
    }

    public override void RandomDistributeKart(Transform car, TrackCheckpoints track) {
        //Debug.Log("RandomDistributeCars");
        int indexOfTrack = tracksList.IndexOf(track);
        track.RemoveKartFromThisTrack(car);
        System.Random rnd = new();
        int randomIndex = rnd.Next(0, tracksList.Count);
        //while (randomIndex == indexOfTrack) {
        //    randomIndex = rnd.Next(0, tracksList.Count);
        //}

        TrackCheckpoints newTrack = tracksList[randomIndex];

        SetNewTrackForCar(newTrack, car);
        newTrack.ResetCheckPointForSingleCar(car);
    }

    private void SetInitialTrackForCar(TrackCheckpoints newTrack, Transform car)
    {
        newTrack.AddKartToThisTrack(car);
        if (car.TryGetComponent<CarController>(out CarController controller)) 
        {
            if (car.TryGetComponent<RacerBot>(out RacerBot bot)) 
            {
                AssignTrackToKart(controller, newTrack);
                bot.SubscribeToTrackEvents(newTrack);
            }         
        }
    }

    private void SetNewTrackForCar(TrackCheckpoints newTrack, Transform car) {
        
        newTrack.AddKartToThisTrack(car);
        if (car.TryGetComponent<CarController>(out CarController controller)) 
        {
            if (car.TryGetComponent<RacerBot>(out RacerBot bot)) 
            {
                bot.UnsubscribeFromTrackEvents(controller.GetTrack());
                AssignTrackToKart(controller, newTrack);
                bot.SubscribeToTrackEvents(newTrack);
            }         
        }
    }

    private void AwakeTracks() {
        foreach (var track in tracksList)
        {
            track.ResetCheckpoints();
        }
    }
}
