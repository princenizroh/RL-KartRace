using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace KartRace
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button singlePlayerButton;
        [SerializeField] private Button multiplayerButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            if (singlePlayerButton != null)
            {
                singlePlayerButton.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene("SinglePlayer");
                });
            }
            if (multiplayerButton != null)
            {
                multiplayerButton.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene("Multiplayer");
                });
            }
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(() =>
                {
                    Application.Quit();
                });
            }
        }
    }
}
