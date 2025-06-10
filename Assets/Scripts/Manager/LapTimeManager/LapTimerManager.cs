using UnityEngine;

namespace KartRace
{
    public class LapTimerManager : MonoBehaviour
    {
        private bool hasStarted = false;
        public bool HasStarted() => hasStarted;
        private float startTime;
        [SerializeField] private int lapCount = 0;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private float[] lapTimes = new float[10];
        private bool checkpointPassed = false;
        [SerializeField] private int wallCollisionCount = 0;
        [SerializeField] private int[] collisionCounts = new int[10];



        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("wall"))
            {
                foreach (var contact in collision.contacts)
                {
                    Vector3 normal = contact.normal;

                    if (Mathf.Abs(normal.y) < 0.5f)
                    {
                        wallCollisionCount++;
                        // Debug.Log($"Wall hit detected via normal! Total: {wallCollisionCount}");
                        break;
                    }
                }
            }
        }

        public void OnStartLap()
        {
            if (!hasStarted)
            {
                hasStarted = true;
                startTime = Time.time;
                // Debug.Log("Collider START lap at " + startTime);
            }
        }

        public void SetCheckpointPassed(bool value)
        {
            checkpointPassed = value;
        }

        public void OnFinishLap()
        {
            if (!hasStarted)
                return;

            if (!checkpointPassed)
            {
                Debug.LogWarning("Agent belum melewati checkpoint — lap diabaikan.");
                return;
            }

            float finishTime = Time.time;
            float lapTime = finishTime - startTime;

            int index = lapCount % lapTimes.Length;
            lapTimes[index] = lapTime;
            collisionCounts[index] = wallCollisionCount;

            int currentLapIndex = lapCount;

            

            lapCount++;
            startTime = Time.time;
            checkpointPassed = false;

            if (uiManager != null)
            {
                string agentName = transform.parent != null ? transform.parent.name : gameObject.name;
                uiManager.UpdateLapTime(agentName, lapTime, currentLapIndex + 1);
                uiManager.SetLapHistoryFromArray(agentName, lapTimes, lapCount);
                uiManager.RecordWallCollision(agentName, wallCollisionCount, lapCount - 1);
                wallCollisionCount = 0;
            }

            Debug.Log($"Collider FINISH lap at {finishTime} — Time taken: {lapTime} seconds");
        }
        public int GetLastCollisionCount()
        {
            if (lapCount == 0)
                return 0;
            int index = ((lapCount - 1) % collisionCounts.Length + collisionCounts.Length) % collisionCounts.Length;
            Debug.Log($"Last collision count for lap {lapCount - 1}: {collisionCounts[index]}");
            return collisionCounts[index];
        }
        public float GetLastLapTime()
        {
            if (lapCount == 0)
                return 0f;

            int index = (lapCount - 1) % lapTimes.Length;

            return lapTimes[index];
        }
    }
}
