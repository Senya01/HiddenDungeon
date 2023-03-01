using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Panel : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private string[] statusStrings;
    [SerializeField] private GameObject newLevelButton;
    [SerializeField] private LevelManager levelManager;

    public void Win()
    {
        Time.timeScale = 0;
        newLevelButton.SetActive(true);
        gameObject.SetActive(true);
        statusText.text = statusStrings[0];
    }
    
    public void Loss()
    {
        Time.timeScale = 0;
        newLevelButton.SetActive(false);
        gameObject.SetActive(true);
        statusText.text = statusStrings[1];
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void NextLevel()
    {
        if (CurrentLevel.currentLevel < levelManager.levels.Length)
        {
            CurrentLevel.currentLevel++;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
