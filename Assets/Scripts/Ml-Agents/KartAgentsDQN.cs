using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace KartRace 
{

    public class KartAgentsDQN : Agent
    {
        [SerializeField] private CheckpointManagerDQN checkpointManager;
        [SerializeField] private KartController kartController;

        private float lastCheckpointDistance;
        private int stuckCounter = 0;
        private float stuckTolerance = 0.5f;
        private int maxStuckSteps = 150;

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
            Vector3 toCheckpoint = (checkpointManager.NextCheckPointToReach.transform.position - transform.position).normalized;
            Vector3 localDirection = transform.InverseTransformDirection(toCheckpoint);
            sensor.AddObservation(localDirection.x); // horizontal
            sensor.AddObservation(localDirection.z); // ke depan

            // Sudut ke checkpoint
            float angle = Vector3.SignedAngle(transform.forward, toCheckpoint, Vector3.up) / 180f;
            sensor.AddObservation(angle);

            // Kecepatan lokal
            Vector3 localVelocity = transform.InverseTransformDirection(kartController.Sphere.velocity);
            sensor.AddObservation(localVelocity.z); // Maju/mundur

            float alignment = Vector3.Dot(transform.forward, toCheckpoint);
            if (alignment > 0.8f && localVelocity.z > 1f)
            {
                AddReward(0.01f);
            }
            else if (localVelocity.z < -0.1f)
            {
                AddReward(-0.05f); // Penalti mundur
            }
            AddReward(-0.001f); // Penalti waktu
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            // Handle actions received from the neural network
            var discreteActions = actions.DiscreteActions;
            int steeringAction = discreteActions[0]; // 
            int throttleAction = discreteActions[1];

            float steering = 0f;
            float throttle = 0f;

            if (steeringAction == 0) steering = -1f;
            else if (steeringAction == 2) steering = 1f;

            if (throttleAction == 0) throttle = -1f;
            else if (throttleAction == 2) throttle = 1f;
            kartController.MoveHandler(throttle, steering);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            // Define heuristic actions for manual control (if needed)
            var discreteActionsOut = actionsOut.DiscreteActions;

            discreteActionsOut[0] = Input.GetKey(KeyCode.LeftArrow) ? 0 :
                                    Input.GetKey(KeyCode.RightArrow) ? 2 : 1;

            discreteActionsOut[1] = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;

        }
        private void FixedUpdate()
        {
            if (!IsEpisodeRunning()) return;

            float currentDistance = Vector3.Distance(transform.position, checkpointManager.NextCheckPointToReach.transform.position);

            if (currentDistance > lastCheckpointDistance - stuckTolerance)
            {
                stuckCounter++;
            }
            else
            {
                stuckCounter = 0;
            }

            lastCheckpointDistance = currentDistance;

            if (stuckCounter >= maxStuckSteps)
            {
                AddReward(-1f);  // Penalti keras karena dianggap stuck
                EndEpisode();
            }
        }

        private bool IsEpisodeRunning()
        {
            return !HasReachedFinalCheckpoint() && checkpointManager.NextCheckPointToReach != null;
        }

        private bool HasReachedFinalCheckpoint()
        {
            return checkpointManager.CurrentCheckpointIndex >= checkpointManager.TotalCheckpoints;
        }
    }
}

