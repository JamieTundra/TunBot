using UnityEngine;

[CreateAssetMenu]
public class Car : ScriptableObject
{

    public string m_carName;
    public float m_mass;
    public float m_maxSpeed;
    public float m_turnForce;
    public float m_maxTorque;
    public float m_maxBrakeTorque;
    public float m_multiplier;
    public bool m_isKinematic;

}
