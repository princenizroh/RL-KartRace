using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace KartRace 
{

    public class KartAgents : Agent
    {
        [SerializeField] private CheckpointManager checkpointManager;
        [SerializeField] private KartController kartController;

        public override void Initialize()
        {
            kartController = GetComponent<KartController>();
        }

        public override void OnEpisodeBegin()
        {
            checkpointManager.ResetCheckpoints(); 
            kartController.Respawn(); // Reset the kart's position and state
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Collect observations from the environment
            Vector3 diff = checkpointManager.NextCheckPointToReach.transform.position - transform.position;
            sensor.AddObservation(diff / 20f); // Normalize the observation
            
            AddReward(-0.001f); // Small penalty for each step taken
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            // Handle actions received from the neural network
            var input = actions.ContinuousActions;
            kartController.MoveHandler(input[1], input[0]);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            // Define heuristic actions for manual control (if needed)
            var action = actionsOut.ContinuousActions;
            action[0] = Input.GetAxisRaw("Horizontal");
            action[1] = Input.GetAxisRaw("Vertical");

        }
        
    }
}

