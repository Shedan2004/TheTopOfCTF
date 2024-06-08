using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnlineCardInfoScript : MonoBehaviour
{
    public OnlineCardController CC;

    public Image Logo;
    public TextMeshProUGUI Name, Attack, Defence, ManaCost;
    public GameObject HideObj, HighlightedObj;
    public Color NormalColor, TargetColor, SpellTargetColor;

    public void HideCardInfo()
    {
        HideObj.SetActive(true);
        ManaCost.text = "";
    }
    public void ShowCardInfo()
    {
        HideObj.SetActive(false);

        Logo.sprite = CC.Card.Logo;
        Logo.preserveAspect = true;
        Name.text = CC.Card.Name;

        RefreshData();
    }

    public void RefreshData()
    {
        Attack.text = CC.Card.Attack.ToString();
        Defence.text = CC.Card.Defence.ToString();
        ManaCost.text = CC.Card.ManaCost.ToString();   
    }

    public void HighlightCard(bool highlight)
    {
        HighlightedObj.SetActive(highlight);
    }

    public void HighlightManaAvailability(int currentMana)
    {
        GetComponent<CanvasGroup>().alpha = currentMana >= CC.Card.ManaCost ?
                                            1 :
                                            .5f;
    }

    public void HighlightAsTarget(bool highlight)
    {
        GetComponent<Image>().color = highlight ?
                                    TargetColor :
                                    NormalColor;
    }

    public void HighlightAsSpellTarget(bool highlight)
    {
        GetComponent<Image>().color = highlight ?
                                    SpellTargetColor :
                                    NormalColor;
    }
}
