using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class OnlineGame
{
    public List<Card> EnemyDeck, PlayerDeck;

    public OnlineGame()
    {
        EnemyDeck = GiveDeckCards();
        PlayerDeck = GiveDeckCards(); 
    }

    List<Card> GiveDeckCards()
    {
        List<Card> list = new List<Card>();

        for (int i = 0; i < 10; i++)
        {
            list.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)].GetCopy());
        }
        return list;
    }
}
public class OnlineGameManagerScript : MonoBehaviourPunCallbacks
{
    public static OnlineGameManagerScript Instance;

    PhotonView view;
    bool gameStarted = false;

    

    public TextMeshProUGUI PlayerNickname;
    public TextMeshProUGUI EnemyNickname;

    public OnlineGame CurrentGame;
    public Transform EnemyHand, PlayerHand,
                     EnemyField, PlayerField;
    public GameObject CardPref;
    public int Turn = 0; 
    int TurnTime = 30;
    public TextMeshProUGUI TurnTimeTxt;
    public Button EndTurnButton;

    public int PlayerMana = 10, EnemyMana = 10;
    public TextMeshProUGUI PlayerManaTxt, EnemyManaTxt;

    public int PlayerHP = 30, EnemyHP = 30;
    public TextMeshProUGUI PlayerHPTxt, EnemyHPTxt;

    public GameObject ResultGO;
    public TextMeshProUGUI ResultTxt;

    public GameObject OnlineGameGO;
    public GameObject WaitingGO;

    public OnlineAttackedHero EnemyHero, PlayerHero;
    
    public List<OnlineCardController> PlayerHandCards = new List<OnlineCardController>(),
                                PlayerFieldCards = new List<OnlineCardController>(),
                                EnemyHandCards = new List<OnlineCardController>(),
                                EnemyFieldCards = new List<OnlineCardController>();

    public bool IsPlayerTurn
    {
        get
        {
            return Turn % 2 == 0;
        }
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        view = GetComponent<PhotonView>();
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }


    private void OnEventReceived(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        Debug.Log("eventCode: " + eventCode);
        //Debug.Log("CustomEventCodes: " + (byte)CustomEventCodes.PlayerJoined);
        Debug.Log("PlayerCount: " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !gameStarted)
        {
            StartGame();
            gameStarted = true;
        }
    }

    public void ReastartGame()
    {
        StopAllCoroutines();

        foreach(var card in PlayerHandCards)
        {
            Destroy(card.gameObject);
        }
        foreach (var card in EnemyHandCards)
        {
            Destroy(card.gameObject);
        }
        foreach (var card in PlayerFieldCards)
        {
            Destroy(card.gameObject);
        }
        foreach (var card in EnemyFieldCards)
        {
            Destroy(card.gameObject);
        }

        PlayerHandCards.Clear();
        EnemyHandCards.Clear();
        PlayerFieldCards.Clear();
        EnemyFieldCards.Clear();

        StartGame();
    }

    public void StartGame()
    {
        Turn = LobbyManagerScript.turn;

        Player[] players = PhotonNetwork.PlayerList;

        PlayerNickname.text = LobbyManagerScript._player.NickName;

        foreach (var player in players)
        {
            if (player.NickName != PlayerNickname.text)
            {
                EnemyNickname.text = player.NickName;
            }
        }

        WaitingGO.SetActive(false);
        OnlineGameGO.SetActive(true);
        
        if (IsPlayerTurn)
           EndTurnButton.interactable = true;
        else
            EndTurnButton.interactable = false;

        CurrentGame = new OnlineGame();

        GiveHandCards(CurrentGame.EnemyDeck, EnemyHand);
        GiveHandCards(CurrentGame.PlayerDeck, PlayerHand);

        PlayerMana = EnemyMana = 10;
        PlayerHP = EnemyHP = 30;

        ShowHP();
        ShowMana();

        ResultGO.SetActive(false);

        OnlineGameMainValuesScript.SaveHPMP();
        OnlineGameMainValuesScript.SaveCardInField(EnemyHandCards);
        OnlineGameMainValuesScript.SaveCardInField(PlayerHandCards);
        OnlineGameMainValuesScript.SaveCardInField(EnemyFieldCards);
        OnlineGameMainValuesScript.SaveCardInField(PlayerFieldCards);

        StartCoroutine(TurnFunc());
    }

    void GiveHandCards(List<Card> deck, Transform hand)
    {
        int i = 0;
        while(i++ < 4)
        {
            GiveCardToHand(deck, hand);
        }
    }

    void GiveCardToHand(List<Card> deck, Transform hand)
    {
        if (deck.Count == 0)
        {
            return;
        }

        CreateCardPref(deck[0], hand);

        deck.RemoveAt(0);
    }

    void CreateCardPref(Card card, Transform hand)
    {
        GameObject cardGO = Instantiate(CardPref, hand, false);
        OnlineCardController cardC = cardGO.GetComponent<OnlineCardController>();

        cardC.Init(card, hand == PlayerHand);

        if(cardC.IsPlayerCard)
        {
            PlayerHandCards.Add(cardC);
        }
        else
        {
            EnemyHandCards.Add(cardC);
        }
    }
    IEnumerator TurnFunc()
    {
        OnlineGameMainValuesScript.LoadHPMP();
        OnlineGameMainValuesScript.LoadCardInField(EnemyHandCards);
        OnlineGameMainValuesScript.LoadCardInField(PlayerHandCards);
        OnlineGameMainValuesScript.LoadCardInField(EnemyFieldCards);
        OnlineGameMainValuesScript.LoadCardInField(PlayerFieldCards);

        TurnTime = 30;
        TurnTimeTxt.text = TurnTime.ToString();

        foreach (var card in PlayerFieldCards)
        {
            card.Info.HighlightCard(false);
        }

        CheckCardsForManaAvailability();

        if (IsPlayerTurn)
        {
            foreach (var card in PlayerFieldCards)
            {
                card.Card.CanAttack = true;
                card.Info.HighlightCard(true);
                card.Ability.OnNewTurn();
            }

            while (TurnTime-- > 0)
            {
                TurnTimeTxt.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
            ChangeTurn();
        }
        else
        {
            foreach (var card in EnemyFieldCards)
            {
                card.Card.CanAttack = true;
                card.Ability.OnNewTurn();
            }
        }
    }

    public void ChangeTurn()
    {
        StopAllCoroutines();
        Turn++;

        EndTurnButton.interactable = IsPlayerTurn;

        if (IsPlayerTurn)
        {
            GiveNewCards();

            PlayerMana = EnemyMana = 10;
            ShowMana();
        }

        StartCoroutine(TurnFunc());
    }


    void GiveNewCards()
    {
        GiveCardToHand(CurrentGame.EnemyDeck, EnemyHand);
        GiveCardToHand(CurrentGame.PlayerDeck, PlayerHand);
    }

    public void CardsFight(OnlineCardController attacker, OnlineCardController defender)
    {
        defender.Card.GetDamage(attacker.Card.Attack);
        attacker.OnDamageDeal();
        defender.OnTakeDamage(attacker);

        attacker.Card.GetDamage(defender.Card.Attack);
        attacker.OnTakeDamage();

        attacker.CheckForAlive();
        defender.CheckForAlive();
    }

    public void ShowMana()
    {
        PlayerManaTxt.text = PlayerMana.ToString();
        EnemyManaTxt.text = EnemyMana.ToString();
    }

    public void ShowHP()
    {
        PlayerHPTxt.text = PlayerHP.ToString();
        EnemyHPTxt.text = EnemyHP.ToString();
    }

    public void ReduceMana(bool playerMana, int manaCost)
    {
        if (playerMana)
        {
            PlayerMana = Mathf.Clamp(PlayerMana - manaCost, 0, int.MaxValue);
        }
        else
        {
            EnemyMana = Mathf.Clamp(EnemyMana - manaCost, 0, int.MaxValue);
        }

        ShowMana();
    }

    public void DamageHero(OnlineCardController card, bool IsEnemyAttacked)
    {
        if (IsEnemyAttacked)
        {
            EnemyHP = Mathf.Clamp(EnemyHP - card.Card.Attack, 0, int.MaxValue);  
        }
        else
        {
            PlayerHP = Mathf.Clamp(PlayerHP - card.Card.Attack, 0, int.MaxValue);
        }

        ShowHP();
        card.OnDamageDeal();
        CheckForResult();
    }

    public void CheckForResult()
    {
        if (EnemyHP == 0 || PlayerHP == 0)
        {
            ResultGO.SetActive(true);
            StopAllCoroutines();

            if (EnemyHP == 0)
            {
                ResultTxt.text = "You win!";
            }
            else
            {
                ResultTxt.text = "You lose!";
            }
        }
    }

    public void CheckCardsForManaAvailability()
    {
        foreach (var card in  PlayerHandCards)
        {
            card.Info.HighlightManaAvailability(PlayerMana);
        }
    }

    public void HighlightTargets(OnlineCardController attacker, bool highlight)
    {
        List<OnlineCardController> targets = new List<OnlineCardController>();

        if (attacker.Card.IsSpell)
        {
            if (attacker.Card.SpellTarget == Card.TargetType.NO_TARGET)
            {
                targets = new List<OnlineCardController>();
            }

            else if (attacker.Card.SpellTarget == Card.TargetType.ALLY_CARD_TARGET)
            {
                targets = PlayerFieldCards;
            }

            else
            {
                targets = EnemyFieldCards;
            }
        }

        else
        {
            if (EnemyFieldCards.Exists(x => x.Card.IsProvocation))
            {
                targets = EnemyFieldCards.FindAll(x => x.Card.IsProvocation);
            }

            else
            {
                targets = EnemyFieldCards;
                EnemyHero.HighlightAsTarget(highlight);
            }
        }

        foreach (var card in targets)
        {
            if (attacker.Card.IsSpell)
            {
                card.Info.HighlightAsSpellTarget(highlight);
            }

            else
            {
                card.Info.HighlightAsTarget(highlight);
            }

        }
    }
}
