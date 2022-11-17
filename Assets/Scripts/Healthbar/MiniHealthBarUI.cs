using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniHealthBarUI : MonoBehaviour
{

    [SerializeField] bool moving = false;
    [SerializeField] bool open = true;

    List<GameObject> UIList;

    [SerializeField] float objectivesUIMaxHeight;

    public Transform rectTransform;


    // Start is called before the first frame update
    private void Awake()
    {
        UIList = new List<GameObject>();
        for (int i = 0; i< transform.childCount ; i++)
        {
            
           UIList.Add(transform.GetChild(i).gameObject);
        }
    }

    void Start()
    {
        objectivesUIMaxHeight = transform.GetComponent<RectTransform>().rect.height;

        

        this.enabled = false;

    }

    public void DisplayUIInfo()
    {
        if (!moving)
        {
            moving = true;
            this.enabled = true;
        }

    }

    private void SetTop( RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    private void SetBottom( RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    // Update is called once per frame
    void Update()
    {
        
            float movingFrom = rectTransform.GetComponent<RectTransform>().rect.height;
            float movingToo;
            if (open)
            {
                movingToo = 0;
            }
            else
            {
                movingToo = objectivesUIMaxHeight;
               
            }

            float currentHeight = Mathf.MoveTowards(movingFrom, movingToo, 300 * Time.deltaTime);
            RectTransform newRect = rectTransform.GetComponent<RectTransform>();
            newRect.sizeDelta = new Vector2(newRect.rect.width, currentHeight);

            transform.GetComponent<RectTransform>().sizeDelta = newRect.sizeDelta;




            float quarterHeight = currentHeight / 4;

          


            for (int i = 0 ; i < UIList.Count; i++ )
            {
                GameObject child = UIList[i];

                SetTop(child.transform.GetComponent<RectTransform>(), currentHeight * i / 4 );
                SetBottom(child.transform.GetComponent<RectTransform>(), (3 - i  ) * currentHeight / 4);

                RectTransform image = child.transform.GetChild(0).gameObject.transform.GetComponent<RectTransform>();
                RectTransform HPbar = child.transform.GetChild(1).gameObject.transform.GetComponent<RectTransform>();

                

                SetTop(image, quarterHeight * 2 / 15 );
                SetBottom(image, quarterHeight * 7 / 15);

                SetTop(HPbar, quarterHeight * 9 / 15);
                SetBottom(HPbar, quarterHeight / 15);


                //image.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, height - (i + 1) * height / 4);
                // m_XAxis = GUI.HorizontalSlider(new Rect(150, 20, 100, 80), m_XAxis, -50.0f, 50.0f);


            }

            if (currentHeight == movingToo)
            {
                this.enabled = false;
                open = !open;
                moving = false;

            }

        
    }

  
}
