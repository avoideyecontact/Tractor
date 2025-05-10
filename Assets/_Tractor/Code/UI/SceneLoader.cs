using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(StaticData.MainMenuScene);
    }
    public void LoadBeach()
    {
        SceneManager.LoadScene(StaticData.BeachScene);
    }
    public void LoadSnow()
    {
        SceneManager.LoadScene(StaticData.SnowScene);
    }
}
