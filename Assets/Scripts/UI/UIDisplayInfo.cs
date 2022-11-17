using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIDisplayInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject objectivesUI;
    private float objectivesUIWidth;
    private bool open = false;
    private bool moving = false;
    private Vector3 uiClosedPosition;
    private Vector3 uiOpenPosition;
    private float moovementDirection;
    private HexToRGB hexToRGB;

    private string unHoveredColor = "416A29";
    private string hoveredColor = "411F29";

    // 190 123

    private void Awake()
    {
        hexToRGB = new HexToRGB();
    }


    // Start is called before the first frame update
    void Start()
    {
        moovementDirection = this.transform.GetChild(0).GetComponent<RectTransform>().localScale.x;

        objectivesUIWidth = objectivesUI.transform.GetChild(0).GetComponent<RectTransform>().rect.width;
        
        uiClosedPosition = objectivesUI.transform.localPosition;
        uiOpenPosition = new Vector3(uiClosedPosition.x + moovementDirection * objectivesUIWidth, uiClosedPosition.y, uiClosedPosition.z);

    }

   

    public void OnPointerEnter(PointerEventData eventData)
    {

        transform.GetComponent<Image>().color = hexToRGB.HexToFloatNormalized(hoveredColor);

    }

  
    public void OnPointerExit(PointerEventData eventData)
    {

        transform.GetComponent<Image>().color = hexToRGB.HexToFloatNormalized(unHoveredColor);

    }

    private void Update()
    {
        if (moving)
        {
            
            Vector3 movingFrom = objectivesUI.transform.localPosition;
            Vector3 movingToo;
            if (open)
            {
                movingToo = uiClosedPosition;
            }
            else 
            {
                movingToo = uiOpenPosition;
            }
            objectivesUI.transform.localPosition = Vector3.MoveTowards(movingFrom, movingToo, Time.deltaTime * 300);

            if (objectivesUI.transform.localPosition == movingToo)
            {
                this.enabled = false;
                open = !open;
                moving = false;
            }
        }
       
    }

    public void DisplayUIInfo()
    {

        if (!moving)
        {
            moving = true;
            this.enabled = true;
        }
        
    }



}
