using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour
{

    public UnityEngine.UI.Slider healthSlider;
    public UnityEngine.UI.Image healthSlide;
    public UnityEngine.UI.Image healthBar;
    public UnityEngine.UI.Image healthUI;

    public UnityEngine.UI.Slider miniHealthSlider;

    public Text healthNumber;
    public TextMeshProUGUI characterType;


    List<UnityEngine.UI.Image> healthListUI;

    private Character character;


    // Start is called before the first frame update
    void Awake()
    {
        healthListUI = new List<UnityEngine.UI.Image>();

        healthListUI.Add(healthSlide);
        healthListUI.Add(healthBar);
        healthListUI.Add(healthUI);


        //removeHealthBar();
    }

    public void setCharactrerName()
    {
        
    }

    public void removeHealthBar()
    {
        foreach (UnityEngine.UI.Image i in healthListUI)
        {
            i.enabled = (false);
        }
        // healthNumber.enabled = false;
    }

    public void updateHealthBar(Character cha)
    {
        foreach (UnityEngine.UI.Image i in healthListUI)
        {
            i.enabled = true;
        }

        healthSlider.value = (float)cha.Health / cha.MaxHeath;
        miniHealthSlider.value = (float)cha.Health / cha.MaxHeath;

        healthNumber.text = cha.Health.ToString() + "/" + cha.MaxHeath.ToString();

        
    }

    public void assignCharacterToHbar(Character cha)
    {
        character = cha;
        characterType.text = character.getCharacterType();
    }

    public Character getCharacter()
    {
        return character;
    }

}
