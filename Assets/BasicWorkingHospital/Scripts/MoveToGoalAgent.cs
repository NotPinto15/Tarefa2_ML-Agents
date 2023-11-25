using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    private Vector3 spawnPoint;

    public override void OnEpisodeBegin()
    {
        // Reset the agent's position to the spawn point at the beginning of each episode
        spawnPoint = new Vector3(0f, 0f, 0f); // Set your desired spawn point
        transform.position = spawnPoint;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Make sure the continuous actions are not empty
        if (actions.ContinuousActions.Length >= 2)
        {
            float moveX = actions.ContinuousActions[0];
            float moveZ = actions.ContinuousActions[1];

            float moveSpeed = 1f;
            transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

            // Add a penalty and respawn if the agent hits a wall
            if (CheckWallCollision())
            {
                SetReward(-1f);
                EndEpisode();
            }
        }
        else
        {
            Debug.LogError("Unexpected number of continuous actions received.");
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // Make sure the continuous actions are not empty
        if (continuousActions.Length >= 2)
        {
            continuousActions[0] = Input.GetAxisRaw("Horizontal");
            continuousActions[1] = Input.GetAxisRaw("Vertical");
        }
        else
        {
            Debug.LogError("Unexpected number of continuous actions in Heuristic.");
        }
    }

    private bool CheckWallCollision()
    {
        // Check if the agent has collided with a wall
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f); // Adjust the radius as needed

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                // Respawn at the spawn point
                transform.position = spawnPoint;
                return true; // Collision detected
            }
        }

        return false; // No collision detected
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1f);
            EndEpisode();
        }
    }
}