using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTrackManager : MonoBehaviour
{
    
    public static TrainingTrackManager instance;

    
    [Header("Tracks")]
    [SerializeField] private List<TrackCheckpoints> tracksList;

    [Header("Karts")]
    [SerializeField] private List<Transform> carTransformList;


    private void Start() {
        if (instance) Destroy(this.gameObject);
        else instance = this;
    }

    private void Awake() {
        InitialDistributeKarts();
        AwakeTracks();
    }



    public void InitialDistributeKarts() {
        //Debug.Log("Calling InitialDistributeKarts");
        System.Random rnd = new();
        /*
        foreach (TrackCheckpoints track in tracksList) {
            track.ResetRace();
        }
        */
        foreach (Transform car in carTransformList) {
            int randomIndex = rnd.Next(0, tracksList.Count);
            tracksList[randomIndex].AddKartToThisTrack(car);
            
            //CarController controller = car.GetComponent<CarController>();

            if (car.TryGetComponent<CarController>(out CarController controller)) {
                //controller.SetSpawnPoint(tracksList[randomIndex].spawnPoint);
                controller.SetTrack(tracksList[randomIndex]);
                //Debug.Log(controller.name +" with controller " + tracksList[randomIndex].name);
            }
        }
    }

    public void RandomDistributeKart(Transform car, TrackCheckpoints track) {
        Debug.Log("Calling RandomDistributeKarts");
        int indexOfTrack = tracksList.IndexOf(track);
        track.RemoveKartFromThisTrack(car);
        System.Random rnd = new();
        int randomIndex = rnd.Next(0, tracksList.Count);
        while (randomIndex == indexOfTrack) {
            randomIndex = rnd.Next(0, tracksList.Count);
        }
        
        tracksList[randomIndex].AddKartToThisTrack(car);
        
        if (car.TryGetComponent<CarController>(out CarController controller)) {
            //controller.SetSpawnPoint(tracksList[randomIndex].spawnPoint);
            //Debug.Log(controller.transform.name + " spawnpoint: "+ tracksList[randomIndex].spawnPoint.name);
            controller.SetTrack(tracksList[randomIndex]);
        }
        tracksList[randomIndex].ResetCheckPointForSingleCar(car);
    }

    public void AwakeTracks() {
        foreach (TrackCheckpoints tracks in tracksList) {
            tracks.ResetCheckpoints();
        }
    }



}
