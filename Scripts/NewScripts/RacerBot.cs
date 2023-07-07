using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RacerBot : Agent
{
    private CarController carController;

    public void Start() 
    {
        carController = GetComponent<CarController>();
    }

    public void SubscribeToTrackEvents(TrackCheckpoints track)
    {
        track.OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        track.OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
        track.OnLapEnded += TrackCheckpoints_OnLapEnded;
    }

    public void UnsubscribeFromTrackEvents(TrackCheckpoints track)
    {
        track.OnCarCorrectCheckpoint -= TrackCheckpoints_OnCarCorrectCheckpoint;
        track.OnCarWrongCheckpoint -= TrackCheckpoints_OnCarWrongCheckpoint;
        track.OnLapEnded -= TrackCheckpoints_OnLapEnded;
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, CarArgs e)
    {
        if (e.carTransform == transform) {
            if (e.correctDirection) {
                //Debug.Log("Cruzaste bien");
                AddReward(1f);
            } else {
                //Debug.Log("Cruzaste al reves");
                AddReward(0.7f);
            }
        }
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, Transform e)
    {
        if (e == transform) {
            AddReward(-1f);
        }   
    }

    private void TrackCheckpoints_OnLapEnded(object sender, Transform e) {
        if (e == transform) {
            AddReward(10f);
            EndEpisode();
        }   
    }

    

    public override void OnEpisodeBegin()
    {
        carController.StopCompletely();
        if (carController.GetTrack().isTraining) {
            TrainTrackManager.instance.RandomDistributeKart(transform, carController.GetTrack());
        } else {
            PlayerTrackManager.instance.RandomDistributeKart(transform, carController.GetTrack());
        }
        ResetCar();
    }


    public void ResetCar() 
    {
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0f; 
        Quaternion rotation = Quaternion.LookRotation(carController.GetSpawnPoint().forward);
        randomDirection = rotation * randomDirection;
        transform.position = carController.GetSpawnPoint().position + new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        //transform.forward = carController.GetSpawnPoint().forward;
        transform.forward  = randomDirection;
    }



    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        float speed = carController.GetSpeed();
        //Debug.Log("Speed:" + speed);

        Transform checkpointTarget = carController.GetTrack().GetNextCheckpoint(transform).transform;
        float directionDot = Vector3.Dot(transform.forward, checkpointTarget.forward);
        float directionDotRight = Vector3.Dot(transform.right, checkpointTarget.forward);
        Vector3 direction = (carController.GetTrack().GetNextCheckpoint(transform)
                            .transform
                            .position - transform.position).normalized;

        
        AddReward(speed * 0.01f);

        if (Mathf.Abs(speed) < 0.1) 
        {
            AddReward(-0.1f);
        }
        //Incita a manejar mas rápido


        sensor.AddObservation(directionDot); // 1
        sensor.AddObservation(directionDotRight); // 1
        sensor.AddObservation(speed); // 1
        sensor.AddObservation(transform.InverseTransformDirection(direction)); // 3

        //Debug.Log("Reward: " + GetCumulativeReward());

    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        float fordwardAmount = 0f;
        float turnAmount = 0f;
        bool isBreaking;

        fordwardAmount = actions.ContinuousActions[0];
        turnAmount = actions.ContinuousActions[1];
        isBreaking = actions.DiscreteActions[0] == 1 ? true : false;
        carController.SetInputs(fordwardAmount, turnAmount, isBreaking); 
    }



    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float fordwardAmount = Input.GetAxis("Vertical");
        float turnAmount = Input.GetAxis("Horizontal");
        bool isBreaking = Input.GetKey(KeyCode.Space);

        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = fordwardAmount;
        continuousActions[1] = turnAmount;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = isBreaking == true ? 1 : 0;
    }
    


    public void OnWallHit() {
        AddReward(-1f);
    }

    public void OnWallStay() {
        AddReward(-0.001f);
    }
}
