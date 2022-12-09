using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject Options;

    private void Start()
    {
        StartCoroutine(delayStart());
    }

    public IEnumerator delayStart()
    {
        yield return new WaitForSeconds(0.1f);
        Options.SetActive(false);
    }

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
