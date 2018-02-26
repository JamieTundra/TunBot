using UnityEngine;

public class InputHandler : MonoBehaviour
{

    // Car movement
    public float m_steer;
    public float m_drivingForce;
    public bool m_handBrake;
    public CarController carController;
    public bool m_carInit = false;


    public void FixedUpdate()
    {
        if (m_carInit)
        {
            m_steer = Input.GetAxis("Horizontal");
            m_drivingForce = Input.GetAxis("Vertical");
            m_handBrake = Input.GetButton("Fire3");

            carController.Steer(m_steer);
            carController.Drive(m_handBrake, m_drivingForce);
            carController.Brake(m_handBrake);
        }

    }
}
