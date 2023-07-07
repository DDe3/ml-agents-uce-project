using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TrackCheckpoints : MonoBehaviour
{

    [SerializeField] public Transform spawnPoint;
    [SerializeField] public int laps = 3;
    
    [Header("Training")]
    [SerializeField] public bool isTraining;

    public event EventHandler OnPlayerCorrectCheckpoint;
    public event EventHandler OnPlayerWrongCheckpoint;

    public event EventHandler<CarArgs> OnCarCorrectCheckpoint;
    public event EventHandler<Transform> OnCarWrongCheckpoint;
    public event EventHandler<int> OnLapUpdate;
    public event EventHandler<Transform> OnLapEnded;
    
    private List<Transform> carTransformList = new();
    private List<CheckpointSingle> checkpointSingleList;
    private List<int> nextCheckpointSingleIndexList = new();
    private List<int> carCurrentLap = new();
    private List<Transform> carPositionList = new();
    public List<CarPosition> carPositions = new();
    private List<Transform> winners = new();



    public void Awake() 
    {
        ResetCheckpoints();
    }

    public List<Transform> GetAllCars() 
    {
        return this.carTransformList;
    }

    public void SetCars(List<Transform> carList) 
    {
        this.carTransformList = carList;
    }
    public void AddKartToThisTrack(Transform carTransform) 
    {
        carTransformList.Add(carTransform);
        nextCheckpointSingleIndexList.Add(0);
        carCurrentLap.Add(1);
    }

    public void RemoveKartFromThisTrack(Transform carTransform) 
    {
        if (carTransformList.Contains(carTransform)) {
            nextCheckpointSingleIndexList.RemoveAt(carTransformList.IndexOf(carTransform));
            carCurrentLap.RemoveAt(carTransformList.IndexOf(carTransform));
            carTransformList.Remove(carTransform);
        }
    }
    
    public void ResetCheckpoints() {
        //Debug.Log("ResetCheckpoints");
        Transform checkpointTransform = transform.Find("Checkpoints");
        checkpointSingleList = new List<CheckpointSingle>();
        foreach (Transform checkpointSingleTransform in checkpointTransform) 
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }
        nextCheckpointSingleIndexList = new List<int>();
        carCurrentLap = new List<int>();

        foreach (Transform carTransform in carTransformList) 
        {
            CarController controller = carTransform.GetComponent<CarController>();
            controller.SetSpawnPoint(spawnPoint);
            controller.SetLastKnownCheckPoint(spawnPoint);
            nextCheckpointSingleIndexList.Add(0);
            carCurrentLap.Add(1);
        }
        winners = new();
        foreach (Transform carTransform in carTransformList) 
        {
            CarPosition carp = new() 
            {
                car = carTransform,
                carIndexInList = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)] + (GetCurrentLap(carTransform) * checkpointSingleList.Count()) 
            };
            carPositions.Add(carp);
        }
        
    }

    private void Update() 
    {
        CheckPosition();
    }

    private void CheckPosition() 
    {
        carPositions.Clear();
        foreach (Transform carTransform in carTransformList) {
            CarPosition carp = new() 
            {
                car = carTransform,
                carIndexInList = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)] + (GetCurrentLap(carTransform) * checkpointSingleList.Count()) 
            };
            carPositions.Add(carp);
        }
        // Ordenar la lista en base a carIndexInList y distancia al siguiente checkpoint
        carPositions = carPositions.OrderByDescending(x => x.carIndexInList)
                                    .ThenBy( s =>  Vector3.Distance(GetNextCheckpoint(s.car).transform.position, s.car.position) )
                                    .ToList();
        
        //Debug.Log("Pos1" + carPositions[0].car.name);
        //Debug.Log("Pos2" + carPositions[1].car.name);
    }

    


    public void ResetCheckPointForSingleCar(Transform car) {
        CarController controller = car.GetComponent<CarController>();
        controller.SetSpawnPoint(spawnPoint);
        controller.SetLastKnownCheckPoint(spawnPoint);
    }


    public CheckpointSingle GetNextCheckpoint(Transform carTransform) {
        //Debug.Log(carTransformList.Count);
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        //Debug.Log("Next checkpoint: " + nextCheckpointSingleIndex);
        return checkpointSingleList[(nextCheckpointSingleIndex) % checkpointSingleList.Count];
    }

    public void SetLastKnownCheckPoint(Transform carTransform, Transform lastKnownPosition) {
        CarController controller = carTransform.GetComponent<CarController>();
        controller.SetLastKnownCheckPoint(lastKnownPosition);
    }
    


    public void CarTroughCheckpoint(CheckpointSingle checkpoint, Transform carTransform, bool correctDirection) {
        //Debug.Log(nextCheckpointSingleIndexList.Count);
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        //Debug.Log(carTransform.name + " next checkpoint : " + nextCheckpointSingleIndexList[nextCheckpointSingleIndex]);
        if (checkpointSingleList.IndexOf(checkpoint) == nextCheckpointSingleIndex) {
            //Debug.Log(carTransform.name + " Correct");
            int checkpointhit = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)]
             = checkpointhit;
            
            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];
            SetLastKnownCheckPoint(carTransform, correctCheckpointSingle.transform);

            if (carTransform.gameObject.CompareTag("Player")) {
                correctCheckpointSingle.Hide();
                OnPlayerCorrectCheckpoint?.Invoke(this, EventArgs.Empty);
            }
            CarArgs thisCar = new CarArgs {carTransform = carTransform, correctDirection = correctDirection};
            OnCarCorrectCheckpoint?.Invoke(this, thisCar);
            
            
            if (checkpointhit == 0 && !isTraining) {
                UpdateLap(carTransform);
            } else if (checkpointhit == 0 && isTraining) {
                OnLapEnded?.Invoke(this, carTransform);
            }
            
        } else {

            //Debug.Log(carTransform.name + " Incorrect");
            OnCarWrongCheckpoint?.Invoke(this, carTransform);
            
            if (carTransform.gameObject.CompareTag("Player") && !correctDirection) {
                CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];
                OnPlayerWrongCheckpoint?.Invoke(this, EventArgs.Empty);
                correctCheckpointSingle.Show();
            }
        }
    }

    private void UpdateLap(Transform carTransform) {
         int lap = carCurrentLap[carTransformList.IndexOf(carTransform)];
         carCurrentLap[carTransformList.IndexOf(carTransform)] = lap + 1;
         if (carTransform.gameObject.CompareTag("Player")) {
            OnLapUpdate?.Invoke(this, lap);
         }
         if (lap > laps) {
            EndRace(carTransform);
            winners.Add(carTransform);
            if (winners.Count == carTransformList.Count - 1) 
            {
                // Ir a la pantalla de ganadores
            }
         }
    }

    private void EndRace(Transform carTransform) {
        if (carTransform.TryGetComponent<PlayerInput>(out PlayerInput m_PI)) 
        {
            m_PI.enabled = false; 
        } 
    }

    public int GetCurrentLap(Transform car)
    {
        int lap = carCurrentLap[carTransformList.IndexOf(car)];
        return lap;
    }

    
}

public class CarArgs : EventArgs {
    public Transform carTransform { get; set;}
    public bool correctDirection { get; set;}
}

public class CarPosition {
    public Transform car { get; set;}
    public int carIndexInList { get; set;}
}

