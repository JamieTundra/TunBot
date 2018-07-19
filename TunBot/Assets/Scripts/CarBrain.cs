using UnityEngine;

public class CarBrain : MonoBehaviour
{
    // Car parts
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform centerOfMass;
    public Rigidbody rigidBody;
    public float distanceFromLastCheckpoint = 0;

    // Car variables
    public Car carData;
    public float mass;
    public float maxSpeed;
    public float maxTorque;
    public float turnForce;
    public float maxBrakeTorque;
    bool m_init = false;

    // Debugging
    [HideInInspector]
    public GUIStyle style;
    public bool debugMode;

    int DNALength = 5;
    public DNA dna;
    public GameObject car;
    public bool seeFrontWall = false;
    public bool seeLeftWall = false;
    public bool seeRightWall = false;
    public bool seeBackWall = false;
    RaycastHit frontHit;
    RaycastHit leftHit;
    RaycastHit rightHit;
    RaycastHit backHit;
    public GameObject[] checkpoints = new GameObject[4];
    Vector3 startPosition;
    public float timeAlive = 0;
    public float distanceTravelled = 0;
    bool alive = true;

    public float sensorLength;
    public GameObject frontSensorPosition;
    public GameObject leftSensorPosition;
    public GameObject rightSensorPosition;
    public GameObject backSensorPosition;
    public LayerMask wall;
    public int checkpointCount = 0;
    bool checkpoint1Clear = false;
    bool checkpoint2Clear = false;
    bool checkpoint3Clear = false;
    bool checkpoint4Clear = false;
    float detection = 8f;

    public float steerMultiplier;

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        dna = new DNA(DNALength, 200);
        startPosition = checkpoints[3].transform.position;
        rigidBody = GetComponent<Rigidbody>();
        mass = carData.m_mass;
        maxSpeed = carData.m_maxSpeed;
        maxTorque = carData.m_maxTorque;
        turnForce = carData.m_turnForce;
        maxBrakeTorque = carData.m_maxBrakeTorque;
        rigidBody.mass = mass;
        rigidBody.centerOfMass = centerOfMass.localPosition;
        m_init = true;
        // override inspector
        sensorLength = detection;
        Drive(1);
        Steer(Random.Range(-1f, 1f));
    }

    private void Sensors()
    {
        //front sensor
        if (Physics.Raycast(frontSensorPosition.transform.position, frontSensorPosition.transform.forward, sensorLength, wall))
        {
            seeFrontWall = true;
        }
        else
        {
            seeFrontWall = false;
        }


        //front sensor
        if (Physics.Raycast(leftSensorPosition.transform.position, leftSensorPosition.transform.forward, sensorLength, wall))
        {
            seeLeftWall = true;
        }
        else
        {
            seeLeftWall = false;
        }


        //front sensor
        if (Physics.Raycast(rightSensorPosition.transform.position, rightSensorPosition.transform.forward, sensorLength, wall))
        {
            seeRightWall = true;
        }
        else
        {
            seeRightWall = false;
        }


        //front sensor
        if (Physics.Raycast(backSensorPosition.transform.position, backSensorPosition.transform.forward, sensorLength, wall))
        {
            seeBackWall = true;
        }
        else
        {
            seeBackWall = false;
        }
        Debug.DrawRay(leftSensorPosition.transform.position, leftSensorPosition.transform.forward * sensorLength, Color.green);
        Debug.DrawRay(rightSensorPosition.transform.position, rightSensorPosition.transform.forward * sensorLength, Color.red);
        Debug.DrawRay(frontSensorPosition.transform.position, frontSensorPosition.transform.forward * sensorLength, Color.yellow);
        Debug.DrawRay(backSensorPosition.transform.position, backSensorPosition.transform.forward * sensorLength, Color.blue);
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.name == "Checkpoint1" && !checkpoint1Clear)
        {
            checkpointCount++;
            checkpoint1Clear = true;
        }
        else if (collision.gameObject.name == "Checkpoint2" && !checkpoint2Clear)
        {
            checkpointCount++;
            checkpoint2Clear = true;
        }
        else if (collision.gameObject.name == "Checkpoint3" && !checkpoint3Clear)
        {
            checkpointCount++;
            checkpoint3Clear = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            alive = false;
            distanceTravelled -= 40f;
        }
    }


    private void FixedUpdate()
    {
        Sensors();
        Drive(1);
        if (!alive) return;

        // read DNA
        float steerForce = 0;

        if (seeFrontWall)
        {
            steerForce = dna.GetGene(0);
        }
        else if (seeLeftWall)
        {
            steerForce = dna.GetGene(1);
        }
        else if (seeRightWall)
        {
            steerForce = dna.GetGene(2);
        }
        else if (seeBackWall)
        {
            steerForce = dna.GetGene(3);
        }
        else
        {
            steerForce = dna.GetGene(4);
        }

        switch (checkpointCount)
        {
            case 0:
                distanceFromLastCheckpoint = Vector3.Distance(transform.position, startPosition);
                break;
            case 1:
                distanceFromLastCheckpoint = Vector3.Distance(transform.position, checkpoints[1].transform.position);
                distanceFromLastCheckpoint += 20f;
                break;
            case 2:
                distanceFromLastCheckpoint = Vector3.Distance(transform.position, checkpoints[2].transform.position);
                distanceFromLastCheckpoint += 40f;
                break;
            case 3:
                distanceFromLastCheckpoint = Vector3.Distance(transform.position, checkpoints[3].transform.position);
                distanceFromLastCheckpoint += 60f;
                break;
            default:
                distanceFromLastCheckpoint = Vector3.Distance(transform.position, startPosition);
                break;
        }

        distanceTravelled = (checkpointCount * 50) + distanceFromLastCheckpoint;

        Drive(1);
        Steer(Random.Range(-1f, 1f));
    }

    public void Steer(float steer)
    {
        float finalAngle = steer * turnForce;
        wheelColliders[0].steerAngle = finalAngle;
        wheelColliders[1].steerAngle = finalAngle;
    }

    public void Drive(float drivingForce)
    {
        if (wheelColliders[0].rpm > 250)
        {
            foreach (WheelCollider wheel in wheelColliders)
            {
                wheel.motorTorque = 0;
            }
        }
        else
        {
            wheelColliders[0].motorTorque = maxTorque * drivingForce;
            wheelColliders[1].motorTorque = maxTorque * drivingForce;
        }
    }

    public void Brake(bool handbrake)
    {
        if (handbrake)
        {
            wheelColliders[2].brakeTorque = maxBrakeTorque;
            wheelColliders[3].brakeTorque = maxBrakeTorque;
        }
        else
        {
            wheelColliders[0].brakeTorque = 0;
            wheelColliders[1].brakeTorque = 0;
            wheelColliders[2].brakeTorque = 0;
            wheelColliders[3].brakeTorque = 0;
        }

    }
}
