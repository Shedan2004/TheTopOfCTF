using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class OnlineCardMovementScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public OnlineCardController CC;

    Camera MainCamera;
    Vector3 offset;
    public Transform DefaultParent, DefaultTempCardParent;
    GameObject TempCardGO;
    public bool IsDraggable;
    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);
    
        DefaultParent = DefaultTempCardParent = transform.parent;

        Debug.Log("DefaultParent: " + DefaultParent);
        Debug.Log("PlayerTurn: " + OnlineGameManagerScript.Instance.IsPlayerTurn);
        Debug.Log("FieldType: " + DefaultParent.GetComponent<OnlineDropPlaceScript>().Type);

        IsDraggable = OnlineGameManagerScript.Instance.IsPlayerTurn &&
                      (
                      (DefaultParent.GetComponent<OnlineDropPlaceScript>().Type == FieldType.SELF_HAND &&
                      OnlineGameManagerScript.Instance.PlayerMana >= CC.Card.ManaCost) ||
                      (DefaultParent.GetComponent<OnlineDropPlaceScript>().Type == FieldType.SELF_FIELD &&
                      CC.Card.CanAttack)
                      );

        if (!IsDraggable)
        {
            return;
        }

        if (CC.Card.IsSpell || CC.Card.CanAttack)
        {
            OnlineGameManagerScript.Instance.HighlightTargets(CC, true);
        }

        TempCardGO.transform.SetParent(DefaultParent);
        TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(DefaultParent.parent);
        
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
        {
            return;
        }

        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + offset;

        if (!CC.Card.IsSpell)
        {
            if (TempCardGO.transform.parent != DefaultTempCardParent)
            {
                TempCardGO.transform.SetParent(DefaultTempCardParent);
            }

            if (DefaultParent.GetComponent<OnlineDropPlaceScript>().Type != FieldType.SELF_FIELD)
            {
                CheckPosition();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
        {
            return;
        }

        OnlineGameManagerScript.Instance.HighlightTargets(CC, false);

        CC.transform.SetParent(DefaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());
        TempCardGO.transform.SetParent(GameObject.Find("OnlineGameCanvas").transform);
        TempCardGO.transform.localPosition = new Vector3(1800, 0);
    }

    void CheckPosition()
    {
        int newIndex = DefaultTempCardParent.childCount;

        for (int i = 0; i < DefaultTempCardParent.childCount; i++)
        {
            if (transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;

                if (TempCardGO.transform.GetSiblingIndex() < newIndex)
                {
                    newIndex--;
                }

                break;
            }
        }

        TempCardGO.transform.SetSiblingIndex(newIndex);
    }
    /*
    public void MoveToField(Transform field)
    {
        transform.SetParent(GameObject.Find("OnlineGameCanvas").transform);
        transform.DOMove(field.position, .5f);
    }

    public void MoveToTarget(Transform target)
    {
        StartCoroutine(MoveToTargetCor(target));
    }

    IEnumerator MoveToTargetCor(Transform target)
    {
        Vector3 pos = transform.position;
        Transform parent = transform.parent;
        int index = transform.GetSiblingIndex();

        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;

        transform.SetParent(GameObject.Find("OnlineGameCanvas").transform);

        transform.DOMove(target.position, .25f);

        yield return new WaitForSeconds(.25f);

        transform.DOMove(pos, .25f);

        yield return new WaitForSeconds(.25f);

        transform.SetParent(parent);
        transform.SetSiblingIndex(index);
        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
    */
}
