using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManagerScript.Instance.IsPlayerTurn)
        {
            return;
        }

        CardController spell = eventData.pointerDrag.GetComponent<CardController>(),
                       target = GetComponent<CardController>();

        if (spell &&
            spell.IsPlayerCard &&
            target.Card.IsPlaced &&
            GameManagerScript.Instance.PlayerMana >= spell.Card.ManaCost)
        {
            if ((spell.Card.SpellTarget == Card.TargetType.ALLY_CARD_TARGET &&
                target.IsPlayerCard) ||
                (spell.Card.SpellTarget == Card.TargetType.ENEMY_CARD_TARGET &&
                !target.IsPlayerCard))
            {
                GameManagerScript.Instance.ReduceMana(true, spell.Card.ManaCost);
                spell.UseSpell(target);
                GameManagerScript.Instance.CheckCardsForManaAvailability();
            }
        }
    }
}
