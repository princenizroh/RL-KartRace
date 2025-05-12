using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRace
{
    public class CheckpointManager : MonoBehaviour
    {
        private KartController kartController;
        [SerializeField] private float MaxTimeToReachCheckpoint = 30f; // Maximum time to reach the checkpoint
        [SerializeField] private float TimeLeft = 30f;

        [SerializeField] private KartAgents kartAgent;
        private Checkpoint nextCheckPointToReach;
        [SerializeField] private int currentCheckpointIndex;
        [SerializeField] private List<Checkpoint> checkpoints;
        private Checkpoint lastCheckpoint; 
        private event Action<Checkpoint> OnCheckpointReached;
        public Checkpoint NextCheckPointToReach => nextCheckPointToReach;
        public int CurrentCheckpointIndex => currentCheckpointIndex;
        public int TotalCheckpoints => checkpoints.Count;

        private void Start()
        {
            checkpoints = FindAnyObjectByType<Checkpoints>().checkPoints;
            ResetCheckpoints();
        }

        private void Update()
        {
            TimeLeft -= Time.deltaTime;

            if (TimeLeft <= 0f)
            {
                kartAgent.AddReward(-1f);
                kartAgent.EndEpisode();
            }
        }

        public void ResetCheckpoints()
        {
            currentCheckpointIndex = 0;
            TimeLeft = MaxTimeToReachCheckpoint;

            SetNextCheckPoint();
        }

        public void CheckPointReached(Checkpoint checkpoint)
        {

              if(nextCheckPointToReach != checkpoint) return;
              // --- NORMAL: maju ke checkpoint benar ---
              lastCheckpoint = checkpoints[currentCheckpointIndex];
              OnCheckpointReached?.Invoke(checkpoint);
              currentCheckpointIndex++;

              if (currentCheckpointIndex >= checkpoints.Count)
              {
                  kartAgent.AddReward(0.5f); // Finish reward
                  kartAgent.EndEpisode();
              }
              else
              {
                  // float speedFactor = kartController.Sphere.velocity.magnitude / 20f; // Ganti ini jika tidak sesuai harapan
                  // kartAgent.AddReward(0.2f + speedFactor * 0.1f);
                  kartAgent.AddReward(1.0f); // Normal reward for progressing
                  SetNextCheckPoint();
              }
        }

        private void SetNextCheckPoint()
        {
            if (checkpoints.Count > 0)
            {
                TimeLeft = MaxTimeToReachCheckpoint;
                nextCheckPointToReach = checkpoints[currentCheckpointIndex];
            }
        }
    }
}
