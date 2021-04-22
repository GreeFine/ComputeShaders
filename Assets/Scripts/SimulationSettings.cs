using UnityEngine;

[CreateAssetMenu()]
public class SimulationSettings : ScriptableObject
{
  [Header("Simulation Settings")]
  [Min(1)] public int stepsPerFrame = 1;
  public int width = 1280;
  public int height = 720;
  public int numAgents = 10 * 16; //Multiplyed by threads 16 so we get even number

  [Header("Trail Settings")]
  public float trailWeight = 1;
  public float decayRate = 1;
  public float diffuseRate = 1;

  public SpeciesSettings[] speciesSettings;

  [System.Serializable]
  public struct SpeciesSettings
  {
    [Header("Movement Settings")]
    public float moveSpeed;
    public float turnSpeed;

    [Header("Sensor Settings")]
    public float sensorAngleSpacing;
    public float sensorOffsetDst;
    [Min(1)] public int sensorSize;
  }
}