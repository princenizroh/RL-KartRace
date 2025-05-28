using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

namespace KartRace
{
    [System.Serializable]
    public class AgentData
    {
        public string name;
        public float totalTime;
        public float bestLap;
        public int bestLapIndex;
        public int totalLap;
        public List<float> lapHistory = new();
        public Button buttonComponent;
        public TextMeshProUGUI textComponent;
        public List<int> wallCollisionsPerLap = new();
        public int totalWallCollisions => wallCollisionsPerLap
            .Take(lapHistory.Count)
            .Sum();
        public int BestCollisionValue => wallCollisionsPerLap
            .Take(lapHistory.Count)
            .DefaultIfEmpty(int.MaxValue)
            .Min();

        public int BestCollisionLapIndex => wallCollisionsPerLap
            .Take(lapHistory.Count)
            .Select((value, index) => (value, index))
            .OrderBy(pair => pair.value)
            .Select(pair => pair.index + 1) // +1 karena lap dimulai dari 1
            .FirstOrDefault();
    }

    public class UIManager : MonoBehaviour
    {
        [Header("Prefabs & Layout")]
        public GameObject bestLapEntryPrefab;
        public GameObject bestCollisionEntryPrefab;
        public Transform bestLapContentParent;
        public Transform bestCollisionContentParent;
        public ScrollRect bestLapScrollRect;
        public ScrollRect bestCollisionScrollRect;

        [Header("Detail Panel")]
        public GameObject detailPanel;
        public GameObject closePanel;
        public TextMeshProUGUI agentNameText;
        public TextMeshProUGUI bestLapText;
        public TextMeshProUGUI totalTimeText;
        public TextMeshProUGUI wallCollisionsText;
        public TextMeshProUGUI lapListText;

        private Dictionary<string, AgentData> agents = new();

        public void UpdateLapTime(string agentName, float lapTime, int lapNumber)
        {
            if (!agents.ContainsKey(agentName))
            {
                var newBtn = Instantiate(bestLapEntryPrefab, bestLapContentParent);
                var text = newBtn.GetComponentInChildren<TextMeshProUGUI>();
                var button = newBtn.GetComponent<Button>();

                AgentData newData = new()
                {
                    name = agentName,
                    totalTime = lapTime,
                    bestLap = lapTime,
                    bestLapIndex = lapNumber,
                    totalLap = lapNumber,
                    lapHistory = new List<float> { lapTime },
                    buttonComponent = button,
                    textComponent = text
                };

                button.onClick.AddListener(() => ShowDetail(newData));
                agents[agentName] = newData;
            }
            else
            {
                var data = agents[agentName];

                if (data.lapHistory.Count >= 10)
                    data.lapHistory.RemoveAt(0);

                data.lapHistory.Add(lapTime);
                data.totalLap = lapNumber;

                if (lapTime < data.bestLap)
                {
                    data.bestLap = lapTime;
                    data.bestLapIndex = lapNumber;
                }
            }

            UpdateBestLapLeaderboardUI();
            UpdateCollisionLeaderboardUI();

        }

        private void UpdateBestLapLeaderboardUI()
        {
            var sorted = agents.Values.OrderBy(a => a.bestLap).ToList();

            foreach (Transform child in bestLapContentParent)
                Destroy(child.gameObject);

            foreach (var agent in sorted)
            {
                var btn = Instantiate(bestLapEntryPrefab, bestLapContentParent);
                var text = btn.GetComponentInChildren<TextMeshProUGUI>();
                text.text = $"{agent.name} | {agent.bestLap:F2}s - L{agent.bestLapIndex}";

                var button = btn.GetComponent<Button>();
                button.onClick.AddListener(() => ShowDetail(agent));
                agent.buttonComponent = button;
                agent.textComponent = text;
            }

            Canvas.ForceUpdateCanvases();
            bestLapScrollRect.verticalNormalizedPosition = 1f;
        }

        private void UpdateCollisionLeaderboardUI()
        {
            var sorted = agents.Values
                .OrderBy(a => a.BestCollisionValue)
                .ToList();

            foreach (Transform child in bestCollisionContentParent)
                Destroy(child.gameObject);

            foreach (var agent in sorted)
            {
                var btn = Instantiate(bestCollisionEntryPrefab, bestCollisionContentParent);
                var text = btn.GetComponentInChildren<TextMeshProUGUI>();
                text.text = $"{agent.name} | {agent.BestCollisionValue} hits | L{agent.BestCollisionLapIndex}";

                var button = btn.GetComponent<Button>();
                button.onClick.AddListener(() => ShowDetail(agent));
            }

            Canvas.ForceUpdateCanvases();
            bestCollisionScrollRect.verticalNormalizedPosition = 1f;
        }


        public void RecordWallCollision(string agentName, int collisionCount, int lapIndex)
        {
            if (!agents.ContainsKey(agentName)) return;

            var data = agents[agentName];

            if (lapIndex < 0) return;
            // Pastikan list cukup panjang
            while (data.wallCollisionsPerLap.Count <= lapIndex)
            {
                data.wallCollisionsPerLap.Add(0);
            }

            // Simpan jumlah collision ke index sesuai lap ke-n
            data.wallCollisionsPerLap[lapIndex] = collisionCount;
            UpdateCollisionLeaderboardUI();

        }



        private string FormatMinute(float seconds)
        {
            float min = Mathf.Floor(seconds / 60f);
            float sec = seconds % 60f;
            return $"{min:0.#}m {sec:00.00}s";
        }

        private void ShowDetail(AgentData data)
        {
            detailPanel.SetActive(true);
            agentNameText.text = $"Agent: {data.name}";
            bestLapText.text = $"Best Lap: {data.bestLap:F2}s (L{data.bestLapIndex})";
            totalTimeText.text = $"Total Time: {FormatMinute(data.lapHistory.Sum())}";
            wallCollisionsText.text = $"Total Hits: {data.totalWallCollisions}";

            int lapStartIndex = Mathf.Max(0, data.lapHistory.Count - 10);
            lapListText.text = string.Join("\n", data.lapHistory
                .Skip(lapStartIndex)
                .Take(10)
                .Select((t, i) => {
                    int realIndex = lapStartIndex + i;
                    string lapTime = $"{t:F2}s";
                    string collisions = realIndex < data.wallCollisionsPerLap.Count ? $" - {data.wallCollisionsPerLap[realIndex]} hits" : "";
                    return $"Lap {realIndex + 1}: {lapTime}{collisions}";
                }));

            if (closePanel != null)
            {
                closePanel.SetActive(true);
                closePanel.GetComponent<Button>().onClick.RemoveAllListeners();
                closePanel.GetComponent<Button>().onClick.AddListener(HideDetail);
            }
        }

        private void HideDetail()
        {
            detailPanel.SetActive(false);
        }

        public void SetLapHistoryFromArray(string agentName, float[] lapArray, int lapCount)
        {
            if (!agents.ContainsKey(agentName)) return;
            agents[agentName].lapHistory = lapArray.Take(lapCount).ToList();
        }
    }
}
