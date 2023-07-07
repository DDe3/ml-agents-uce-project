using UnityEngine;

public class CurriculumTrackCheckpoint : MonoBehaviour
{
    private CurriculumManager curriculumManager;
    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform.parent.TryGetComponent<CarController>(out CarController carController)) 
        {
             float dotProduct = Vector3.Dot(other.transform.forward, transform.forward);
            curriculumManager.CarTroughCheckpoint(this, other.transform.parent.transform, dotProduct >= 0);
        }
    }

    public void SetCurriculumManager(CurriculumManager manager) 
    {
        this.curriculumManager = manager;
    }

    public CurriculumManager GetCurriculumManager() {
        return this.curriculumManager;
    }
}
