using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRace
{
    public class TimeScaler : MonoBehaviour
    {
        void Start()
        {
            Time.timeScale = 20f; // Ubah angka ini sesuai keinginan, misal 30f
            Application.targetFrameRate = -1; // Biarkan Unity jalan secepat mungkin
            QualitySettings.vSyncCount = 0;   // Jangan sync dengan monitor
        }
    }
}
