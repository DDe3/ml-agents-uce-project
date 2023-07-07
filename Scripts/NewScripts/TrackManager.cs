using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackManager : MonoBehaviour
{

    [Header("Tracks")]
    [SerializeField] public List<TrackCheckpoints> tracksList;

    [Header("Cars in Track")]
    [SerializeField] public List<Transform> carTransformList;

    public abstract void RandomDistributeKart(Transform car, TrackCheckpoints track);
    public void AssignTrackToKart(CarController controller, TrackCheckpoints track) {
        controller.SetTrack(track);
    }


}
