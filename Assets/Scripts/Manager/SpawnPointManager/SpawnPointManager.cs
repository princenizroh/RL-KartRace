using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRace
{
    public class SpawnPointManager : MonoBehaviour
    {
        public Transform[] spawnPoints;
        [SerializeField] private bool useFirstSpawnPoint = false;
        private int nextSpawnIndex = 0;

        public Transform SelectRandomSpawnpoint()
        {
            if (useFirstSpawnPoint)
            {
                // Pastikan tidak melebihi jumlah spawn point yang tersedia
                if (nextSpawnIndex >= spawnPoints.Length)
                    nextSpawnIndex = 0; // Reset jika mencapai akhir daftar

                return spawnPoints[nextSpawnIndex++]; // Ambil spawn point berikutnya dalam urutan
            }
            else
            {
                int rnd = Random.Range(0, spawnPoints.Length);
                return spawnPoints[rnd]; // Pilih secara acak jika kondisi tidak terpenuhi
            }
        }
    }
}