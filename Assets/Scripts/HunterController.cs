// Updated HunterController
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class HunterController : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] public float moveSpeed;
    [SerializeField] private Transform envLocation;

    private Rigidbody rb;
    public AgentController classObject;

    private float previousDistanceToTarget;

    private bool hasWon = false;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset the hunter's position
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));

        // Reset agent's rotation
        transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // Reset Rigidbody velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Ensure a valid target position is available
        if (target != null)
        {
            previousDistanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Hunter's position
        sensor.AddObservation(transform.localPosition);

        // Target relative position
        if (target != null)
        {
            Vector3 relativePosition = target.localPosition - transform.localPosition;
            sensor.AddObservation(relativePosition);
        }
        else
        {
            sensor.AddObservation(Vector3.zero); // Placeholder for missing target
        }

        // Hunter's velocity
        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        // Apply movement and rotation
        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);

        // Calculate distance to target
        if (target != null)
        {
            float currentDistanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

            // Reward for approaching the target
            if (currentDistanceToTarget < previousDistanceToTarget)
            {
                AddReward(1f);
            }
            else
            {
                AddReward(-1f);
            }

            previousDistanceToTarget = currentDistanceToTarget;
        }

        // Penalize idling behavior
        AddReward(-0.001f * Time.deltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("prey"))
        {
            AddReward(50f);
            classObject.AddReward(-50f);
            other.gameObject.GetComponent<AgentController>()?.EndEpisode();
            hasWon = true;
            EndEpisode();
        }
        else if (other.gameObject.CompareTag("wall"))
        {
            AddReward(-15f);
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
