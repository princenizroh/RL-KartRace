using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace KartRace
{
    public class BackToMenu : MonoBehaviour
    {
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene("MainMenu");
                });
            }
        }
    }
}
