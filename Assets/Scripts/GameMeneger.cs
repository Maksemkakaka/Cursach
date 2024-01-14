using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject startScreen; // Переменная для экрана начала игры
    public GameObject endScreen;   // Переменная для экрана окончания игры

    void Start()
    {
        PauseGame();
    }
    public void PauseGame()
    {
        Time.timeScale = 0; // Останавливаем игру
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1; // Запускаем игру
        startScreen.SetActive(false);
    }

    public void EndGame()
    {
        // Показать экран окончания игры
        endScreen.SetActive(true);
    }

    public void ShowStartScreen()
    {
        // Показать экран начала игры
        startScreen.SetActive(true);
        endScreen.SetActive(false);
    }

    public void RestartGame()
    {
        // Перезапуск сцены/игры
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
