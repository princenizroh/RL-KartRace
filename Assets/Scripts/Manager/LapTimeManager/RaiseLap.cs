using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRace
{
    public class RaiseLap : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var lapTimer = other.GetComponent<LapTimerManager>();
            if (lapTimer != null)
            {
                lapTimer.SetCheckpointPassed(true);
                Debug.Log("Checkpoint dilewati oleh: " + other.name);
            }
        }
    }
}
