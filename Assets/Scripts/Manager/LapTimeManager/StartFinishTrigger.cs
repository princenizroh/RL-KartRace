using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRace
{
    public class StartFinishTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log($"Masuk trigger oleh: {other.name}");
            LapTimerManager timer = other.GetComponent<LapTimerManager>();

            if (timer != null)
            {
                if (!timer.HasStarted())
                {
                    timer.OnStartLap();
                }
                else
                {
                    timer.OnFinishLap();
                }
            }
        }
    }
}
