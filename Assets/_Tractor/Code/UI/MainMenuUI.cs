using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas MainMenuCanvas;
    [SerializeField] private Canvas LevelsCanvas;

    private void Start()
    {
        MainMenuCanvas.enabled = true;
        LevelsCanvas.enabled = false;
    }

    public void StartButtonPress()
    {
        MainMenuCanvas.enabled = false;
        LevelsCanvas.enabled = true;
    }

    public void BackButtonPress()
    {
        MainMenuCanvas.enabled = true;
        LevelsCanvas.enabled = false;
    }
}
