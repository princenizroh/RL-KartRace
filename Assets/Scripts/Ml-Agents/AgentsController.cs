using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;


public class AgentsController : Agent
{
    [SerializeField] private Transform target;

    public override void OnEpisodeBegin()
    {
        // Reset the agent's position at the beginning of each episode
        transform.localPosition = new Vector3(-3.3f, 0.5f, -12.53f); // Reset position
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            target.localPosition = new Vector3(0, 0.5f, -12.53f); // Reset target position
        }
        else
        {
            target.localPosition = new Vector3(-6.6f, 0.5f, -12.53f); // Reset target position
        }    
            
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // Agent's position
        sensor.AddObservation(target.localPosition); // Target's position
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the action from the neural network
        float moveX = actions.ContinuousActions[0]; // Move left/right
        float moveSpeed = 4f;

        // Apply movement to the agent
        transform.localPosition += new Vector3(moveX, 0) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        Debug.Log($"ContinuousActions Length: {continuousActions.Length}");

        if (continuousActions.Length > 0)
        {
            continuousActions[0] = Input.GetAxis("Horizontal"); // Move left/right
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("finish"))
        {
            // Reward the agent for reaching the    target
            AddReward(10f);
            EndEpisode(); // End the episode
        }
        if (other.CompareTag("wall"))
        {
            // Reward the agent for reaching the target
            AddReward(-5f);
            EndEpisode(); // End the episode
        }
    }   
}
