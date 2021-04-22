using UnityEngine;
using ComputeShaderUtility;
using UnityEngine.Experimental.Rendering;
public class Simulation : MonoBehaviour
{
  public SimulationSettings settings;
  private GraphicsFormat format = ComputeHelper.defaultGraphicsFormat;
  [SerializeField, HideInInspector] protected RenderTexture renderTexture;
  public ComputeShader compute;

  const int simulationKernel = 0;
  ComputeBuffer agentBuffer;
  public struct Agent
  {
    public Vector2 position;
    public float angle;
    public float speed;
    public Vector4 color;
  }

  protected virtual void Start()
  {
    ComputeHelper.CreateRenderTexture(ref renderTexture, settings.width, settings.height, FilterMode.Point, format);

    Agent[] agents = new Agent[settings.numAgents];
    for (int i = 0; i < agents.Length; i++)
    {
      Vector2 centre = new Vector2(settings.width / 2, settings.height / 2);
      Vector2 startPos = centre;
      float randomAngle = Random.value * Mathf.PI * 2;
      float angle = randomAngle;
      // float angle = 0f * Mathf.PI;
      float speed = (Random.value * 10.0f) + 10.0f;
      Vector4 color;
      if (Random.Range(0, 5) == 1)
        color = new Vector4(0, 0, 0, 0);
      else
        color = new Vector4((float)Random.Range(0, 5) / 16f, 0, (float)Random.Range(0, 2), 1.0f);
      agents[i] = new Agent() { position = startPos, angle = angle, speed = speed, color = color };
    }
    ComputeHelper.CreateAndSetBuffer<Agent>(ref agentBuffer, agents, compute, "agents", simulationKernel);
    compute.SetBuffer(0, "agents", agentBuffer);

    compute.SetInt("width", settings.width);
    compute.SetInt("height", settings.height);

    transform.GetComponentInChildren<MeshRenderer>().material.mainTexture = renderTexture;
  }


  void FixedUpdate()
  {
    for (int i = 0; i < settings.stepsPerFrame; i++)
    {
      RunSimulation();
    }
  }

  // void LateUpdate()
  // {
  //   ComputeHelper.CopyRenderTexture(trailMap, renderTexture);
  // }


  void RunSimulation()
  {
    compute.SetTexture(simulationKernel, "renderTexture", renderTexture);

    // Assign settings
    compute.SetFloat("deltaTime", Time.fixedDeltaTime);
    compute.SetFloat("time", Time.fixedTime);

    // ComputeHelper.Dispatch(compute, settings.width, settings.height, 1, kernelIndex: simulationKernel);
    ComputeHelper.Dispatch(compute, settings.numAgents, 1, 1, kernelIndex: simulationKernel);
  }

  void OnDestroy()
  {
    ComputeHelper.Release(agentBuffer);
  }
}
