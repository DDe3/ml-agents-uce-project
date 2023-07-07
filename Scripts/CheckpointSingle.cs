using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{

    private TrackCheckpoints trackCheckpoints;
    [SerializeField] private MeshRenderer ringMeshRenderer;


    private void Start() {
        Hide();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.TryGetComponent<CarController>(out CarController controller)) {
            
            float dotProduct = Vector3.Dot(other.transform.forward, transform.forward);
            if (dotProduct >= 0) {
                trackCheckpoints.CarTroughCheckpoint(this, other.transform.parent.transform, true);
            } else {
                trackCheckpoints.CarTroughCheckpoint(this, other.transform.parent.transform, false);
            }
            
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints) {
        this.trackCheckpoints = trackCheckpoints;
    }


    public void Show() {
        ringMeshRenderer.enabled = true;
    }

    public void Hide() {
        ringMeshRenderer.enabled = false;
    }

}
