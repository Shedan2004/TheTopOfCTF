using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{
    public void ExitPressed()
    {
        SceneManager.LoadScene("_MainMenuScene");
    }
}
