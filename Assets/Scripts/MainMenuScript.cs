using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public void playButton()
    {
        SceneManager.LoadScene("LoadingScene");
    }


    public void exitButton()
    {
        Debug.Log("Exited.");
        Application.Quit();
    }

}
