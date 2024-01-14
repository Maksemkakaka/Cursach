using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject startScreen; // ���������� ��� ������ ������ ����
    public GameObject endScreen;   // ���������� ��� ������ ��������� ����

    void Start()
    {
        PauseGame();
    }
    public void PauseGame()
    {
        Time.timeScale = 0; // ������������� ����
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1; // ��������� ����
        startScreen.SetActive(false);
    }

    public void EndGame()
    {
        // �������� ����� ��������� ����
        endScreen.SetActive(true);
    }

    public void ShowStartScreen()
    {
        // �������� ����� ������ ����
        startScreen.SetActive(true);
        endScreen.SetActive(false);
    }

    public void RestartGame()
    {
        // ���������� �����/����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
