using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

namespace KartRace
{
    public class WallTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<KartAgents>(out var agent))
            {
                // Check if the agent is the one that finished
                agent.AddReward(-1f); // Give a reward for finishing
                agent.EndEpisode(); // End the episode
            }
        }
    }
}