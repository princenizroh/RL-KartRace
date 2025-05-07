using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRace
{
    public class Checkpoint : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<CheckpointManager>()!= null)
            {
                other.GetComponent<CheckpointManager>().CheckPointReached(this);
            }
        }    
    }
}
