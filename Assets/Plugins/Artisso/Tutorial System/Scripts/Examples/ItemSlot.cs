using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class ItemSlot : MonoBehaviour, IDropHandler
{

    // Start is called before the first frame update
    public void OnDrop(PointerEventData eventData)
    {
       if (eventData.pointerDrag != null)
        {
            Debug.Log("Drpping");
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = this.GetComponent<RectTransform>().anchoredPosition;

        }
    }

    public void GoToNextStep()
    {
        Debug.Log("Dropped");
    }


}
