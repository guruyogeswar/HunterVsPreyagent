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
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));

        transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (target != null)
        {
            previousDistanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        if (target != null)
        {
            Vector3 relativePosition = target.localPosition - transform.localPosition;
            sensor.AddObservation(relativePosition);
        }
        else
        {
            sensor.AddObservation(Vector3.zero); 
        }

        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);

        if (target != null)
        {
            float currentDistanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

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
