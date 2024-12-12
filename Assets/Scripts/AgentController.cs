// Updated AgentController
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentController : Agent
{
    [SerializeField] private Transform target;
    public float pelletCount;
    public GameObject foodPrefab;

    [SerializeField] private List<GameObject> spawnedTargets = new List<GameObject>();

    [SerializeField] private Transform environmentLocation;

    [SerializeField] private float moveSpeed;
    private Rigidbody rb;

    public HunterController hunterObject;

    

    private bool hasWon = false;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        moveSpeed = 3.0f;
        // Reset agent's position
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));

        // Reset agent's rotation
        transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // Reset Rigidbody velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Remove and recreate food pellets
        RemovePellets();
        CreateTargets();
    }

    private void CreateTargets()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            int retryCounter = 0;
            Vector3 targetPosition;
            bool validPosition;

            do
            {
                targetPosition = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
                validPosition = true;

                // Check for overlap with existing targets
                foreach (var existingTarget in spawnedTargets)
                {
                    if (Vector3.Distance(targetPosition, existingTarget.transform.localPosition) < 0.5f)
                    {
                        validPosition = false;
                        break;
                    }
                }

                retryCounter++;
            } while (!validPosition && retryCounter < 10);

            // Instantiate the food pellet if valid position is found
            if (validPosition)
            {
                GameObject newTarget = Instantiate(foodPrefab, environmentLocation);
                newTarget.transform.localPosition = targetPosition;
                spawnedTargets.Add(newTarget);
            }
        }
    }

    public void RemovePellets()
    {
        foreach (var target in spawnedTargets)
        {
            Destroy(target);
        }
        spawnedTargets.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add agent's position as an observation
        sensor.AddObservation(transform.localPosition);

        // Add relative positions of all targets
        foreach (var target in spawnedTargets)
        {
            sensor.AddObservation(target.transform.localPosition - transform.localPosition);
        }

        // Add total number of remaining pellets
        sensor.AddObservation(spawnedTargets.Count);

        // Add relative position of the hunter
        if (hunterObject != null)
        {
            Vector3 hunterRelativePosition = hunterObject.transform.localPosition - transform.localPosition;
            sensor.AddObservation(hunterRelativePosition);
        }
        else
        {
            sensor.AddObservation(Vector3.zero); // Placeholder for missing hunter
        }
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        // Apply movement and rotation
        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {

        if (Vector3.Dot(transform.up, Vector3.up) < 0.9f) 
        {
            AddReward(-0.1f * Time.deltaTime);
        }

        if (other.gameObject.CompareTag("target"))
        {
            // Reward for collecting a target
            AddReward(10f);

            spawnedTargets.Remove(other.gameObject);
            Destroy(other.gameObject);

            if (spawnedTargets.Count == 0)
            {
                AddReward(20f);
                hunterObject.AddReward(-10f);
                hasWon = true;
                EndEpisode();
            }
        }
        else if (other.gameObject.CompareTag("wall"))
        {
            // Penalize for hitting a wall
            AddReward(-50f);

            RemovePellets();
            hunterObject.EndEpisode();
            EndEpisode();
        }
    }
    public bool HasWon()
    {
        return hasWon;
    }

    public void ResetWinFlag()
    {
        hasWon = false;
    }
}