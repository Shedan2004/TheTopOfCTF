using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnlineSpellTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!OnlineGameManagerScript.Instance.IsPlayerTurn)
        {
            return;
        }

        OnlineCardController spell = eventData.pointerDrag.GetComponent<OnlineCardController>(),
                       target = GetComponent<OnlineCardController>();

        if (spell &&
            spell.IsPlayerCard &&
            target.Card.IsPlaced &&
            OnlineGameManagerScript.Instance.PlayerMana >= spell.Card.ManaCost)
        {
            if ((spell.Card.SpellTarget == Card.TargetType.ALLY_CARD_TARGET &&
                target.IsPlayerCard) ||
                (spell.Card.SpellTarget == Card.TargetType.ENEMY_CARD_TARGET &&
                !target.IsPlayerCard))
            {
                OnlineGameManagerScript.Instance.ReduceMana(true, spell.Card.ManaCost);
                spell.UseSpell(target);
                OnlineGameManagerScript.Instance.CheckCardsForManaAvailability();
            }
        }
    }
}
