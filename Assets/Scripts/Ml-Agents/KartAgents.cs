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
            Vector3 diff = checkpointManager.NextCheckPointToReach.transform.position - transform.position;

            // Observasi posisi relatif ke checkpoint
            sensor.AddObservation(diff.normalized);
            sensor.AddObservation(diff.magnitude / 100f);

            // sensor.AddObservation(Physics.Raycast(transform.position, transform.forward, 1.0f));

            // Observasi kecepatan dan arah gerak
            sensor.AddObservation(kartController.Sphere.velocity.magnitude / 20f);
            sensor.AddObservation(kartController.Sphere.transform.forward);

            // Observasi indeks checkpoint saat ini (perbandingan relatif)
            sensor.AddObservation(checkpointManager.CurrentCheckpointIndex / (float)checkpointManager.TotalCheckpoints);
            //
            // // Observasi sudut relatif ke checkpoint (pakai sudut bertanda)
            Vector3 forward = kartController.Sphere.transform.forward;
            Vector3 directionToCheckpoint = diff.normalized;
            Vector3 cross = Vector3.Cross(forward, directionToCheckpoint);
            float signedAngle = Mathf.Sign(cross.y) * Vector3.Angle(forward, directionToCheckpoint) / 180f;
            sensor.AddObservation(signedAngle); // [-1,1]
            //
            float rayLength = 10f;
            Vector3[] rayDirections = new Vector3[]
            {
                transform.forward,
                Quaternion.AngleAxis(30, Vector3.up) * transform.forward,
                Quaternion.AngleAxis(-30, Vector3.up) * transform.forward,
                Quaternion.AngleAxis(60, Vector3.up) * transform.forward,
                Quaternion.AngleAxis(-60, Vector3.up) * transform.forward,
            };

            foreach (var dir in rayDirections)
            {
                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, rayLength))
                {
                    sensor.AddObservation(hit.distance / rayLength); // Jarak ke dinding (0 - 1)
                }
                else
                {
                    sensor.AddObservation(1f); // Tidak ada objek di depan
                }
            }
            //
            // // Reward shaping kecil untuk hadap ke checkpoint
            float alignment = Vector3.Dot(forward, directionToCheckpoint); // [-1, 1]
            AddReward(alignment * 0.01f);
            //
            // // Reward makin dekat ke checkpoint
            float distanceReward = Mathf.Clamp01(1f - (diff.magnitude / 100f));
            AddReward(distanceReward * 0.01f);
            //

            // Penalti waktu setiap langkah
            AddReward(-0.001f);
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

