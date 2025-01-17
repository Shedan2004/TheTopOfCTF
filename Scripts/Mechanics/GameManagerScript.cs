using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Game
{
    public List<Card> EnemyDeck, PlayerDeck;

    public Game()
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
public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;

    public Game CurrentGame;
    public Transform EnemyHand, PlayerHand,
                     EnemyField, PlayerField;
    public GameObject CardPref;
    int Turn, TurnTime = 30;
    public TextMeshProUGUI TurnTimeTxt;
    public Button EndTurnButton;

    public int PlayerMana = 10, EnemyMana = 10;
    public TextMeshProUGUI PlayerManaTxt, EnemyManaTxt;

    public int PlayerHP = 30, EnemyHP = 30;
    public TextMeshProUGUI PlayerHPTxt, EnemyHPTxt;

    public GameObject ResultGO;
    public TextMeshProUGUI ResultTxt;

    public AttackedHero EnemyHero, PlayerHero;
    
    public List<CardController> PlayerHandCards = new List<CardController>(),
                                PlayerFieldCards = new List<CardController>(),
                                EnemyHandCards = new List<CardController>(),
                                EnemyFieldCards = new List<CardController>();


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
        StartGame();
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
        Turn = 0;

        EndTurnButton.interactable = true;

        CurrentGame = new Game();

        GiveHandCards(CurrentGame.EnemyDeck, EnemyHand);
        GiveHandCards(CurrentGame.PlayerDeck, PlayerHand);

        PlayerMana = EnemyMana = 10;
        PlayerHP = EnemyHP = 30;

        ShowHP();
        ShowMana();

        ResultGO.SetActive(false);

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
        CardController cardC = cardGO.GetComponent<CardController>();

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

            StartCoroutine(EnemyTurn(EnemyHandCards));
        }
    }

    IEnumerator EnemyTurn(List<CardController> cards)
    {
        yield return new WaitForSeconds(1);
        int count = cards.Count == 1 ? 1 :
                    Random.Range(0, cards.Count);
        for (int i = 0; i < count; i++)
        {
            if (EnemyFieldCards.Count > 5 || 
                EnemyMana == 0 ||
                EnemyHandCards.Count == 0)
            {
                break;
            }

            List<CardController> cardslist = cards.FindAll(x => EnemyMana >= x.Card.ManaCost && !x.Card.IsSpell);

            if (cardslist.Count == 0)
            {
                break;
            }

            cardslist[0].GetComponent<CardMovementScript>().MoveToField(EnemyField);

            yield return new WaitForSeconds(.51f);

            cardslist[0].transform.SetParent(EnemyField);

            cardslist[0].OnCast();
        }

        yield return new WaitForSeconds(1);

        while(EnemyFieldCards.Exists(x => x.Card.CanAttack))
        {
            var activeCard = EnemyFieldCards.FindAll(x => x.Card.CanAttack)[0];
            bool hasProvocation = PlayerFieldCards.Exists(x => x.Card.IsProvocation);

            if (hasProvocation ||
                Random.Range(0, 2) == 0 &&
                PlayerFieldCards.Count > 0)
            {
                CardController enemy;

                if (hasProvocation)
                {
                    enemy = PlayerFieldCards.Find(x => x.Card.IsProvocation);
                }

                else
                {
                    enemy = PlayerFieldCards[Random.Range(0, PlayerFieldCards.Count)];
                }

                Debug.Log(activeCard.Card.Name + " (" + activeCard.Card.Attack + "; " + activeCard.Card.Defence + ")     ---> " +
                          enemy.Card.Name + " (" + enemy.Card.Attack + "; " + enemy.Card.Defence + ")");

                activeCard.Movement.MoveToTarget(enemy.transform);
                yield return new WaitForSeconds(.75f);

                CardsFight(enemy, activeCard);
            }
            else
            {
                Debug.Log(activeCard.Card.Name + " (" + activeCard.Card.Attack + ") Attacked hero");

                activeCard.GetComponent<CardMovementScript>().MoveToTarget(PlayerHero.transform);
                yield return new WaitForSeconds(.75f);

                DamageHero(activeCard, false);
            }

            yield return new WaitForSeconds(.2f);
        }

        yield return new WaitForSeconds(1);
        ChangeTurn();
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

    public void CardsFight(CardController attacker, CardController defender)
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

    public void DamageHero(CardController card, bool IsEnemyAttacked)
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

    public void HighlightTargets(CardController attacker, bool highlight)
    {
        List<CardController> targets = new List<CardController>();

        if (attacker.Card.IsSpell)
        {
            if (attacker.Card.SpellTarget == Card.TargetType.NO_TARGET)
            {
                targets = new List<CardController>();
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
