using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;


public class CurriculumAgent : Agent
{
    private CarController m_Car;
    private float configValue = 0f;
    private float completionReward = 1f;
    private Quaternion rotation;
    private float m_timer = 0f;
    private CurriculumManager manager;

    public void Start() 
    {
        m_Car = GetComponent<CarController>();
        manager = GetComponentInParent<CurriculumManager>();
        SubscribeToTrackEvents(manager);
    }

    private void Update() {
       m_timer += Time.deltaTime;
    }

    public void SubscribeToTrackEvents(CurriculumManager track)
    {
        track.OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        track.OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
    }

    public void UnsubscribeFromTrackEvents(CurriculumManager track)
    {
        track.OnCarCorrectCheckpoint -= TrackCheckpoints_OnCarCorrectCheckpoint;
        track.OnCarWrongCheckpoint -= TrackCheckpoints_OnCarWrongCheckpoint;
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, KartArgs e)
    {
        if (e.carTransform == transform) 
        {
            //Debug.Log(transform.name + " Correcto!");
            if (e.correctDirection) 
            {
                
                TimeReward();
                AddReward(completionReward + 0.5f);
                //Debug.Log("Tiempo: " + m_timer);
                //Debug.Log("Reward: " + GetCumulativeReward());
                EndEpisode();
                //ResetEpisodeForThisCar();
            }
            else 
            {
                TimeReward();
                AddReward(completionReward);
                //Debug.Log("Tiempo: " + m_timer);
                //Debug.Log("Reward: " + GetCumulativeReward());
                EndEpisode();
                //ResetEpisodeForThisCar();
            }
        }
    }


    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, Transform e)
    {
        if (e == transform) {
            //Debug.Log(transform.name + " Incorrecto!");
            AddReward(-completionReward);
            //EndEpisode(); // No debe terminar el episodio para que no eviten tocar el checkpoint bueno para regresar a la pista
        }   
    }

    

    public override void OnEpisodeBegin()
    {
        manager.ResetEnvironment();
        ResetCar();
        m_timer = 0f;
    }


    public void ResetCar() 
    {
        // Curriculo

        // Obtener variables del curriculo
        configValue = manager.configValue; 
        ConfigureAgent(configValue);

        // Reset agent position, rotation
        m_Car.StopCompletely();
        

        if (configValue == 3 || configValue == 5)
        {
            transform.position = manager.GetSpawnPoint().position + new Vector3(UnityEngine.Random.Range(-4f, 4f), 1f, 0f);
            transform.rotation = this.rotation;
        } else 
        {
            transform.position = manager.GetSpawnPoint().position + new Vector3(0f, 1f, 0f);
            transform.rotation = this.rotation;
        }


    }

    void ConfigureAgent(float configure) 
    {
        switch (configure) 
        {
            case 0: // Lesson 0  Sin rotacion
                this.completionReward = 1f;
                this.rotation = manager.GetSpawnPoint().rotation;
                break;
            case 1: // Lesson 1  Rotación en un angulo de 90°
                this.completionReward = 2f;
                this.rotation = Quaternion.Euler(Vector3.up * Random.Range(-45f, 45f));
                break;
            case 2: // Lesson 2  Rotación en un angulo de 90° hacia la derecha o izquierda
                this.completionReward = 3f;
                if (Random.Range(0, 100) < 50) // probabilidad 50% 
                {
                    this.rotation = Quaternion.Euler(Vector3.up * Random.Range(-120f, -60f));
                }
                else
                {
                    this.rotation = Quaternion.Euler(Vector3.up * Random.Range(60f, 120f));
                }
                break;
            case 3: // Lesson 2  Rotación en un angulo de 90° hacia la derecha o izquierda con spawn aleatorio
                this.completionReward = 4f;
                if (Random.Range(0, 100) < 50) // probabilidad 50% 
                {
                    this.rotation = Quaternion.Euler(Vector3.up * Random.Range(-120f, -60f));
                }
                else
                {
                    this.rotation = Quaternion.Euler(Vector3.up * Random.Range(60f, 120f));
                }
                break;
            case 4: // Lesson 4 Rotación en un angulo de 90° hacia atras
                this.completionReward = 5f;
                this.rotation = Quaternion.Euler(Vector3.up * Random.Range(135f, 225f));
                break;
            case 5: // Lesson 5  Rotación aleatoria con spawn aleatorio
                this.completionReward = 6f;
                this.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
                break;
            default:
                Debug.Log("Entrenamiento terminado!: " + configure);
                break;
        }
    }

    private void TimeReward() 
    {
        float reward = (completionReward / m_timer);
        AddReward(reward);
    }



    public override void CollectObservations(VectorSensor sensor)
    {
        //Vector3 velocity = m_Car.Rigidbody.velocity.normalized; // x, y, z = 3 componentes
        //DrawArrow.ForDebug(transform.position, velocity, Color.black);
        Vector3 direction = (manager.GetNextCheckpoint(transform)
                            .transform
                            .position - transform.position).normalized;
        
        //Debug.Log(m_Car.GetSpeed());
        float speed = m_Car.GetSpeed();
        sensor.AddObservation(speed); // 1 componente
        //sensor.AddObservation(velocity);
        sensor.AddObservation(transform.InverseTransformVector(direction)); // x, y, z = 3 componentes
        //sensor.AddObservation(m_Car.m_Accelerate == 0); ¿Es necesario?

        //DrawArrow.ForDebug(transform.position, transform.InverseTransformVector(direction), Color.red);
        
        AddReward(-0.001f); // Incentivar terminar el escenario lo más rapido posible
        //AddReward(Mathf.Abs(speed)/100);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        int turnAmount = 0;
        int forwardAmount = 0;
        bool isBreaking;

        forwardAmount = actions.DiscreteActions[0];
        turnAmount = actions.DiscreteActions[1];
        isBreaking = actions.DiscreteActions[2] == 1 ? true : false;

        // 0 adelante
        // 1 tras

        int moveX = 0;
        int moveZ = 0;

        switch (forwardAmount)
        {
            case 0:
                moveX = 1;
                break;
            case 1:
                moveX = -1;
                break;
        }

        // 0 nada
        // 1 izquierda
        // 2 derecha

        switch (turnAmount)
        {
            case 0:
                moveZ = 0;
                break;
            case 1:
                moveZ = 1;
                break;
            case 2:
                moveZ = -1;
                break;
        }

        m_Car.SetInputs(moveX, moveZ, isBreaking); 
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
        AddReward(-0.1f);
    }
}
