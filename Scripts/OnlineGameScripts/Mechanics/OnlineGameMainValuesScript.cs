using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class OnlineGameMainValuesScript : MonoBehaviourPunCallbacks
{
    public static OnlineGameManagerScript OnGameMng;
    public static Transform CloudEnemyHand, CloudPlayerHand,
                     CloudEnemyField, CloudPlayerField;
    static int CloudTurnTime = 30;
    public static TextMeshProUGUI CloudTurnTimeTxt;

    public static int CloudPlayerMana = 10, CloudEnemyMana = 10;
    public static TextMeshProUGUI CloudPlayerManaTxt, CloudEnemyManaTxt;

    public static int CloudPlayerHP = 30, CloudEnemyHP = 30;
    public static TextMeshProUGUI CloudPlayerHPTxt, CloudEnemyHPTxt;

    public static List<OnlineCardController> CloudPlayerHandCards = new List<OnlineCardController>(),
                                             CloudPlayerFieldCards = new List<OnlineCardController>(),
                                             CloudEnemyHandCards = new List<OnlineCardController>(),
                                             CloudEnemyFieldCards = new List<OnlineCardController>();
    
    public static void SaveCardInField(List<OnlineCardController> fieldCards)
    {
        if (fieldCards == OnGameMng.EnemyFieldCards)
        {
            CloudEnemyFieldCards = fieldCards;
        }

        else if (fieldCards == OnGameMng.PlayerFieldCards)
        {
            CloudPlayerFieldCards = fieldCards;
        }

        else if (fieldCards == OnGameMng.PlayerHandCards)
        {
            CloudPlayerHandCards = fieldCards;
        }

        else if (fieldCards == OnGameMng.EnemyHandCards)
        {
            CloudEnemyHandCards = fieldCards;
        }
    }

    public static void SaveHPMP()
    {
        CloudPlayerHP = OnGameMng.PlayerHP;
        CloudPlayerMana = OnGameMng.PlayerMana;

        CloudEnemyHP = OnGameMng.EnemyHP;
        CloudEnemyMana = OnGameMng.EnemyMana;
    }

    public static void LoadCardInField(List<OnlineCardController> fieldCards)
    {
        if (fieldCards == OnGameMng.EnemyFieldCards)
        {
            fieldCards = CloudEnemyFieldCards;
        }

        else if (fieldCards == OnGameMng.PlayerFieldCards)
        {
            fieldCards = CloudPlayerFieldCards;
        }

        else if (fieldCards == OnGameMng.PlayerHandCards)
        {
            fieldCards = CloudPlayerHandCards;
        }

        else if (fieldCards == OnGameMng.EnemyHandCards)
        {
            fieldCards = CloudEnemyHandCards;
        }
    }

    public static void LoadHPMP()
    {
        OnGameMng.PlayerHP = CloudPlayerHP;
        OnGameMng.PlayerMana = CloudPlayerMana;

        OnGameMng.EnemyHP = CloudEnemyHP;
        OnGameMng.EnemyMana = CloudEnemyMana;
    }
}
