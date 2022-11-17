using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //private HexToRGB hexToRGB;


    [SerializeField] private Color unHoveredColor;
    [SerializeField] private Color hoveredColor;


    // Start is called before the first frame update
    void Awake()
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetComponent<Image>().color  = hoveredColor;
        // transform.GetComponent<Image>().color = hexToRGB.HexToFloatNormalized(hoveredColor);

    }

    // upon pointer exiting button reset square grid and disable ui
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetComponent<Image>().color  = unHoveredColor;
        // transform.GetComponent<Image>().color = hexToRGB.HexToFloatNormalized(unHoveredColor);
    }

}
