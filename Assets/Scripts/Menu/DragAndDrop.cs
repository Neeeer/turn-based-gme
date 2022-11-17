using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour 
{
    [SerializeField] private Camera mainCamera;


    private Controls controls;



    [SerializeField] private Text partyNotReadyText;


    private GameObject draggableObject;


    [SerializeField] private bool dragging { get; set; }
    [SerializeField] private bool draggingCharacter { get; set; }
    [SerializeField] private bool draggingAbility { get; set; }


    public List<Image> abilitySlots { get; set; }
    public List<Image> characterSlots { get; set; }

    [SerializeField] private int partySize;
    [SerializeField] private int abilitySize;



    public void OnEnable()
    {
        controls.Enable();
    }

    public void OnDisable()
    {
        controls.Disable();
    }


    private void Awake()
    {
        draggingCharacter = false;
        draggingAbility = false;
        dragging = false;

        controls = new Controls();

        controls.clicks.Click.started += _ => press();
        controls.clicks.Click.canceled += _ => release();

        characterSlots = new List<Image>();

        for (int i = 0; i < partySize; i++)
        {
            characterSlots.Add(transform.GetChild(i).transform.GetComponent<Image>());
        }

        abilitySlots = new List<Image>();

        for (int i = 0; i < abilitySize; i++)
        {
            abilitySlots.Add(transform.GetChild(i).transform.GetComponent<Image>());
        }
    }

    private void press()
    {

        Vector2 position = controls.clicks.Position.ReadValue<Vector2>();

        RaycastHit2D hit2d = Physics2D.Raycast(position, Vector2.zero);


        if (hit2d.collider != null && (hit2d.transform.gameObject.layer == LayerMask.NameToLayer("DraggableCharacter") ))
        {

            draggingCharacter = true;

        }
        else if (hit2d.collider != null && hit2d.transform.gameObject.layer == LayerMask.NameToLayer("DraggableAbility"))
        {
          
            draggingAbility = true;

        }
        else
        {
            return;
        }

        draggableObject = Instantiate(hit2d.collider.gameObject);

        Image r = hit2d.collider.gameObject.GetComponent<Image>();
        Color newColor = r.color;
        newColor.a = 0.5f;
        draggableObject.GetComponent<Image>().color = newColor;

        dragging = true;

        StartCoroutine(dragUpdate());

    }

    public Vector2 getMousePosition()
    {
        return controls.clicks.Position.ReadValue<Vector2>();
    }

    
    private void release()
    {
        if (dragging)
        {
            Vector2 position = getMousePosition();
            RaycastHit2D hit2d = Physics2D.Raycast(position, Vector2.zero);
            
            if (hit2d.collider != null && hit2d.transform.gameObject.layer == LayerMask.NameToLayer("CharacterSlot"))
            {
                releaseOnUI(hit2d);
            }
            else if (hit2d.collider != null && hit2d.transform.gameObject.layer == LayerMask.NameToLayer("CharacterAbility"))
            {
                releaseOnUI(hit2d);
            }


            draggingCharacter = false;
            draggingAbility = false;


            dragging = false;
        }

    }


    public void partyNotReady()
    {
        partyNotReadyText.gameObject.SetActive(true);

        Color c = partyNotReadyText.color;
        c.a = 1f;
        partyNotReadyText.color = c;

        StartCoroutine(notReady());

    }

    public IEnumerator notReady()
    {

            Color c = partyNotReadyText.color;
            c.a -= Time.deltaTime;
            partyNotReadyText.color = c;

            if (c.a > 0)
            {
                yield return new WaitForEndOfFrame();
            }

            partyNotReadyText.gameObject.SetActive(false);
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }


    public IEnumerator dragUpdate()
    {
        while (dragging)
        {

            draggableObject.gameObject.transform.position = getMousePosition();

            yield return new WaitForEndOfFrame();

        }
    }

    public void releaseOnUI(RaycastHit2D hit2d)
    {
        string text = draggableObject.GetComponentInChildren<Text>().text;

        if (draggingAbility)
        {
            foreach (Image abilitySlot in abilitySlots)
            {
                if (abilitySlot.GetComponentInChildren<Text>().text == text)
                {
                    abilitySlot.GetComponentInChildren<Text>().text = "Empty";
                }
            }
        }
        if (draggingCharacter)
        {

            foreach (Image charSlots in characterSlots)
            {
                if (charSlots.GetComponentInChildren<Text>().text == text)
                {
                    charSlots.GetComponentInChildren<Text>().text = "Empty";
                }
            }
        }

        hit2d.collider.GetComponentInChildren<Text>().text = text;

        Destroy(draggableObject);



    }


    // Update is called once per frame
    void Update()
    {

    }


    public void unEquipAbilities()
    {
        foreach (Image abilitySlot in abilitySlots)
        {
            abilitySlot.GetComponentInChildren<Text>().text = "Empty";

        }
    }
    public void unEquipCharacters()
    {
        foreach (Image charSlot in characterSlots)
        {
            charSlot.GetComponentInChildren<Text>().text = "Empty";

        }

    }

    public bool checkIfPartyReady()
    {
        List<string> charList = getCharacterSlots();

        foreach (string text in charList)
        {
            if (text == "Empty")
            {
                partyNotReady();
                return false;
            }
        }

        return true;
    }

    public List<string>  getCharacterSlots()
    {
        List<string> charList = new List<string>() ;


        foreach (Image charSlot in characterSlots)
        {
            charList.Add(charSlot.GetComponentInChildren<Text>().text);

        }

        return charList;
    }


}
