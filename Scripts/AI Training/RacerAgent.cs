using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RacerAgent : Agent
{
    private float timeOnRace;
    private CarController carController;

    private void Update() 
    {
        timeOnRace += Time.deltaTime;
    }

    public void Start() {
        carController = GetComponent<CarController>();
    }

    public void AwakeAgent(CarController controller) {
        //Debug.Log("Esto es nulo? " + carController == null);
        carController = controller;
        carController.GetTrack().OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        carController.GetTrack().OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
        carController.GetTrack().OnLapEnded += TrackCheckpoints_OnLapEnded;
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, CarArgs e)
    {
        if (e.carTransform == transform) {
            //Debug.Log("Car: " + gameObject.name + " correct checkpoint");
            if (e.correctDirection) {
                AddReward(1f);
            } else {
                AddReward(0.5f);
            }
            
        }
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, Transform e)
    {
        if (e == transform) {
            //Debug.Log("Car: " + gameObject.name + " incorrect checkpoint");
            AddReward(-1f);
        }   
    }

    private void TrackCheckpoints_OnLapEnded(object sender, Transform e) {
        if (e == transform && timeOnRace > 0f) {
            //Debug.Log("timeOnRace" + timeOnRace);
            AddReward((60f / timeOnRace));
            EndEpisode();
        }   
    }

    
    /*
    public override void OnEpisodeBegin()
    {
        carController.StopCompletely();
        //Debug.Log("TrackManager.instance es nulo? " + TrackManager.instance == null);
        //Debug.Log("carController es nulo? " + carController == null);
        TrainTrackManager.instance.RandomDistributeKart(transform, carController.GetTrack());
        transform.position = carController.GetSpawnPoint().position + new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f));
        transform.forward = carController.GetSpawnPoint().forward;
        timeOnRace = 0f;   
    }
    */



    public override void CollectObservations(VectorSensor sensor)
    {
        
        
        Transform checkpointTarget = carController.GetTrack().GetNextCheckpoint(transform).transform;
        float directionDot = Vector3.Dot(transform.forward, checkpointTarget.forward);
        float directionDotRight = Vector3.Dot(transform.right, checkpointTarget.forward);

        float velocity = carController.Rigidbody.velocity.magnitude;
        
        Debug.Log("Velocity: " + velocity);
        Debug.Log("Producto punto forward: " + directionDot);
        Debug.Log("Producto punto right: " + directionDotRight);

        sensor.AddObservation(directionDot);
        sensor.AddObservation(directionDotRight);
        sensor.AddObservation(velocity);
        

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
        //Debug.Log(name + " wall hit");
        AddReward(-0.5f);
    }

    public void OnWallStay() {
        //Debug.Log(name + " wall stay");
        AddReward(-0.1f);
    }
    

    // Called when i hit another agent
    public void OnAgentHitOther() {
        AddReward(0.01f);
        //Debug.Log(name + " I hit someone!");
    }


}
