using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falloff : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.gameObject.TryGetComponent<CarController>(out CarController car)) {
            car.GoBackToLastCheckpoint();
        }
    }
}
