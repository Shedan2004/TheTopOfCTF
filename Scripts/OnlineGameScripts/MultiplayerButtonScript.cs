using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerButtonScript : MonoBehaviour
{
    public void MultiplayerModePressed()
    {
        SceneManager.LoadScene("LoadingScene");
    }
}
