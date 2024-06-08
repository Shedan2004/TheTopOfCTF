using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class OnlineAttackedHero : MonoBehaviour, IDropHandler
{
    public enum HeroType
    {
        ENEMY,
        PLAYER
    }

    public HeroType Type;
    public OnlineGameManagerScript GameManager;
    public Color NormalColor, TargetColor;
    public void OnDrop(PointerEventData eventData)
    {
        if (!OnlineGameManagerScript.Instance.IsPlayerTurn)
        {
            return;
        }

        OnlineCardController card = eventData.pointerDrag.GetComponent<OnlineCardController>();

        if (card &&
            card.Card.CanAttack &&
            Type == HeroType.ENEMY &&
            !OnlineGameManagerScript.Instance.EnemyFieldCards.Exists(x => x.Card.IsProvocation))
        {
            OnlineGameManagerScript.Instance.DamageHero(card, true);
        }
    }

    public void HighlightAsTarget(bool highlight)
    {
        GetComponent<Image>().color = highlight ?
                                    TargetColor :
                                    NormalColor;
    }
}
