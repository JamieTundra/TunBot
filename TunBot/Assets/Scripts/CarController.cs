using UnityEngine;

public class CarController : MonoBehaviour
{
    // Car parts
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform centerOfMass;
    public Rigidbody rigidBody;

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

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        rigidBody = GetComponent<Rigidbody>();
        mass = carData.m_mass;
        maxSpeed = carData.m_maxSpeed;
        maxTorque = carData.m_maxTorque;
        turnForce = carData.m_turnForce;
        maxBrakeTorque = carData.m_maxBrakeTorque;
        rigidBody.mass = mass;
        rigidBody.centerOfMass = centerOfMass.localPosition;
        m_init = true;
    }

    private void Update()
    {
        if (m_init)
        {
            float currentSpeed = 2.23694f * rigidBody.velocity.magnitude; //* (-1 * direction.x);

            if (Input.GetAxis("Horizontal") != 0)
            {
                Steer(Input.GetAxis("Horizontal"));
            }

            if (currentSpeed < maxSpeed)
            {
                if (Input.GetAxis("Vertical") != 0)
                {
                    Drive(Input.GetAxis("Vertical"));
                }
                else
                {
                    Drive(0);
                }
            }

            foreach (WheelCollider wheel in wheelColliders)
            {
                if (wheel.rpm > 250)
                {
                    wheel.motorTorque = 0;
                }
            }

            Brake(Input.GetButtonDown("Fire3"));
        }

    }


    public void Steer(float steer)
    {
        float finalAngle = steer * turnForce;
        wheelColliders[0].steerAngle = finalAngle;
        wheelColliders[1].steerAngle = finalAngle;
    }

    public void Drive(float drivingForce)
    {
        wheelColliders[0].motorTorque = maxTorque * drivingForce;
        wheelColliders[1].motorTorque = maxTorque * drivingForce;
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
