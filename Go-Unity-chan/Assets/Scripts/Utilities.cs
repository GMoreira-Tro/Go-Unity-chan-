using UnityEngine.SceneManagement;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
