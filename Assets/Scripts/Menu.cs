using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This function is called when the Play button is pressed
    public void PlayGame()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    // This function is called when the Instructions button is pressed
    public void ShowInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    // This function is called when the Quit button is pressed
    public void QuitGame()
    {
        // If we're running in the editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // If we're running in a built application
            Application.Quit();
        #endif
    }
}