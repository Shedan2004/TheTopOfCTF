using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class OnlinePlayersScript : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject OnlineGameGO;
    [SerializeField] GameObject DisconnectingGO;
    
    private Player _player;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_player == otherPlayer)
        {
            StopAllCoroutines();
            OnlineGameGO.SetActive(false);
            DisconnectingGO.SetActive(true);
        }
    }

    public void LeftRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("_MainMenuScene");
    }
}
