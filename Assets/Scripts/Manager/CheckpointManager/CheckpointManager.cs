using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRace
{
    public class CheckpointManager : MonoBehaviour
    {
        [SerializeField] private float MaxTimeToReachCheckpoint = 30f; // Maximum time to reach the checkpoint
        [SerializeField] private float TimeLeft = 30f;

        [SerializeField] private KartAgents kartAgent;
        private Checkpoint nextCheckPointToReach;
        [SerializeField] private int currentCheckpointIndex;
        [SerializeField] private List<Checkpoint> checkpoints;
        private Checkpoint lastCheckpoint; 
        private event Action<Checkpoint> OnCheckpointReached;
        public Checkpoint NextCheckPointToReach => nextCheckPointToReach;

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
            if (nextCheckPointToReach != checkpoint) return;

            lastCheckpoint = checkpoints[currentCheckpointIndex];
            OnCheckpointReached?.Invoke(checkpoint);
            currentCheckpointIndex++;

            if (currentCheckpointIndex >= checkpoints.Count)
            {
                kartAgent.AddReward(0.5f); // Give a reward for finishing
                kartAgent.EndEpisode(); // End the episode
            }
            else
            {
                kartAgent.AddReward(0.5f / checkpoints.Count); // Give a reward for reaching the checkpoint
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
