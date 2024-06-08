using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class LobbyManagerScript : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject NicknameGO;
    [SerializeField] GameObject LobbyGO;

    public InputField createInput;
    public InputField joinInput;
    public InputField nicknameInput;

    public static int turn = 0;

    public static Player _player;

    private void Start()
    {
        _player = PhotonNetwork.LocalPlayer;
    }

    public void NicknameInput()
    {
        _player.NickName = nicknameInput.text;
        NicknameGO.SetActive(false);
        LobbyGO.SetActive(true);
    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        turn = 0;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
        
    }

    public void JoinRoom()
    {
        turn = 1;
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        {
            PhotonNetwork.LoadLevel("OnlineGameScene");
        }
    }
}
