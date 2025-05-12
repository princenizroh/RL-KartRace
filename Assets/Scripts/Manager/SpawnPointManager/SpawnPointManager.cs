using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRace
{
    public class SpawnPointManager : MonoBehaviour
    {
        public Transform[] spawnPoints;

        public Transform SelectRandomSpawnpoint()
        {
            int rnd = Random.Range(0, spawnPoints.Length);
            return spawnPoints[rnd];
        }
    }
}
