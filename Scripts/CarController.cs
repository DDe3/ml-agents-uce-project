using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float m_Steering, m_Accelerate;
    private float currentSteerAngle, currentbreakForce;
    public bool isBreaking;
    private Transform spawnPoint;
    // Settings
    [SerializeField] public float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] public WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    
    [Range(0, 200)]
    public float MaxSpeed;

    public Rigidbody Rigidbody { get; private set; }

    [Header("Player Color")]
    public GameObject driverVisual;

    [Header("Vehicle Physics")]
    [Tooltip("The transform that determines the position of the kart's mass.")]
    public Transform CenterOfMass;
    private Material carColorMaterial;
    public Color carColor;
    private Transform lastKnownCheckpoint;
    private TrackCheckpoints trackCheckpoints;
    private float speedClamped;


    private void Start() 
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);

        // Give Random color to driver
        carColorMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        carColor = Random.ColorHSV();
        carColorMaterial.color = carColor;
        driverVisual.GetComponent<Renderer>().material = carColorMaterial;

        if (gameObject.tag == "Agent") 
        {
            MaxSpeed = Random.Range(150f, 200f);
        }
    }

    public Color GetCarColor() {
        return carColor;
    }

    public Transform GetSpawnPoint() {
        return spawnPoint;
    }

    public void SetSpawnPoint(Transform sp) {
        spawnPoint = sp;
    }

    public void SetTrack(TrackCheckpoints track) {
        trackCheckpoints = track;
    }

    public TrackCheckpoints GetTrack() {
        return trackCheckpoints;
    }

    public void SetInputs(float verticalInput, float horizontalInput, bool isBreaking) {
        this.m_Accelerate = verticalInput;
        this.m_Steering = horizontalInput;
        this.isBreaking = isBreaking;
    }
    private void FixedUpdate() {
        if (Rigidbody.velocity.sqrMagnitude < MaxSpeed) {
            HandleMotor(m_Accelerate * motorForce);
        } else {
            HandleMotor(0);
        }
        HandleSteering();
    }

    private void HandleMotor(float torque) {
        frontLeftWheelCollider.motorTorque = torque;
        frontRightWheelCollider.motorTorque = torque;
        rearLeftWheelCollider.motorTorque = torque;
        rearRightWheelCollider.motorTorque = torque;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking() {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering() {
        currentSteerAngle = maxSteerAngle * m_Steering;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
        UpdateWheels();
    }

    private void UpdateWheels() {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform) {
        Vector3 pos;
        Quaternion rot; 
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    public void StopCompletely() {
        Rigidbody.Sleep();
        m_Steering = 0f;
        m_Accelerate = 0f;
        Rigidbody.WakeUp();
    }

    public void GoBackToLastCheckpoint() {
        StopCompletely();
        transform.position = lastKnownCheckpoint.position + lastKnownCheckpoint.forward * 2f;
        //transform.position = lastKnownCheckpoint.position + new Vector3(2f, 1f, 0f);
        transform.forward = lastKnownCheckpoint.forward;
        if (TryGetComponent<RacerBot>(out RacerBot agent)) {
            agent.AddReward(-1f);
        }
    }

    public void SetLastKnownCheckPoint(Transform lastKnownPosition) {
        lastKnownCheckpoint = lastKnownPosition;
    }


    public float GetSpeed() 
    {
        float dot = Vector3.Dot(transform.forward, Rigidbody.velocity);
        if (Mathf.Abs(dot) > 0.1f)
        {
            float speed = Rigidbody.velocity.sqrMagnitude;
            return dot < 0 ? - (speed / MaxSpeed) : (speed / MaxSpeed);
        }
        return 0f;
    }

    public float GetWheelSpeed() 
    {
        float speed = frontLeftWheelCollider.rpm * frontLeftWheelCollider.radius * 2f * Mathf.PI / 10f;
        speedClamped = Mathf.Lerp(speedClamped, speed, Time.deltaTime);
        // Debug.Log(speed);
        return speedClamped;
    }

    public float GetSpeedRatio() 
    {
        var gas = Mathf.Clamp(Mathf.Abs(m_Accelerate), 0.5f, 1f);
        return GetWheelSpeed() / MaxSpeed;
    }
}



    
