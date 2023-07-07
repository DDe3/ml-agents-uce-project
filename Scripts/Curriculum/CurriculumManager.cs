using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;

public class CurriculumManager : MonoBehaviour
{
    [SerializeField] List<CurriculumTrackCheckpoint> checkpoints;
    [SerializeField] private List<Transform> carTransformList;
    [SerializeField] private Transform spawnpoint;
    private List<int> nextCheckpointSingleIndexList;
    public event EventHandler<KartArgs> OnCarCorrectCheckpoint;
    public event EventHandler<Transform> OnCarWrongCheckpoint;
    private Vector3 originalBlockPosition;
    private Quaternion originalBlockRotation;
    [Range(0, 5)]
    public float configValue;
    

    private void Awake() 
    {
        configValue = Academy.Instance.EnvironmentParameters.GetWithDefault("config_value", configValue);
        originalBlockPosition = spawnpoint.position;
        originalBlockRotation = spawnpoint.rotation;
        InitializeEnvironment();
    }

    public Transform GetSpawnPoint() {
        return this.spawnpoint;
    }

    private void InitializeEnvironment() 
    {
        nextCheckpointSingleIndexList = new();

        foreach (CurriculumTrackCheckpoint checkpoint in checkpoints) 
        {
            checkpoint.SetCurriculumManager(this);
        }
        
        foreach (Transform car in carTransformList) 
        {
            nextCheckpointSingleIndexList.Add(0);
        }
    }

    public void ResetCar(Transform car) 
    {
        nextCheckpointSingleIndexList[carTransformList.IndexOf(car)] = 0;
    }



    public void ResetEnvironment() 
    {
        nextCheckpointSingleIndexList = new();

        foreach (Transform car in carTransformList) 
        {
            nextCheckpointSingleIndexList.Add(0);
        }
        configValue = Academy.Instance.EnvironmentParameters.GetWithDefault("config_value", configValue);
        Debug.Log("Curriculum value: " + configValue.ToString());
    }

    public void CarTroughCheckpoint(CurriculumTrackCheckpoint checkpoint, Transform carTransform, bool correctDirection) 
    {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        
        if (checkpoints.IndexOf(checkpoint) == nextCheckpointSingleIndex) {
            int checkpointhit = (nextCheckpointSingleIndex + 1) % checkpoints.Count;
            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)]
             = checkpointhit;

            KartArgs m_Car = new KartArgs {carTransform = carTransform, correctDirection = correctDirection};
            OnCarCorrectCheckpoint?.Invoke(this, m_Car);
        } else {
            OnCarWrongCheckpoint?.Invoke(this, carTransform);
        }
    }


    public CurriculumTrackCheckpoint GetNextCheckpoint(Transform carTransform) 
    {
       
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];

        return checkpoints[(nextCheckpointSingleIndex) % checkpoints.Count];
    }

    


}

public class KartArgs : EventArgs {
    public Transform carTransform { get; set;}
    public bool correctDirection { get; set;}
}
