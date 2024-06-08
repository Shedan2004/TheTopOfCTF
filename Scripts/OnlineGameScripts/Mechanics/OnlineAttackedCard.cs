using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnlineAttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(!OnlineGameManagerScript.Instance.IsPlayerTurn)
        {
            return;
        }

        OnlineCardController attacker = eventData.pointerDrag.GetComponent<OnlineCardController>(),
                       defender = GetComponent<OnlineCardController>();

        if (attacker&&
            attacker.Card.CanAttack && 
            defender.Card.IsPlaced)
        {
            if (OnlineGameManagerScript.Instance.EnemyFieldCards.Exists(x => x.Card.IsProvocation) &&
                !defender.Card.IsProvocation)
            {
                return;
            }

            OnlineGameManagerScript.Instance.CardsFight(attacker, defender);
        }
    }
}
