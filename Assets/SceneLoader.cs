using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Scene(int level)
    {
        SceneManager.LoadScene(level);
        Debug.Log("Load" + level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
