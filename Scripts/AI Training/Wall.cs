using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.gameObject.TryGetComponent<RacerBot>(out RacerBot agent)) {
            //Debug.Log(other.gameObject.name + " hit " + gameObject.name);
            agent.OnWallHit();
        } else if (other.transform.parent.gameObject.TryGetComponent<CurriculumAgent>(out CurriculumAgent c_agent)) {
            c_agent.OnWallHit();
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.parent.gameObject.TryGetComponent<RacerBot>(out RacerBot agent)) {
            //Debug.Log(other.gameObject.name + " stay " + gameObject.name);
            agent.OnWallStay();
        } else if (other.transform.parent.gameObject.TryGetComponent<CurriculumAgent>(out CurriculumAgent c_agent)) {
            //c_agent.OnWallStay();
        }
    }
}
